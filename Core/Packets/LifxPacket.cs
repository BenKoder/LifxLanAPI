using Core.Packets.Payload.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets
{
    /// <summary>
    /// A lifex packet is made up of the header and the payload
    /// </summary>
    public class LifxPacket
    {
        public PacketHeader HeaderPacket { get; set; } = null;
        public PayloadBase PayloadPacket { get; set; } = null;

        public byte[] CombinePacketsIntoOne()
        {
            List<byte> combinedBytes = new List<byte>();
            if(this.HeaderPacket != null)
                combinedBytes.AddRange(HeaderPacket.EncodedPacketsAsByte);
            // not all messages we sent to a client will have a payload (some get messages don't have any payload)
            if(this.PayloadPacket != null)
                combinedBytes.AddRange(PayloadPacket.EncodedPacketsAsByte);

            return combinedBytes.ToArray();
        }

    }
}
