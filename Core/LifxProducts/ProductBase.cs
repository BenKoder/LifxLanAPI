using Core.Messages;
using Core.Packets;
using Core.Packets.Payload.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts
{
    public class ProductBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        private string _Label = string.Empty;
        /// <summary>
        /// The name the user has assgined to this product
        /// </summary>
        public string Label 
        { 
            get => this._Label;
            set
            {
                this._Label = value;
                this.OnPropertyChanged(this,nameof(this.Label));
            }
        }
        /// <summary>
        /// True when <see cref="PowerLevel"/> is greater than zero, else zero
        /// </summary>
        public bool IsPowerdOn { get; private set; }

        private UInt16 _PowerLevel = 0;
        /// <summary>
        /// The current power level of the device. 0 means off and any other value means on. 
        /// Note that 65535 is full power and during a power transition (i.e. via SetLightPower (117)) 
        /// the value may be any value between 0 and 65535
        /// </summary>
        public UInt16 PowerLevel 
        {
            get => _PowerLevel;
            set
            {
                _PowerLevel = value;
                if (_PowerLevel > 0)
                    IsPowerdOn = true;
                else
                    IsPowerdOn = false;
            }

        }
        /// <summary>
        /// The MAC address of the device
        /// </summary>
        public string MacAddress { get; set; }
        /// <summary>
        /// Not currently used. Would be the port the product is using but at present all use the same port so this has
        /// been hard coded.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// The IP address of the product on the local networok
        /// </summary>
        public string IpAddresss { get; set; }

        /// <summary>
        /// The exact product we are looking at (see https://lan.developer.lifx.com/docs/product-registry)
        /// </summary>
        public ProductType ProductType { get; set; }

        private int _MessageCount;
        /// <summary>
        /// Should be a number between 1 and 255.
        /// If number passed in is greator than 255, <see cref="MessageCount"/>
        /// will be set back to 1
        /// </summary>
        public int MessageCount
        {
            get => this._MessageCount;
            set
            {
                // dont' allow the number to be less than 0 or greater than 255.
                // set back to 1 if it is
                if (value < 0 || value > 255)
                    this._MessageCount = 1;
                else
                    this._MessageCount = value;
            }
        }

        /// <summary>
        /// Everytime a message is sent to this bulb, the date and time should be set
        /// </summary>
        public DateTime LastMessageSentAt { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Default is 50 milliseconds. No message should be sent if the previouse message  (<see cref="LastMessageSentAt"/>)
        /// was sent less than <see cref="TimeToAllowBetweenSendingMessagesInMilliSeconds"/>
        /// </summary>
        public int TimeToAllowBetweenSendingMessagesInMilliSeconds { get; set; } = 50;


        /// <summary>
        /// The last message that was sent across the network from this application to the device
        /// </summary>
        public LifxPacket LastLifxPacketMessageThatWasSent { get; set; } = null;

        public ProductBase(string MacAddress, int Port, string IpAddress, ProductType ProductType)
        {
            this.MacAddress = MacAddress;
            this.Port = Port;
            this.IpAddresss = IpAddress;
            this.ProductType = ProductType;

        }

        /// <summary>
        /// Child class will overwride this method and see what the message is and act apropriatly
        /// </summary>
        /// <param name="MessagePayload">The payload we have been sent from a lifx product</param>
        public virtual void ProcessMessage(PayloadBase MessagePayload)
        {

        }

        /// <summary>
        /// Gets called when we recieve an acknowledgent message from a lifx product.
        /// This is normaly because we just sent a set message of some kind
        /// The child class that inherits this class should overload this method
        /// </summary>
        public virtual void AcknowledgementReceived(Packets.LifxPacket lifxPacket)
        {

        }


        /// <summary>
        /// Passing true will turn the light on, passing false will turn the light off.
        /// </summary>
        /// <param name="OnOrOff">true turns light on, false turns light off</param>
        public void TurnLightOnOrOff(bool OnOrOff)
        {
            // if we want the bulb off, set to zero, if we want the bulb on, set to 65535
            UInt16 thePowerLevel = OnOrOff == true ? (UInt16)65535 : (UInt16)0;
            
            // create the packet we will be sending accross the networok
            MessageCreator messageCreator = new MessageCreator();
            LifxPacket packet = messageCreator.CreateMessage_SetPower(this.MacAddress, ++this.MessageCount, thePowerLevel);

            // raise an event to get the packet sent
            this.CallRaiseMessageEvent(this, packet);
        }


        public delegate void RaiseMessageDelegate(ProductBase product, LifxPacket lifxPacket);
        /// <summary>
        ///  Raise this event when you want to send a message to this product
        /// </summary>
        public event RaiseMessageDelegate RaiseMessage = delegate { };
        


        /// <summary>
        /// Allows child classes that inherit this class to call the <see cref="RaiseMessage"/> event
        /// Will only allow event to be raised if the previouse message was in the past bt more than <see cref="TimeToAllowBetweenSendingMessagesInMilliSeconds"/> milliseconds
        /// </summary>
        /// <param name="product"></param>
        /// <param name="lifxPacket"></param>
        /// <returns>true if allows event to be called, else false</returns>
        protected bool CallRaiseMessageEvent(ProductBase product, LifxPacket lifxPacket)
        {
            // check we are not trying to send a message too soon.
            if (DateTime.Now.Subtract(LastMessageSentAt).TotalMilliseconds < this.TimeToAllowBetweenSendingMessagesInMilliSeconds)
                return false;

            this.LastLifxPacketMessageThatWasSent = lifxPacket;
            this.RaiseMessage(product, lifxPacket);
            return true;
        }


        protected void OnPropertyChanged(object caller, string PropertyName)
        {
            PropertyChanged(caller, new PropertyChangedEventArgs(PropertyName));
        }
    }

    public enum ProductType
    {
        Lifx_A19 = 43,
        Unknown = 9999
    }
}
