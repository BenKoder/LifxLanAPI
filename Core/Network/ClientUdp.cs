using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.ComponentModel;

namespace Core.Network
{
    /// <summary>
    /// Sends and recieves packets of data over the local network to lifx products
    /// </summary>
    internal class ClientUdp
    {
        #region private variables
        /// <summary>
        /// Sends and recieves data
        /// </summary>
        private UdpClient _socket;
        /// <summary>
        /// The port we will listen on
        /// </summary>
        private int _Port = 56700;

        /// <summary>
        /// When set to true we will be listening for incoming data
        /// </summary>
        private bool _IsRunning;

        /// <summary>
        /// The IP address of the local computer used to connect to the internet
        /// e.g. 192.168.1.100
        /// </summary>
        private string _LocalComputerIpAddress;


        /// <summary>
        /// Will contain a list of ip addresses local to this computer.
        /// This will be usefull when sending broadcast messages (will let us ignore our own message we send out as a broadcast)
        /// </summary>
        private string[] _LocalAddressList;
        #endregion

        /// <summary>
        /// Used to fire event when data has been recieved on the network
        /// </summary>
        /// <param name="IpAddress">The ip address we recieved data from</param>
        /// <param name="Data">The byte data we recieved</param>
        public delegate void dRecievedData(string IpAddress, byte[] Data);
        /// <summary>
        /// Event that gets called when we recieve data from the network
        /// </summary>
        public event dRecievedData RecievedData = delegate { };



        public ClientUdp()
        {
            IPEndPoint end;
            // get local computers ip address
            // Not sure what would happen if computer is using IPv6 (we assuming its ip v4 because lifx uses ip v4)
            this._LocalComputerIpAddress = this.GetComputerLocalIpAddress();
            IPAddress localIpAddress;
            bool DidIpConvert = IPAddress.TryParse(this._LocalComputerIpAddress, out localIpAddress);

            // if we were able to get the local computers ip address, use it in the UDP Connection
            if(DidIpConvert == true)
                end = new IPEndPoint(localIpAddress,this._Port);
            else
            // we were not able to get local ip address, so just use all ip address on local computer
                end = new IPEndPoint(IPAddress.Any,this._Port);

            // set up the UDP socket
            this._socket = new UdpClient(end);
            this._socket.Client.Blocking = false;
            this._socket.DontFragment = true;
            this._socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            // create a string array of all local ip addresses on this computer.
            // this will allow us to see if when sending a broadcast message on the networok if
            // we have recieved a message back from our selfs, which we can then ignore
            this.CreateLocalIPAddressList();

  

        }


        /// <summary>
        /// Starts listening for data on the network
        /// </summary>
        /*public void Start()
        {
            
            // boolian flag that keeps getting checked to see if we should keep
            // listening for incoming data. once set to false, will stop listening for incoming data.
            _IsRunning = true;

            // We don't want to block the main thread so listen for incoming
            // data on a seperate thread
            Task.Run(async () =>
            {
                // while try keep listening for incoming data
                while (_IsRunning)
                    try
                    {
                        // try and recieve some data
                        var result = await _socket.ReceiveAsync();
                        // if we have recieved some data
                        if (result.Buffer.Length > 0)
                        {// pass the data off to antoher function to be proccesed
                            this.ProcessRecievedData(result.RemoteEndPoint.ToString(), result.Buffer);
                            
                        }

                    }
                    catch { }
            });

   
        }*/

        BackgroundWorker _BackgroundWorkerThread;
        /// <summary>
        /// Starts listening for data on the network
        /// </summary>
        public void Start()
        {
            // boolian flag that keeps getting checked to see if we should keep
            // listening for incoming data. once set to false, will stop listening for incoming data.
            _IsRunning = true;

            // we listen for data sent to us on another thread.
            // This means when we recieve data we are not on the UI Thread.
            // By using a BackgroundWorker we can raise an event that will allow
            // us to pass the data we recieve back to the UI Thread
            this._BackgroundWorkerThread = new BackgroundWorker();
            this._BackgroundWorkerThread.ProgressChanged += _BackgroundWorkerThread_ProgressChanged;
            this._BackgroundWorkerThread.RunWorkerCompleted += _BackgroundWorkerThread_RunWorkerCompleted;
            // make sure this is true so the progress changed event will be called
            this._BackgroundWorkerThread.WorkerReportsProgress = true;

            // We don't want to block the main thread so listen for incoming
            // data on a seperate thread
            this._BackgroundWorkerThread.DoWork += delegate (object sender, DoWorkEventArgs e)
            {
                // while try keep listening for incoming data
                while (_IsRunning)
                    try
                    {
                        
                        // the below  await _socket.ReceiveAsync(); will not work with a 
                        // BackgroundWorker because as soon as we recieve back from the await the 
                        // background worker completes and stops us calling its ProgressChanged event
                        //
                        // try and recieve some data
                        //var result = await _socket.ReceiveAsync();
                        
                        // if we have recieved some data
                        //if (result.Buffer.Length > 0)
                        //{// pass the data off to antoher function to be proccesed
                        //    this.ProcessRecievedData(result.RemoteEndPoint.ToString(), result.Buffer);
                            
                        //}

                        // check to see if we have any data coming in from the network.
                        // If there is no data, sleep the thread for 100 milliseconds
                        if (this._socket.Available <= 0)
                        {
                            System.Threading.Thread.Sleep(100);
                            continue;
                        }

                        // will need a blank ipEndPoint to pass in the the Recieve function
                        IPEndPoint? iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        // get the data that was sent from the network
                        byte[] result = this._socket.Receive(ref iPEndPoint);
                        
                        // make sure we recieved some data
                        if (result.Length > 0)
                        {
                            // pass the data off to antoher function to be proccesed
                            this.ProcessRecievedData(iPEndPoint.ToString(), result);

                        }
                        

                       


                    }
                    catch (Exception ex)
                    { }
            };

            this._BackgroundWorkerThread.RunWorkerAsync();

            

           

        }

