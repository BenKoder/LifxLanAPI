using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets.Payload.Types
{
    public class SetPowerPayload : PayloadBase
    {
        /// <summary>
        /// If you specify 0 the light will turn off and if you specify 65535 the device will turn on.
        /// </summary>
        public UInt16 Level { get; set; }

        /// <summary>
        /// Convert the packet to a binary string and byte array.
        /// </summary>
        /// <param name="PowerLevel">value between 0 (off) and 65535 (on)</param>
        /// <returns>Binary string</returns>
        public string EncodePacket(UInt16 PowerLevel)
        {
            this.PayloadType = MessageType.SetPower;

            Level = PowerLevel;

            return this.EncodePacketToBinary();
        }

        /// <summary>
        /// Converts the SetPower value to a byte array and binary string
        /// </summary>
        /// <returns>binary string</returns>
        private string EncodePacketToBinary()
        {
            this.EncodedPacketsAsByte.Clear();
            this.EncodedPacketAsBinaryString = String.Empty;

            StringBuilder sb = new StringBuilder();

            // Convert the value to a byte array
            byte[] byteData = this._ByteConverter.ConvertValueToBytes(this.Level);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(byteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(byteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return sb.ToString();
        }
    }
}
