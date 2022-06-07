using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Helpers;

namespace Core.Packets.Payload.Types
{
    public class PayloadBase
    {
        /// <summary>
        /// Helper class for converting values to byte array and byte array to binary
        /// </summary>
        protected ByteConverter _ByteConverter;

        #region public properties
        
        
        private string _EncodedPacketAsBinaryString;

        // payload data represented as a binary string
        public string EncodedPacketAsBinaryString
        {
            get => this._EncodedPacketAsBinaryString;
            protected set => this._EncodedPacketAsBinaryString = value;
        }

        /// <summary>
        /// Payload data represted as byte data
        /// </summary>
        public List<Byte> EncodedPacketsAsByte { get; set; } = new List<byte>();

        /// <summary>
        /// The type of payload we are dealilng with
        /// </summary>
        public MessageType PayloadType { get; protected set; }
        #endregion

        public PayloadBase()
        {
            this._ByteConverter = new ByteConverter();
        }



        

        public static PayloadBase Decode(PacketHeader header, byte[] data)
        {
            PayloadBase payload = new PayloadBase();
            
            switch((MessageType)header.PacketType)
            {
                case MessageType.StateService://StateService
                    payload = new StateServicePayload();
                    break;

                case MessageType.StateVersion:
                    payload = new StateVersionPayload();
                    break;

                case MessageType.LightState:
                    payload = new LightStatePayload();
                    break;
                    

                default:
                    return null;
            }

            payload.DecodePacket(header, data);
            return payload;
        }




        /// <summary>
        /// Child class that inherites this class should override this method
        /// </summary>
        /// <param name="header">A decoded header</param>
        /// <param name="data">Data data containing the header and payload data</param>
        /// <returns>true if sucsefull, else false</returns>
        protected virtual bool DecodePacket(PacketHeader header, byte[] data)
        {
            return true;
        }
    }
}