        private void _BackgroundWorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int i = 0;
        }

        private void _BackgroundWorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DataHolder dataHolder = e.UserState as DataHolder;
            this.RecievedData(dataHolder.ip, dataHolder.data);
            
        }





        #region public methods
        /// <summary>
        /// Stops litening for data on the network.
        /// Will need to call start again if want to start listening again
        /// </summary>
        public void Stop()
        {
            this._IsRunning = false;
        }

        /// <summary>
        /// Dispose of the socket, class will not be usefull after this has been called
        /// </summary>
        public void Dispose()
        {
            this._IsRunning = false;
            this._socket.Dispose();
        }

        /// <summary>
        /// Sends passed in data to the specified ip address
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="IpAddress">IP address to send too</param>
        public void SendData(byte[] data, string IpAddress)
        {
            var d = this._socket.Client.AddressFamily;
            try
            {
                this._socket.SendAsync(data, data.Length, IpAddress, this._Port);
            }
            catch
            { }
        }
        #endregion


        #region private methods
        /// <summary>
        /// Rases an even to anyone listening to say data has been recieved
        /// </summary>
        /// <param name="IpAddressDataCameFrom">IP address we recieved the data from (should be a lifx product)</param>
        /// <param name="RecievedData">Data recieved from a lifx product</param>
        private void ProcessRecievedData(string IpAddressDataCameFrom, byte[] RecievedData)
        {
            // only pass on the recieved message if it is from another computer on the network (not this computer)
            if (IsLocalComputerIpAddress(IpAddressDataCameFrom) == false)
            {
                // create an object to hold the ip and byte data in
                DataHolder dataHolder = new DataHolder();
                dataHolder.ip = IpAddressDataCameFrom;
                dataHolder.data = RecievedData;

                // raise the _BackgroundWorkerThread_ProgressChanged event passing in the dataHolder object
                // This should raise then event on the main thread
                this._BackgroundWorkerThread.ReportProgress(1, dataHolder);
                //this.RecievedData(IpAddressDataCameFrom, RecievedData);
            }
        }

   


        private void CreateLocalIPAddressList()
        {
            // get the local computers host name
            string HostName = System.Net.Dns.GetHostName();

            // get all the ip addresses assinged to this local computer
            System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(HostName);

            // create the array length for the string that will hold all the ip addresses
            this._LocalAddressList = new string[hostEntry.AddressList.Length];
            // go through each ip address in the host entry and add it to the _LocalAddressList
            for (int eachAddressIndex = 0; eachAddressIndex < hostEntry.AddressList.Length; eachAddressIndex++)
            {
                this._LocalAddressList[eachAddressIndex] = hostEntry.AddressList[eachAddressIndex].ToString();

                if (hostEntry.AddressList[eachAddressIndex].AddressFamily == AddressFamily.InterNetwork)
                {
                    string local = hostEntry.AddressList[eachAddressIndex].ToString();
                    IPAddress a = System.Net.IPAddress.Broadcast;

                    


                }
            }



        }

        /// <summary>
        /// Gets the local IP Address the compuer uses to connect to the internet
        /// </summary>
        /// <returns></returns>
        private string GetComputerLocalIpAddress()
        {
            // open a socket to googles dns. This will allow us to get the local ip address that was
            // used to make the connection.

            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }

            return localIP;
        }

        /// <summary>
        /// Checks the passed in ip address to see if it is an ip address that belongs to this computer
        /// </summary>
        /// <param name="IpAddress">IP address to check</param>
        /// <returns>True if Passed in IP belongs to this computer</returns>
        private bool IsLocalComputerIpAddress(string IpAddress)
        {
            return this._LocalAddressList.Any(IpAddress.Contains);
        }
        #endregion
    }

    internal class DataHolder
    {
        public string ip;
        public byte[] data;
    }
}
