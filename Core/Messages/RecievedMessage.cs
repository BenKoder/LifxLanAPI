using Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Messages
{
    public class RecievedMessage
    {

        public LifxPacket DecodeMessage(byte[] Data)
        {
            LifxPacket lifxPacket = new Packets.LifxPacket();


            lifxPacket.HeaderPacket = Packets.PacketHeader.DecodePacketHeader(Data);
            // if we were not able to get the header from the data sent, then there
            // is nothing we can do so just ignore this data we recieved.
            if (lifxPacket.HeaderPacket == null)
                return null;

            // Decode the payload data sent with it (if there was any sent with it)
            lifxPacket.PayloadPacket = Packets.Payload.Types.PayloadBase.Decode(lifxPacket.HeaderPacket, Data);

            return lifxPacket;
        }
    }
}
