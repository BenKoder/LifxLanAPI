using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts.Colors
{
    public class LifxColor
    {
        public LifxColor()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hue">Must be a number from 0 to 259</param>
        /// <param name="saturation">0 to 100</param>
        /// <param name="brightness">0 to 100</param>
        /// <param name="kelvin">Check Bulb range</param>
        public LifxColor(ushort hue, ushort saturation, ushort brightness, UInt16 kelvin)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
            Kelvin = kelvin;
        }

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


        #region converters

        #region Hue converter
        /// <summary>
        /// Converts a Hue Value (0 to 260) to the Lifx Equiverlent number
        /// </summary>
        /// <param name="value">The hue value to convert to lifex equiverlent</param>
        /// <returns>Value between 0 and 65535</returns>
        public static UInt16 ConvertHueToLifxEquiverlent(UInt16 value)
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
        public static UInt16 ConvertHueFromLifexEquiverlent(UInt16 value)
        {
            UInt16 maxValue = 65535; // could have got this by doing UInt16.MaxValue;

            return (UInt16)Math.Round((float)value * 360 / maxValue, 2);
        }
        #endregion

        #region Brightness and Saturation converters
        /// <summary>
        /// Converst the Brightness or Saturation value to a lifex equiverlent
        /// </summary>
        /// <param name="value">Value from 0 to 100</param>
        /// <returns>value from 0 to 65535</returns>
        public static UInt16 ConvertSaturationOrBrightnessToLifxEquiverlent(UInt16 value)
        {
            double valueAsDecimal = (int)value / 100.00;


            return (UInt16)Math.Round(0xFFFF * valueAsDecimal);
        }
        /// <summary>
        /// Converts a lifex equiverlent back to Brightness or Saturation
        /// </summary>
        /// <param name="value">value from 0 to 65535</param>
        /// <returns>value from 0 to 100</returns>
        public static UInt16 ConvertSaturationOrBrightnessFromLifxEquiverlent(UInt16 value)
        {
            UInt16 saturation = (UInt16)Math.Round((decimal)(value / 0xFFFF), 4);
            return (UInt16)(saturation * 100);
        }
        #endregion

        #endregion
    }
}
