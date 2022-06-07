using Core.Helpers;
using Core.Packets.Payload;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets
{
    public class PacketHeader
    {
        #region private variables
        private static Int16 _PacketHeaderSize = 36;
        /// <summary>
        /// Helper class for converting values to byte array and byte array to binary
        /// </summary>
        private ByteConverter _ByteConverter;
        #endregion

        public PacketHeader()
        {
            this._ByteConverter = new ByteConverter();
        }

        #region public properties
        /// <summary>
        /// Size of the total Packet, header + payload
        /// </summary>
        public Int16 SizeOfPacket { get; set; }
        /// <summary>
        ///  Should be set to 1024
        /// </summary>
        public Int16 Protocol { get; set; }
        /// <summary>
        /// Must be set to true
        /// </summary>
        public bool Addressable { get; set; }
        /// <summary>
        /// If broadcasting to all lights, set to true, else if sending to a single light, set to false
        /// </summary>
        public bool Tagged { get; set; }
        /// <summary>
        /// Set to zero
        /// </summary>
        public sbyte Origin { get; set; }
        /// <summary>
        /// Set to Two when sending out messages to lights
        /// </summary>
        public Int32 Source { get; set; }
        /// <summary>
        /// Device ID we want to send message to e.g. d073d5001337
        /// </summary>
        public string Target { get; set; } = string.Empty;
        /// <summary>
        /// This should be a six byte array (Set to zeros)
        /// </summary>
        public Byte[] ReservedOne { get; set; }

        /// <summary>
        /// Should be get a reply? 
        /// (at present this should always be set to false even though when false we will still get a replay)
        /// </summary>
        public bool ResRequired { get; set; }
        /// <summary>
        /// Documentation says to set this to true
        /// </summary>
        public bool AckRequired { get; set; }

        /// <summary>
        /// 6 bits that are reserved (will all be set to zero when sending a message)
        /// </summary>
        public BitArray ReservedTwo { get; set; }

        /// <summary>
        /// should be a number between 1 and 255. Should be incremented by one each time message is set out to light.
        /// Once reaches 255 should be set to zero to start incrementing again.
        /// </summary>
        public Byte Sequence { get; set; }
        /// <summary>
        /// Set tozero
        /// </summary>
        public Int64 ReservedThree { get; set; }
        /// <summary>
        /// The packet type being sent. e.g. SetColor would be 102
        /// </summary>
        public Int16 PacketType { get; set; }
        /// <summary>
        /// Set to zero
        /// </summary>
        public Int16 ReservedFour { get; set; }

        private string _EncodedPacketAsBinaryString;
        public string EncodedPacketAsBinaryString
        {
            get => this._EncodedPacketAsBinaryString;
            private set => this._EncodedPacketAsBinaryString = value;
        }
        public List<Byte> EncodedPacketsAsByte { get; set; } = new List<byte>();


        #endregion

        #region public static properties
        /// <summary>
        /// Gets the Size a header packet should be
        /// </summary>
        public static Int16 PacketHeaderSize { get => PacketHeader._PacketHeaderSize; }

        /// <summary>
        /// The type of packet we are dealilng with
        /// </summary>
        public MessageType PayloadType { get; private set; }
        #endregion

        #region public methods
        /// <summary>
        /// Creates a header packet and returns a string binary version of the header
        /// </summary>
        /// <param name="IsBroadcastToMultipleLights">Set to true if message should be set to all lights on the network</param>
        /// <param name="PayLoadTypeToSend">The type of message we want to send. e.g. SetColor</param>
        /// <param name="IdOfLightSendingTo">If NOT broadcasting (sending to a specific light) the id of the light e.g. d073d5001337 </param>
        /// <param name="MessageCount">Message number we are sending. should be incremented each time a new message is sent and a number from 1 to 255. set back to 1 when hits 255</param>
        /// <returns>Header data as a binary string</returns>
        public string EncodePacket(bool IsBroadcastToMultipleLights, Payload.PayLoadType PayLoadTypeToSend, string IdOfLightSendingTo, int MessageCount)
        {
            this._EncodedPacketAsBinaryString = string.Empty;
            this.EncodedPacketsAsByte.Clear();

            // calculate the size of the packet (header plus payload)
            this.SizeOfPacket = (Int16)(PacketHeader._PacketHeaderSize + PayLoadTypeToSend.Size);
            // will always be 1024 unless lifx decide to change it
            this.Protocol = 1024;
            // must be set to true
            this.Addressable = true;
            // set to true if broadcasting to all lights. if sending to one light, set to false
            this.Tagged = IsBroadcastToMultipleLights;
            // must be set to zero
            this.Origin = 0;
            // must be set to 2
            this.Source = 2;
            // if we are broadcasting to more than one light, this will be string.empty (which gets converted to zeros in binary)
            // if not broadcasting, gets set to the ID of the light we want to send to which looks somthing like "d073d5001337"
            this.Target = IdOfLightSendingTo;
            // must be set to zero
            // create six bytes
            this.ReservedOne = new byte[6];
            // set all bytes to zero
            for(int byteIndex = 0; byteIndex < this.ReservedOne.Length; byteIndex++)
                this.ReservedOne[byteIndex] = 0;
            
            // this is always set to false at present
            this.ResRequired = false;
            // this is always set to true at present
            this.AckRequired = true;
            // 6 bits that are all set to zero
            this.ReservedTwo = new BitArray(new bool[] { false, false, false, false, false, false });
            // the current message count we are on. Each time we send a message one gets added to Sequence until we 
            // reach 255, at which point sequence starts back at 1
            if (MessageCount < 0 || MessageCount > 255)
                throw new ArgumentOutOfRangeException(nameof(MessageCount), "value must be from 0 to 255");
            this.Sequence = Convert.ToByte(MessageCount);
            // should be set to zero
            this.ReservedThree = 0;
            // each packet type has a enique number to tell the light what we are trying to do on the light
            this.PacketType = PayLoadTypeToSend.Identifier;
            // same as packetType but as an enum
            this.PayloadType = PayLoadTypeToSend.MesssageType;
            // should be set to zero
            this.ReservedFour = 0;

            return this.EncodeHeaderToBinary();
        }

        public string EncodeGetPacket(bool IsBroadcastToMultipleLights, Payload.PayLoadType PayLoadTypeToSend, string IdOfLightSendingTo, int MessageCount)
        {
            this._EncodedPacketAsBinaryString = string.Empty;
            this.EncodedPacketsAsByte.Clear();

            // calculate the size of the packet (header plus payload)
            this.SizeOfPacket = (Int16)(PacketHeader._PacketHeaderSize + PayLoadTypeToSend.Size);
            // will always be 1024 unless lifx decide to change it
            this.Protocol = 1024;
            // must be set to true
            this.Addressable = true;
            // set to true if broadcasting to all lights. if sending to one light, set to false
            this.Tagged = IsBroadcastToMultipleLights;
            // must be set to zero
            this.Origin = 0;
            // must be set to 2
            this.Source = 2;
            // if we are broadcasting this will be string.empty (which gets converted to zeros in binary)
            // if not broadcasting, gets set to the ID of the light we want to send to which looks somthing like "d073d5001337"
            this.Target = IdOfLightSendingTo;
            // must be set to zero
            // create six bytes
            this.ReservedOne = new byte[6];
            // set all bytes to zero
            for (int byteIndex = 0; byteIndex < this.ReservedOne.Length; byteIndex++)
                this.ReservedOne[byteIndex] = 0;

            // when sending get message, we set this to false
            this.ResRequired = false;
            // when sending get messages, we set this to false
            this.AckRequired = false;
            // 6 bits that are all set to zero
            this.ReservedTwo = new BitArray(new bool[] { false, false, false, false, false, false });
            // the current message count we are on. Each time we send a message one gets added to Sequence until we 
            // reach 255, at which point sequence starts back at 1
            if (MessageCount < 0 || MessageCount > 255)
                throw new ArgumentOutOfRangeException(nameof(MessageCount), "value must be from 0 to 255");
            this.Sequence = Convert.ToByte(MessageCount);
            // should be set to zero
            this.ReservedThree = 0;
            // each packet type has a enique number to tell the light what we are trying to do on the light
            this.PacketType = PayLoadTypeToSend.Identifier;
            // same as packetType but as an enum
            this.PayloadType = PayLoadTypeToSend.MesssageType;
            // should be set to zero
            this.ReservedFour = 0;
            
            return this.EncodeHeaderToBinary();
        }
        public string EncodeSetPacket(bool IsBroadcastToMultipleLights, Payload.PayLoadType PayLoadTypeToSend, string IdOfLightSendingTo, int MessageCount)
        {
            this._EncodedPacketAsBinaryString = string.Empty;
            this.EncodedPacketsAsByte.Clear();

            // calculate the size of the packet (header plus payload)
            this.SizeOfPacket = (Int16)(PacketHeader._PacketHeaderSize + PayLoadTypeToSend.Size);
            // will always be 1024 unless lifx decide to change it
            this.Protocol = 1024;
            // must be set to true
            this.Addressable = true;
            // set to true if broadcasting to all lights. if sending to one light, set to false
            this.Tagged = IsBroadcastToMultipleLights;
            // must be set to zero
            this.Origin = 0;
            // must be set to 2
            this.Source = 2;
            // if we are broadcasting this will be string.empty (which gets converted to zeros in binary)
            // if not broadcasting, gets set to the ID of the light we want to send to which looks somthing like "d073d5001337"
            this.Target = IdOfLightSendingTo;
            // must be set to zero
            // create six bytes
            this.ReservedOne = new byte[6];
            // set all bytes to zero
            for (int byteIndex = 0; byteIndex < this.ReservedOne.Length; byteIndex++)
                this.ReservedOne[byteIndex] = 0;

            // when sending get message, we set this to false
            this.ResRequired = false;
            // when sending get messages, we set this to false
            this.AckRequired = true;
            // 6 bits that are all set to zero
            this.ReservedTwo = new BitArray(new bool[] { false, false, false, false, false, false });
            // the current message count we are on. Each time we send a message one gets added to Sequence until we 
            // reach 255, at which point sequence starts back at 1
            if (MessageCount < 0 || MessageCount > 255)
                throw new ArgumentOutOfRangeException(nameof(MessageCount), "value must be from 0 to 255");
            this.Sequence = Convert.ToByte(MessageCount);
            // should be set to zero
            this.ReservedThree = 0;
            // each packet type has a enique number to tell the light what we are trying to do on the light
            this.PacketType = PayLoadTypeToSend.Identifier;
            // same as packetType but as an enum
            this.PayloadType = PayLoadTypeToSend.MesssageType;
            // should be set to zero
            this.ReservedFour = 0;

            return this.EncodeHeaderToBinary();
        }

        public string EncodeSetPacket(bool IsBroadcastToMultipleLights, bool ResponseRequired, bool AcknowledgementRequired, Payload.PayLoadType PayLoadTypeToSend, string IdOfLightSendingTo, int MessageCount)
        {
            this._EncodedPacketAsBinaryString = string.Empty;
            this.EncodedPacketsAsByte.Clear();

            // calculate the size of the packet (header plus payload)
            this.SizeOfPacket = (Int16)(PacketHeader._PacketHeaderSize + PayLoadTypeToSend.Size);
            // will always be 1024 unless lifx decide to change it
            this.Protocol = 1024;
            // must be set to true
            this.Addressable = true;
            // set to true if broadcasting to all lights. if sending to one light, set to false
            this.Tagged = IsBroadcastToMultipleLights;
            // must be set to zero
            this.Origin = 0;
            // must be set to 2
            this.Source = 2;
            // if we are broadcasting this will be string.empty (which gets converted to zeros in binary)
            // if not broadcasting, gets set to the ID of the light we want to send to which looks somthing like "d073d5001337"
            this.Target = IdOfLightSendingTo;
            // must be set to zero
            // create six bytes
            this.ReservedOne = new byte[6];
            // set all bytes to zero
            for (int byteIndex = 0; byteIndex < this.ReservedOne.Length; byteIndex++)
                this.ReservedOne[byteIndex] = 0;

            // when sending get message, we set this to false
            this.ResRequired = ResponseRequired;
            // when sending get messages, we set this to false
            this.AckRequired = AcknowledgementRequired;
            // 6 bits that are all set to zero
            this.ReservedTwo = new BitArray(new bool[] { false, false, false, false, false, false });
            // the current message count we are on. Each time we send a message one gets added to Sequence until we 
            // reach 255, at which point sequence starts back at 1
            if (MessageCount < 0 || MessageCount > 255)
                throw new ArgumentOutOfRangeException(nameof(MessageCount), "value must be from 0 to 255");
            this.Sequence = Convert.ToByte(MessageCount);
            // should be set to zero
            this.ReservedThree = 0;
            // each packet type has a enique number to tell the light what we are trying to do on the light
            this.PacketType = PayLoadTypeToSend.Identifier;
            // same as packetType but as an enum
            this.PayloadType = PayLoadTypeToSend.MesssageType;
            // should be set to zero
            this.ReservedFour = 0;

            return this.EncodeHeaderToBinary();
        }

        /// <summary>
        /// Decodes the Packet header part of the data we recieved. (does not decode the payload, if there is any)
        /// </summary>
        /// <param name="PacketData">the data sent from a lifx product</param>
        /// <returns>Decoded packet header or null if unable to decode</returns>
        public static PacketHeader DecodePacketHeader(byte[] PacketData)
        {
            PacketHeader header = new PacketHeader();

            // the packet data can't be less than 36 bytes because the header part of the packet data is 36 bytes
            if (PacketData.Length < PacketHeader._PacketHeaderSize)
                return null;

            try
            {
                PacketHeader.DecodeSize(PacketData, header);
                PacketHeader.DecodeProtocal(PacketData, header);
                PacketHeader.DecodeAddressable(PacketData, header);
                PacketHeader.DecodeTagged(PacketData, header);
                PacketHeader.DecodeOrigin(PacketData, header);
                PacketHeader.DecodeSource(PacketData, header);
                PacketHeader.DecodeTarget(PacketData, header);
                PacketHeader.DecodeReservedOne(PacketData, header);
                PacketHeader.DecodeResRequired(PacketData, header);
                PacketHeader.DecodeAckRequired(PacketData, header);
                PacketHeader.DecodeReservedTwo(PacketData, header);
                PacketHeader.DecodeSequence(PacketData, header);
                PacketHeader.DecodeReservedThree(PacketData, header);
                PacketHeader.DecodePacketType(PacketData, header);
                PacketHeader.DecodeReservedFour(PacketData, header);
            }
            catch (Exception ex)
            {
                return null;
            }

            return header;

        }

  


        #endregion


        #region private methods

        #region Encode packets
        /// <summary>
        /// Converts the public header properties into binary and returns the binary as a string
        /// </summary>
        /// <returns>the binary value of the headers properties</returns>
        private string EncodeHeaderToBinary()
        { 
            // will hold the binary data as a string
            StringBuilder sb = new StringBuilder();

            sb.Append(this.ConvertSizeOfPacketToBinary());

            sb.Append(this.ConvertProtocolAddressableTaggedAndOriginToBinary());

            sb.Append(this.ConvertSourceToBinary());

            sb.Append(this.ConvertTargetToBinary());

            sb.Append(this.ConvertReservedOneToBinary());

            sb.Append(this.ConvertResRequiredAckRequiredAndReservedTwoToBinary());

            sb.Append(this.ConvertSequenceToBinary());

            sb.Append(this.ConvertReservedThreeToBinary());

            sb.Append(this.ConvertPacketTypeToBinary());

            sb.Append(this.ConvertReservedFourToBinary());

            return sb.ToString();
        }



        #region converting properties to binary 
        /// <summary>
        /// Converts the <see cref="SizeOfPacket"/> to a binary string
        /// </summary>
        /// <returns>Binary data as string</returns>
        private string ConvertSizeOfPacketToBinary()
        {
            // the converted value as a byte array
            byte[] bytes;
            // converts the SizeOfPacket to binary string
            string valueAsBinaryString =  this._ByteConverter.ConvertInt16ToBinaryString(this.SizeOfPacket, out bytes);
            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(bytes);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            // return the binary string
            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="Protocol"/>, <see cref="Addressable"/>, <see cref="Tagged"/> & <see cref="Origin"/> to a binary string
        /// </summary>
        /// <returns>Binary data as string</returns>
        private string ConvertProtocolAddressableTaggedAndOriginToBinary()
        {
            byte[] bytes;
            int IndexPositionInByteArray;
            int IndexPositionInByte;

            // This will give us 16 bits (16 zeroes or ones when converted to binary).
            // However we know from the docs we will only use of the first 12 bits.
            // The last 4 bits will be used below to store information for other things
            bytes = this._ByteConverter.ConvertValueToBytes(this.Protocol);


            



            ///////////////////////////////////
            // Set the this.Addressable value

            // which index of the address should Addressable be in
            IndexPositionInByteArray = (int)(13  / 8); 
            // which index position within the individual byte does Addressable need to be set
            IndexPositionInByte = ((13 - 1) % 8) + 1;

            // 13th Bit will store the addressable value
            ByteModifier.SetBit(ref bytes[IndexPositionInByteArray], IndexPositionInByte, this.Addressable);




            ////////////////////////////
            // set the this.Tagged value

            // which index of the address should Tagged be in
            IndexPositionInByteArray = (int)14 / 8;
            // which index position within the individual byte does Tagged need to be set
            IndexPositionInByte = ((14 - 1) % 8) + 1;
            // 13th Bit will store the addressable value
            ByteModifier.SetBit(ref bytes[IndexPositionInByteArray], IndexPositionInByte, this.Tagged);



            ////////////////////////////
            // set the this.Origin value

            // this.Origin is an sByte so is only 1 byte big but for some reason we get 2 bytes returned
            // (we are only interested in the first byte in the byte array).
            // we are only interested in the first 2 bits of the byte which should be set to zero anyway
            byte[] OriginByteArray = this._ByteConverter.ConvertValueToBytes(this.Origin);

            // which index of the address should Tagged be in
            IndexPositionInByteArray = (int)15 / 8;
            
            IndexPositionInByte = ((15 - 1) % 8) + 1;
            if(ByteModifier.IsBitSet(OriginByteArray[0], 0) == true)
                ByteModifier.SetBit(ref bytes[IndexPositionInByteArray], IndexPositionInByte, true);
            else
                ByteModifier.SetBit(ref bytes[IndexPositionInByteArray], IndexPositionInByte, false);

            IndexPositionInByte = ((16 - 1) % 8) + 1;
            if (ByteModifier.IsBitSet(OriginByteArray[0], 0) == true)
                ByteModifier.SetBit(ref bytes[IndexPositionInByteArray], IndexPositionInByte, true);
            else
                ByteModifier.SetBit(ref bytes[IndexPositionInByteArray], IndexPositionInByte, false);

            


            // converts the Protocal to binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(bytes);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(bytes);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;

        }


        /// <summary>
        /// Converts the <see cref="Source"/> to a binary string
        /// </summary>
        /// <returns>Binary data as string</returns>
        private string ConvertSourceToBinary()
        {
            byte[] ByteData = this._ByteConverter.ConvertValueToBytes(this.Source);
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(ByteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(ByteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="Target"/> (Device ID in Hex) to Binary string
        /// </summary>
        /// <returns><see cref="Target"/> as binary data as string</returns>
        private string ConvertTargetToBinary()
        {
            // Holds the binary information we want to return (the target as a binary string)
            StringBuilder sb = new StringBuilder();
            // target is made up of 6 bytes but the api says we need api (the last 2 bytes are zeros)
            byte[] ByteData = new byte[8];
            // Target is made up of Hexadecimal (every 2 chars is a hexedecimal)
            // go through each one and convert it to base 10
            for (int i = 0; i < this.Target.Length; i = i + 2)
            {
                // get the hex value as a string (we have to add 0x to the start of the string
                // so c# knows we are looking at hex
                string hexString = "0x" + this.Target.Substring(i, 2);
                // convert the hex value to base 10 and store it as an 8 bit
                byte base10NumberIn8Bit = Convert.ToByte(hexString, 16);

                // keep track of all the bytes we have converted from hex to base 10
                ByteData[i / 2] = base10NumberIn8Bit;

                // convert the byte into a binary string
                string binaryString = this._ByteConverter.ConvertByteArrayToBinaryString(new byte[] {base10NumberIn8Bit});
                // add the binary string to the string builder
                sb.Append(binaryString);
            }

            // there are 2 bytes (16 bits) that need to be added to the end of target.
            // These are all set to zero (probs extra space for furter use)
            // set the last 2 bytes to zero
            ByteData[6] = 0;
            ByteData[7] = 0;
            // create the last 2 bytes as a binary string (all zeroes)
            sb.Append(this._ByteConverter.ConvertByteArrayToBinaryString(new byte[] { ByteData[6], ByteData[7] }));

            // return the Target value as a binary string
            string valueAsBinaryString = sb.ToString();

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(ByteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="ReservedOne"/> to a binary string
        /// </summary>
        /// <returns><see cref="ReservedOne"/> as binary data as string</returns>
        private string ConvertReservedOneToBinary()
        {

            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(this.ReservedOne);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(this.ReservedOne);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            // return the binary string
            return valueAsBinaryString;
        }

        

        /// <summary>
        /// Converts <see cref="ResRequired"/>, <see cref="AckRequired"/> & <see cref="ReservedTwo"/> to 
        /// an 8 bit binary string
        /// </summary>
        /// <returns>8 bit binary string</returns>
        private string ConvertResRequiredAckRequiredAndReservedTwoToBinary()
        {
            byte aByte = new byte();

            // add the resRequired and AckRequired to the byte in positions 1 and 2
            ByteModifier.SetBit(ref aByte, 1, this.ResRequired);
            ByteModifier.SetBit(ref aByte, 2, this.AckRequired);

            // for the remaining 6 bits, set them to the value of ReservedTwo (this should be a 6 bit array)
            for(int i = 0; i < this.ReservedTwo.Length; i++)
            {
                ByteModifier.SetBit(ref aByte, i + 3, this.ReservedTwo[i]);
            }

            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(new byte[] { aByte });

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.Add(aByte);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            // return the binary string
            return valueAsBinaryString;

        }


        /// <summary>
        /// Converts the <see cref="Sequence"/> to a binary string
        /// </summary>
        /// <returns>8 bit binary string</returns>
        private string ConvertSequenceToBinary()
        {
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(new byte[] { this.Sequence });

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.Add(this.Sequence);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            // return the binary string
            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="ReservedThree"/> to a binary string
        /// </summary>
        /// <returns>64 bit binary string</returns>
        private string ConvertReservedThreeToBinary()
        {
            // convert the Int64 to a byte array
            byte[] ByteData = this._ByteConverter.ConvertValueToBytes(this.ReservedThree);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(ByteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(ByteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            // return the binary string
            return valueAsBinaryString;
        }

        /// <summary>
        /// Convert the <see cref="PacketType"/> to a binary string
        /// </summary>
        /// <returns>16 bit binary string</returns>
        private string ConvertPacketTypeToBinary()
        {
            // convert the Int16 to a byte array
            byte[] ByteData = this._ByteConverter.ConvertValueToBytes(this.PacketType);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(ByteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(ByteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            // return binary string
            return valueAsBinaryString;
        }


        /// <summary>
        /// Converts the <see cref="ReservedFour"/> to a binary string
        /// </summary>
        /// <returns>16 bit binary string</returns>
        private string ConvertReservedFourToBinary()
        {
            // convert the Int16 to a byte array
            byte[] ByteData = this._ByteConverter.ConvertValueToBytes(this.ReservedFour);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(ByteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(ByteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            // return binary string
            return valueAsBinaryString;
        }


        #endregion

        #endregion


        #endregion



        #region private static methods

        #region Decode packets

        /// <summary>
        /// Gets <see cref="SizeOfPacket"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeSize(byte[] ByteData, PacketHeader packetHeader)
        {
            // size is found in the first 2 bytes
            packetHeader.SizeOfPacket = BitConverter.ToInt16(ByteData, 0);

            return;
        }


        /// <summary>
        /// Gets <see cref="Protocol"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeProtocal(byte[] ByteData, PacketHeader packetHeader)
        {
            // size is stored in the positions ByteData[2] & ByteData[3]
            // However it only uses the first 12 bits.

            // The last 4 bits are to store other header values in.
            // so we need to extract the first 12 bits

            byte[] protocalBytes = new byte[2];
            // first 8 bytes are part of the protocal so we can simple copy that
            protocalBytes[0] = ByteData[2];
            // we only need the first 4 bits from the second byte
            ByteModifier.SetBit(ref protocalBytes[1], 1, ByteModifier.IsBitSet(ByteData[3], 1));
            ByteModifier.SetBit(ref protocalBytes[1], 2, ByteModifier.IsBitSet(ByteData[3], 2));
            ByteModifier.SetBit(ref protocalBytes[1], 3, ByteModifier.IsBitSet(ByteData[3], 3));

            // size is found in the first 2 bytes
            packetHeader.Protocol = BitConverter.ToInt16(protocalBytes, 0);

            return;
        }

        /// <summary>
        /// Gets <see cref="Addressable"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeAddressable(byte[] ByteData, PacketHeader packetHeader)
        {
            packetHeader.Addressable = ByteModifier.IsBitSet(ByteData[3], 4);
        }


        /// <summary>
        /// Gets <see cref="Tagged"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeTagged(byte[] ByteData, PacketHeader packetHeader)
        {
            packetHeader.Tagged = ByteModifier.IsBitSet(ByteData[3], 5);
        }

        /// <summary>
        /// Gets <see cref="Origin"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeOrigin(byte[] ByteData, PacketHeader packetHeader)
        {
            byte origin = 0;

            // origin is found in the fourth byte in the 6th and 7th bit positions
            bool isFirstBitSet = ByteModifier.IsBitSet(ByteData[3], 6);
            bool isSecondBitSet = ByteModifier.IsBitSet(ByteData[3], 7);

            
            ByteModifier.SetBit(ref origin, 1, isFirstBitSet);
            ByteModifier.SetBit(ref origin, 2, isSecondBitSet);

            packetHeader.Origin = (SByte)origin;
        }

        /// <summary>
        /// Gets <see cref="Source"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeSource(byte[] ByteData, PacketHeader packetHeader)
        {
            // Source starts at the fifth byte and is 4 bytes long
            packetHeader.Source = BitConverter.ToInt32(ByteData, 4);
        }

        /// <summary>
        /// Gets <see cref="Target"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeTarget(byte[] ByteData, PacketHeader packetHeader)
        {
            // Target is 8 bytes long (but currently only uses the first 6 bytes) and starts at the 9th byte
            double targetAsNumbers = BitConverter.ToDouble(ByteData, 8);

            byte[] targetAsByteArray = new byte[6];
            Array.Copy(ByteData, 8, targetAsByteArray, 0, 6);


            packetHeader.Target = BitConverter.ToString(targetAsByteArray).Replace("-", "");
        }

        /// <summary>
        /// Gets <see cref="ReservedOne"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeReservedOne(byte[] ByteData, PacketHeader packetHeader)
        {
            // ReserveOne is 6 bytes long and starts and 17th byte

            byte[] reservedOne = new byte[6];
            Array.Copy(ByteData, 16, reservedOne, 0, 6);
        
            packetHeader.ReservedOne = reservedOne; 
        }

        /// <summary>
        /// Gets <see cref="ResRequired"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeResRequired(byte[] ByteData, PacketHeader packetHeader)
        {
            // Res Required is 1 bit long and stored in the 23rd byte at position 1 (1 to 8 bits)
            packetHeader.ResRequired = ByteModifier.IsBitSet(ByteData[22], 1);
            
        }

        /// <summary>
        /// Gets <see cref="AckRequired"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeAckRequired(byte[] ByteData, PacketHeader packetHeader)
        {
            // Ack Required is 1 bit long and stored in the 23rd byte at position 2 (1 to 8 bits)
            packetHeader.AckRequired = ByteModifier.IsBitSet(ByteData[22], 2);
        }

        /// <summary>
        /// Gets <see cref="ReservedTwo"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeReservedTwo(byte[] ByteData, PacketHeader packetHeader)
        {
            // ReservedTwo is 6 bits long and stored in the 23rd byte starting at position 3 ( 1 to 8 bits)
            BitArray reservedTwo = new BitArray(6);

            reservedTwo.Set(0, ByteModifier.IsBitSet(ByteData[22], 3));
            reservedTwo.Set(1, ByteModifier.IsBitSet(ByteData[22], 4));
            reservedTwo.Set(2, ByteModifier.IsBitSet(ByteData[22], 5));
            reservedTwo.Set(3, ByteModifier.IsBitSet(ByteData[22], 6));
            reservedTwo.Set(4, ByteModifier.IsBitSet(ByteData[22], 7));
            reservedTwo.Set(5, ByteModifier.IsBitSet(ByteData[22], 8));

            packetHeader.ReservedTwo = reservedTwo;
        }

        /// <summary>
        /// Gets <see cref="Sequence"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeSequence(byte[] ByteData, PacketHeader packetHeader)
        {
            // Sequence is 8 bits long (one byte) and is stored in the 24th byte
            packetHeader.Sequence = ByteData[23];
        }

        /// <summary>
        /// Gets <see cref="ReservedThree"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeReservedThree(byte[] ByteData, PacketHeader packetHeader)
        {
            // Reserved Three is 8 bytes long and starts at the 25th byte
            packetHeader.ReservedThree = BitConverter.ToInt64(ByteData, 24);
        }

        /// <summary>
        /// Gets <see cref="PacketType"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodePacketType(byte[] ByteData, PacketHeader packetHeader)
        {
            // PacketType is 2 bytes long and starts at the 33rd byte
            packetHeader.PacketType = BitConverter.ToInt16(ByteData, 32);

            packetHeader.PayloadType = (MessageType)packetHeader.PacketType;
        }

        /// <summary>
        /// Gets <see cref="ReservedFour"/> from ByteData
        /// </summary>
        /// <param name="ByteData">The data that contains the value we are looking for</param>
        /// <param name="packetHeader">Place to store the value we find</param>
        private static void DecodeReservedFour(byte[] ByteData, PacketHeader packetHeader)
        {
            // PacketType is 2 bytes long and starts at the 35th byte
            packetHeader.ReservedFour = BitConverter.ToInt16(ByteData, 34);
        }


        #endregion

        #endregion

    }
}
