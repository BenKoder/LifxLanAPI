
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Packets.Payload.Types
{
    public class SetColorPayload : PayloadBase
    {
        #region public properties
        /// <summary>
        /// For now always set to zero
        /// </summary>
        public byte ReservedOne { get; set; }

        /// <summary>
        /// Must be a number from 0 to 259
        /// </summary>
        public UInt16 Hue { get; set; }
        /// <summary>
        /// Must be a number between 0 and 100
        /// </summary>
        public UInt16 Saturation { get; set; }
        /// <summary>
        /// Must be a number between 0 and 100
        /// </summary>
        public UInt16 Brightness { get; set; }

        public UInt16 Kelvin { get; set; }

        public uint Duration { get; set; }

        #endregion

        #region public methods
        /// <summary>
        /// Creats a SetColor packet to transfer accross the network
        /// </summary>
        /// <param name="hue">Must be a number from 0 to 259</param>
        /// <param name="saturation">0 to 100</param>
        /// <param name="brightness">0 to 100</param>
        /// <param name="kelvin">See lifx product details for this value</param>
        /// <returns>Binary string of the encoded packet</returns>
        public string EncodePacket(UInt16 hue, UInt16 saturation, UInt16 brightness, UInt16 kelvin)
        {
            this.PayloadType = MessageType.SetColor;

            this.Hue = hue;
            this.Saturation = saturation;
            this.Brightness = brightness;
            this.Kelvin = kelvin;
            this.Duration = 500;

            return this.EncodePacketToBinary();
        }
        
        #endregion


        #region private methods

        #region converters

        #region Hue converter
        /// <summary>
        /// Converts a Hue Value (0 to 260) to the Lifx Equiverlent number
        /// </summary>
        /// <param name="value">The hue value to convert to lifex equiverlent</param>
        /// <returns>Value between 0 and 65535</returns>
        private UInt16 ConvertHueToLifxEquiverlent(UInt16 value)
        {
            UInt16 maxValue = 65535; // could have got this by doing UInt16.MaxValue;

            if (value == 260)
                return maxValue;
            else
                return (UInt16)(((maxValue * value) / 360) % maxValue);
        }

        /// <summary>
        /// Converts a Lifx Hue Value (0 to 65535) to a value 0 to 260
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private UInt16 ConvertHueFromLifexEquiverlent(UInt16 value)
        {
            UInt16 maxValue = 65535; // could have got this by doing UInt16.MaxValue;

            return  (UInt16)Math.Round( (float)value * 360 / maxValue, 2);
        }
        #endregion

        #region Brightness and Saturation converters
        /// <summary>
        /// Converst the <see cref="Brightness"/> or <see cref="Saturation"/> value to a lifex equiverlent
        /// </summary>
        /// <param name="value">Value from 0 to 100</param>
        /// <returns>value from 0 to 65535</returns>
        private UInt16 ConvertSaturationOrBrightnessToLifxEquiverlent(UInt16 value)
        {
            double valueAsDecimal = (int)value / 100.00;


            return (UInt16)Math.Round(0xFFFF * valueAsDecimal);
        }
        /// <summary>
        /// Converts a lifex equiverlent back to <see cref="Brightness"/> or <see cref="Saturation"/>
        /// </summary>
        /// <param name="value">value from 0 to 65535</param>
        /// <returns>value from 0 to 100</returns>
        private UInt16 ConvertSaturationOrBrightnessFromLifxEquiverlent(UInt16 value)
        {
            UInt16 saturation = (UInt16)Math.Round((decimal)(value / 0xFFFF), 4);
            return (UInt16)(saturation * 100);
        }
        #endregion

        #endregion



        #region Binary Encoders
        private string EncodePacketToBinary()
        {
            this.EncodedPacketsAsByte.Clear();
            this.EncodedPacketAsBinaryString = String.Empty;

            StringBuilder sb = new StringBuilder();

            sb.Append(this.ConvertReservedOneToBinary());

            sb.Append(this.ConvertHueToBinary());

            sb.Append(this.ConvertSaturationToBinary());

            sb.Append(this.ConvertBrightnessToBinary());

            sb.Append(this.ConvertKelvinToBinary());

            sb.Append(this.ConvertDurationToBinary());



            return sb.ToString();
        }
        /// <summary>
        /// Converts the <see cref="ReservedOne"/> to a binary string
        /// </summary>
        /// <returns>8 bit binary string</returns>
        private string ConvertReservedOneToBinary()
        {
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(new byte[] { this.ReservedOne });

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.Add(this.ReservedOne);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="Hue"/> to a binary string
        /// </summary>
        /// <returns>16 bit binary string</returns>
        private string ConvertHueToBinary()
        {
            // convert the hue to a lifx equivelent value
            UInt16 lifxHue = this.ConvertHueToLifxEquiverlent(this.Hue);
            // convert the value to a byte array
            byte[] byteData = this._ByteConverter.ConvertValueToBytes(lifxHue);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(byteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(byteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="Saturation"/> to a binary string
        /// </summary>
        /// <returns>16 bit binary string</returns>
        private string ConvertSaturationToBinary()
        {
            // converts the saturation to a lifx equivelent value
            UInt16 LifxSaturation = this.ConvertSaturationOrBrightnessToLifxEquiverlent(this.Saturation);
            // convert the value to a byte array
            byte[] byteData = this._ByteConverter.ConvertValueToBytes(LifxSaturation);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(byteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(byteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="Brightness"/> to a lifx equivelent value
        /// </summary>
        /// <returns>16 bit binary string</returns>
        private string ConvertBrightnessToBinary()
        {
            // Converts the brightness to a lifx equivelent value
            UInt16 LifxBrightness = this.ConvertSaturationOrBrightnessToLifxEquiverlent(this.Brightness);
            // convert the value to a byte array
            byte[] byteData = this._ByteConverter.ConvertValueToBytes(LifxBrightness);


            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(byteData);


            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(byteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="Kelvin"/> to a binary string
        /// </summary>
        /// <returns>16 bit binary string</returns>
        private string ConvertKelvinToBinary()
        {
            byte[] byteData = this._ByteConverter.ConvertValueToBytes(this.Kelvin);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(byteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(byteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }

        /// <summary>
        /// Converts the <see cref="Duration"/> to a binary string
        /// </summary>
        /// <returns></returns>
        private string ConvertDurationToBinary()
        {
            byte[] byteData = this._ByteConverter.ConvertValueToBytes(this.Duration);
            // convert the byte array to a binary string
            string valueAsBinaryString = this._ByteConverter.ConvertByteArrayToBinaryString(byteData);

            // add the packets to the total converted packets
            this.EncodedPacketsAsByte.AddRange(byteData);
            this.EncodedPacketAsBinaryString += valueAsBinaryString;

            return valueAsBinaryString;
        }
        #endregion

        #endregion

    }
}
