using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets.Payload.Types
{
    public class StateLabelPayload : PayloadBase
    {
        protected override bool DecodePacket(PacketHeader header, byte[] data)
        {
            this.PayloadType = MessageType.StateLabel;

            int PayLoadSize = PayLoadTypes.StateLabel.Size;

            // check to see if we have the correct number of bytes
            if (data.Length != PacketHeader.PacketHeaderSize + PayLoadSize)
                return false;

            int PayloadStartPosition = PacketHeader.PacketHeaderSize;

            // will use this to create a byte array at the exact size needed for each property we are getting
            ByteConverter byteConverter = new ByteConverter();

            this.Label = BitConverter.ToString(byteConverter.Take(data, PayloadStartPosition, 32));

            return true;
        }


        public string Label { get; set; }
    }
}
