using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets.Payload.Types
{
    public class StateServicePayload : PayloadBase
    {

        /// <summary>
        /// Decodes a State Service message
        /// </summary>
        /// <param name="header">The decoded Header data</param>
        /// <param name="data">The header and payload byte data</param>
        protected override bool DecodePacket(PacketHeader header, byte[] data)
        {
            this.PayloadType = MessageType.StateService;

            // a StageSerice should contain to bit of information
            // Service (1 byte)
            // Port (4 bytes)
            int PayLoadSize = PayLoadTypes.StateService.Size;

            // check to see if we have the correct number of bytes
            if (data.Length != PacketHeader.PacketHeaderSize + PayLoadSize)
                return false;

            // get the fifth byte from the end
            this.Service = data[^5];
            this.Port = BitConverter.ToInt32(data, PacketHeader.PacketHeaderSize + 1);

            // return true if sucsefull, else false
            return true;
        }

        public int Service { get; private set; }
        public int Port { get; private set; }
    }
}
