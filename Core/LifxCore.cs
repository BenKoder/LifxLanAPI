//using LifxNet;
using Core.Messages;
using Core.Network;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
namespace Core
{
    public class LifxCore
    {
        //private LifxClient client = null;
        //public async void Start()
        //{
        //    LifxClient client = await LifxNet.LifxClient.CreateAsync();
        //    client.DeviceDiscovered += Client_DeviceDiscovered1; ;
        //    client.DeviceLost += Client_DeviceLost; ;
        //    client.StartDeviceDiscovery();
        //}

        //private async void Client_DeviceDiscovered1(object? sender, LifxClient.DeviceDiscoveryEventArgs e)
        //{
        //    if (client == null)
        //        return;

        //    var bulb = e.Device as LifxNet.LightBulb;
        //    if (bulb == null)
        //        return;

        //    await client.SetDevicePowerStateAsync(bulb, true); //Turn bulb on
        //    await client.SetColorAsync(bulb, LifxColors.Red, 2700); //Set color to Red and 2700K Temperature	
        //}

        //private void Client_DeviceLost(object? sender, LifxClient.DeviceDiscoveryEventArgs e)
        //{
        //    int i = 0;
        //}

        private ClientUdp _ClientUdp;

        /// <summary>
        /// Keeps track of all the ip addresses we recieve a response from when sending out a GetService broadcast
        /// to find out what lifx products are on the network. We do this because for some reason we are getting
        /// multiple respones from a single product to the GetService request. We only want to react to the response
        /// Once. By logging the Lifx response ip we can see if we have allready recieved a response from GetService
        /// </summary>
        private List<string> _BroadcastResponseTracker;

        private LifxProducts.ProductsList _LifxProductsOnNetwork;

        public delegate void LifxDeviceFoundDelegate(LifxProducts.ProductBase product);
        public event LifxDeviceFoundDelegate LifxDeviceFound = delegate { };

        public LifxCore()
        {
            // keeps track of response from GetService
            this._BroadcastResponseTracker = new List<string>();
            this._LifxProductsOnNetwork = new LifxProducts.ProductsList();

            this._ClientUdp = new ClientUdp();
            this._ClientUdp.RecievedData += _ClientUdp_RecievedData;
            this._ClientUdp.Start();

            
        }

   


        /// <summary>
        /// Sends out a broadcast messsage to the local network. Any Lifx product that
        /// recieves the message should send back one or more StateService (3) message
        /// </summary>
        public void FindDevicesOnNetwork()
        {
            byte[] DataToSend;

            this._BroadcastResponseTracker.Clear();

            // holds the message data we want to convert to broadcast out
            MessageCreator messageCreator = new MessageCreator();
            // create the message and get back the message in bytes
            DataToSend = messageCreator.CreateMessage_GetService().CombinePacketsIntoOne();

            /////////////////
            //TODO: Find out how to work out what the broadcast ip address is on differnet networks
            //this._ClientUdp.SendData(DataToSend, "192.168.1.255");
            
            // send the message out to the network
            this._ClientUdp.SendData(DataToSend, System.Net.IPAddress.Broadcast.ToString());

            
        }



        private void SendMessage(string IpAddress, byte[] DataToSend)
        {
            this._ClientUdp.SendData(DataToSend, IpAddress);
        }


