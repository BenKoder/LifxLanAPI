using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    /// <summary>
    /// Converts values to byte array
    /// </summary>
    public class ByteConverter
    {

        /// <summary>
        /// Converts a <see cref="Int16"/> value to a LittleEndian Binary Value as a string
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns>Binary data in LittleEndian as string</returns>
        public string ConvertInt16ToBinaryString(Int16 value)
        {
            // the byte array of the converted value
            byte[] Packets;

            Packets = this.ConvertValueToBytes(value);
            // return the binary string
            return this.ConvertByteArrayToBinaryString(Packets);
        }
        /// <summary>
        /// Converts a <see cref="Int16"/> value to a LittleEndian Binary Value as a string
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <param name="Packets">value converted to byte array</param>
        /// <returns>Binary data in LittleEndian as string</returns>
        public string ConvertInt16ToBinaryString(Int16 value, out byte[] Packets)
        {
            // the byte array of the converted value
            //byte[] Packets;

            Packets = this.ConvertValueToBytes(value);
            // return the binary string
            return this.ConvertByteArrayToBinaryString(Packets);
        }

        /// <summary>
        /// Converts a Int16 value to a byte array in LittleEndian format
        /// </summary>
        /// <param name="value">the value to convert</param>
        /// <returns>Byte array in Little Endian</returns>
        public byte[] ConvertValueToBytes(Int16 value)
        {
            // the byte array of the converted value
            byte[] Packets;
            Packets = BitConverter.GetBytes(value);
            // if the CPU we are on is using BigEndian we need to reverse the array because lifx uses little endian
            Packets = this.ConvertByteArrayToLittleEndianIfNecessary(Packets);
            return Packets;
        }
        /// <summary>
        /// Converts a Int16 value to a byte array in LittleEndian format
        /// </summary>
        /// <param name="value">the value to convert</param>
        /// <returns>Byte array in Little Endian</returns>
        public byte[] ConvertValueToBytes(uint value)
        {
            // the byte array of the converted value
            byte[] Packets;
            Packets = BitConverter.GetBytes(value);
            // if the CPU we are on is using BigEndian we need to reverse the array because lifx uses little endian
            Packets = this.ConvertByteArrayToLittleEndianIfNecessary(Packets);
            return Packets;
        }
        /// <summary>
        /// Converts an Int64 value to a byte array in LittleEndian format
        /// </summary>
        /// <param name="value"> the value to convert</param>
        /// <returns>Byte array in Little Endian</returns>
        public byte[] ConvertValueToBytes(Int64 value)
        {
            // the byte array of the converted value
            byte[] Packets;
            Packets = BitConverter.GetBytes(value);
            // if the CPU we are on is using BigEndian we need to reverse the array because lifx uses little endian
            Packets = this.ConvertByteArrayToLittleEndianIfNecessary(Packets);
            return Packets;
        }
        /// <summary>
        /// Converts a ushort to a byte array in LittleEndian format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] ConvertValueToBytes(ushort value)
        {
            // the byte array of the converted value
            byte[] Packets;
            Packets = BitConverter.GetBytes(value);
            // if the CPU we are on is using BigEndian we need to reverse the array because lifx uses little endian
            Packets = this.ConvertByteArrayToLittleEndianIfNecessary(Packets);
            return Packets;
        }
        /// <summary>
        /// Converts a Int32 to a byte array in LittleEndian format
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] ConvertValueToBytes(Int32 value)
        {
            // the byte array of the converted value
            byte[] Packets;
            Packets = BitConverter.GetBytes(value);
            // if the CPU we are on is using BigEndian we need to reverse the array because lifx uses little endian
            Packets = this.ConvertByteArrayToLittleEndianIfNecessary(Packets);
            return Packets;
        }

        /// <summary>
        /// Converts byte array to binary string 
        /// </summary>
        /// <param name="ByteData">Data to convert</param>
        /// <returns>Binary numbers as a string</returns>
        public string ConvertByteArrayToBinaryString(byte[] ByteData)
        {
            // will hold the binary data as a string
            StringBuilder sb = new StringBuilder();


            // go through each byte and convert it to a binary string using the ByteHelper class
            foreach (byte aPacket in ByteData)
                sb.Append(ByteModifier.ToString(aPacket));

            // return the binary string
            return sb.ToString();
        }

        /// <summary>
        /// Takes a subset of the passed in data
        /// </summary>
        /// <param name="Data">Data to look at and take a subset of</param>
        /// <param name="StartIndex">Start index to extract</param>
        /// <param name="Length">Length from start index to take</param>
        /// <returns>subset of bytes from passed in Data</returns>
        public byte[] Take(byte[] Data, int StartIndex, int Length)
        {
            byte[] byteData = new byte[Length];

            int countIndex = 0;
            for(int i = StartIndex; i < (StartIndex + Length); i++)
            {
                byteData[countIndex++] = Data[i];
            }

            return byteData;
        }




        /// <summary>
        /// Call this method just after calling BitConverter.GetBytes. This will ensure the Generated byte data is in
        /// LittleEndian
        /// </summary>
        /// <param name="ByteData">ByteData that just got converted from BitConverter.GetBytes()</param>
        /// <returns></returns>
        public byte[] ConvertByteArrayToLittleEndianIfNecessary(byte[] ByteData)
        {
            // if BitConvert is set to BigEndian, we need to convert the byte data to Little Endian
            if (BitConverter.IsLittleEndian == false)
                Array.Reverse(ByteData);

            return ByteData;

        }
    }
}
