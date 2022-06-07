using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets.Payload.Types
{
    public class LightStatePayload : PayloadBase
    {
        /// <summary>
        /// Decodes a Light State message
        /// </summary>
        /// <param name="header">The decoded Header data</param>
        /// <param name="data">The header and payload byte data</param>
        protected override bool DecodePacket(PacketHeader header, byte[] data)
        {
            this.PayloadType = MessageType.LightState;

            // a LightState should contain 8 parts of information
            // hue (2 bytes)
            // saturation (2 bytes)
            // brightness (2 bytes)
            // kelvin (2 bytes)
            // reserved6 (2 bytes)
            // power (2 bytes)
            // label (32 bytes)
            // reserved7 (8 bytes)
            int PayLoadSize = PayLoadTypes.LightState.Size;

            // check to see if we have the correct number of bytes
            if (data.Length != PacketHeader.PacketHeaderSize + PayLoadSize)
                return false;



            // BitConverter.ToUInt16 is not working on the Data byte array (not sure why).
            // we are having to extract the exam number of bytes we need from the array
            // and then pass it into the BitConverter

            int PayloadStartPosition = PacketHeader.PacketHeaderSize;

            // will use this to create a byte array at the exact size needed for each property we are getting
            ByteConverter byteConverter = new ByteConverter();

            
            
            // get first 2 bytes of payload
            this.Hue = BitConverter.ToUInt16(byteConverter.Take(data, PayloadStartPosition, 2));
            this.Saturation = BitConverter.ToUInt16(byteConverter.Take(data, PayloadStartPosition + 2, 2));
            this.Brightness = BitConverter.ToUInt16(byteConverter.Take(data, PayloadStartPosition + 4, 2));
            this.Kelvin = BitConverter.ToUInt16(byteConverter.Take(data, PayloadStartPosition + 6, 2));
            this.ReservedOne = BitConverter.ToUInt16(byteConverter.Take(data, PayloadStartPosition + 8, 2));
            this.Power = BitConverter.ToUInt16(byteConverter.Take(data, PayloadStartPosition + 10, 2));
            this.Label = Encoding.ASCII.GetString(byteConverter.Take(data, PayloadStartPosition + 12, 32)).Trim('\0');
            this.ReservedTwo = BitConverter.ToUInt16(byteConverter.Take(data, PayloadStartPosition + 44, 2));
            

            // return true if sucsefull, else false
            return true;
        }


        public UInt16 Hue { get; private set; }
        public UInt16 Saturation { get; private set; }
        public UInt16 Brightness { get; private set; }
        public UInt16 Kelvin { get; private set; }
        public UInt16 ReservedOne { get; private set; }
        public UInt16 Power { get; private set; }
        public string Label { get; private set; }
        public UInt16 ReservedTwo { get; private set; }


    }
}