        /// <summary>
        /// Message recieved from a Lifx product (well we hope its a lifx product)
        /// </summary>
        /// <param name="IpAddress">IP Addrss of the Lifex Product</param>
        /// <param name="Data">Data the Lifx Product has sent</param>
        private void _ClientUdp_RecievedData(string IpAddress, byte[] Data)
        {

            Packets.LifxPacket lifxPacket;
            RecievedMessage recievedMessage = new RecievedMessage();

            lifxPacket = recievedMessage.DecodeMessage(Data);
            if (lifxPacket == null)
                return;

            // remove the port number off the ip address
            string ipAddressWithoutPortNumber = IpAddress;
            int colonPosition = IpAddress.IndexOf(":"); ;
            if(colonPosition != -1)
            {
                ipAddressWithoutPortNumber = IpAddress.Substring(0, colonPosition);
            }

            this.ProcessMessage(ipAddressWithoutPortNumber, lifxPacket);

        }
        /// <summary>
        /// Used to lock part of code during multiple threading issues
        /// </summary>
        private object _ThreadLock = new object();
        private void ProcessMessage(string IpAddress, Packets.LifxPacket lifxPacket)
        {
            MessageCreator messageCreator;
            byte[] data;

            switch (lifxPacket.HeaderPacket.PayloadType)
            {
                case Packets.Payload.MessageType.StateService:

                    // a single lifx product might send multiple respones to the GetService
                    // if we have allready recieved the message, just ignore it.
                    if (this._BroadcastResponseTracker.Contains(IpAddress) == true)
                        return;
                    else
                        this._BroadcastResponseTracker.Add(IpAddress);

                    // send a message back to the lifx product asking them what they are
                    messageCreator = new MessageCreator();
                    data = messageCreator.CreateMessage_GetVersion(lifxPacket.HeaderPacket.Target, lifxPacket.HeaderPacket.Sequence + 1).CombinePacketsIntoOne();
                    this.SendMessage(IpAddress, data);
                    break;

                // Response from us sending a GetVersion message
                case Packets.Payload.MessageType.StateVersion:

                    LifxProducts.ProductBase LifxProduct = LifxProducts.ProductsList.CreateProduct(IpAddress,lifxPacket, ((Packets.Payload.Types.StateVersionPayload)lifxPacket.PayloadPacket).ProductEnumType);
                    
                    // if we could not work out which product we have
                    if (lifxPacket == null)
                        break;

                    // Getting some strange threading issues where 
                    // this code is getting run more than once at the same time on differnet threads
                    // need to make sure hole code runs before being run again.
                    lock (_ThreadLock)
                    {


                        // check to see if we allready have this product
                        if (LifxProduct == null)
                            break;
                        if (this._LifxProductsOnNetwork.Exists(LifxProduct.MacAddress) == true)
                        {
                            // check to see if the ip address has changed.
                            LifxProducts.ProductBase aProduct = this._LifxProductsOnNetwork.Find(LifxProduct.MacAddress);
                            // if the ip address is differnet we have the product but its ip address has changed
                            if (aProduct.IpAddresss != LifxProduct.IpAddresss)
                            {
                                // remove the product with the old ip addrss
                                this._LifxProductsOnNetwork.Remove(aProduct);

                            }
                            // we allrady know about this product so we don't need to add it
                            else
                                return;
                        }

                        LifxProduct.RaiseMessage += LifxProduct_RaiseMessage;
                        this._LifxProductsOnNetwork.Add(LifxProduct);
                        this.LifxDeviceFound(LifxProduct);

                        // send a message back to the lifx product asking them what there current color is.
                        // It will also send back the power level & label assigned to the lifx product (the name the user gave it)
                        messageCreator = new MessageCreator();
                        data = messageCreator.CreateMessage_GetColor(lifxPacket.HeaderPacket.Target, lifxPacket.HeaderPacket.Sequence + 1).CombinePacketsIntoOne();
                        this.SendMessage(IpAddress, data);
                    }

                    break;

                case Packets.Payload.MessageType.LightState:

                    // if the message being sent is in our list of products
                    // send the message payload onto that product for it to handle appropreatly
                    this._LifxProductsOnNetwork.Find(lifxPacket.HeaderPacket.Target)?.ProcessMessage(lifxPacket.PayloadPacket);
                    break;


                // a response to a set message we sent out.
                case Packets.Payload.MessageType.Acknowledgement:

                    this._LifxProductsOnNetwork.Find(lifxPacket.HeaderPacket.Target)?.AcknowledgementReceived(lifxPacket);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets called when a Lifx Product wants a message to be sent
        /// </summary>
        /// <param name="lifxPacket"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void LifxProduct_RaiseMessage(LifxProducts.ProductBase product, Packets.LifxPacket lifxPacket)
        {
            this.SendMessage(product.IpAddresss, lifxPacket.CombinePacketsIntoOne());
        }

        public void Finish()
        {
            this._ClientUdp.Dispose();
        }

        
  

    }

    //public class LifxColors
    //{

    //    public static LifxNet.Color Red { get => new LifxNet.Color() { R = 255, G = 0, B = 0 }; }
    //}
}