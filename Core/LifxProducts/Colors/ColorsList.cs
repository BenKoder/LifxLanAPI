using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts.Colors
{
    public class ColorsList
    {
        static ColorsList()
        {
            Red = new LifxColor(0, 100, 50, 1500);
            Green = new LifxColor(109, 100, 50, 1500);
            White = new LifxColor(0, 0, 50, 5000);
            CandleLight = new LifxColor(0, 0, 50, 1500);
            Sunset = new LifxColor(0, 0, 50, 2000);
            UltraWarm = new LifxColor(0, 0, 50, 2500);
            Incandescent = new LifxColor(0, 0, 50, 2700);
            Warm = new LifxColor(0, 0, 50, 3000);
            Neutral = new LifxColor(0, 0, 50, 3500);
            Cool = new LifxColor(0, 0, 50, 4000);
            CoolDaylight = new LifxColor(0, 0, 50, 4500);
            SoftDaylight = new LifxColor(0, 0, 50, 5000);
            Daylight = new LifxColor(0, 0, 50, 5600);
            NoonDaylight = new LifxColor(0, 0, 50, 6000);
            BrightDaylight = new LifxColor(0, 0, 50, 6500);
            CloudyDaylight = new LifxColor(0, 0, 50, 7000);
            BlueDaylight = new LifxColor(0, 0, 50, 7500);
            BlueOvercast = new LifxColor(0, 0, 50, 8000);
            BlueIce = new LifxColor(0, 0, 50, 9000);
        }


        public static LifxColor Red { get; private set; }
        public static LifxColor Green { get; private set; }
        public static LifxColor White { get; private set; }

        #region Differnet Shades of White
        public static LifxColor CandleLight { get; private set; }
        public static LifxColor Sunset { get; private set; }
        public static LifxColor UltraWarm { get; private set; }
        public static LifxColor Incandescent { get; private set; }
        public static LifxColor Warm { get; private set; }
        public static LifxColor Neutral { get; private set; }
        public static LifxColor Cool { get; private set; }
        public static LifxColor CoolDaylight { get; private set; }
        public static LifxColor SoftDaylight { get; private set; }
        public static LifxColor Daylight { get; private set; }
        public static LifxColor NoonDaylight { get; private set; }
        public static LifxColor BrightDaylight { get; private set; }
        public static LifxColor CloudyDaylight { get; private set; }
        public static LifxColor BlueDaylight { get; private set; }
        public static LifxColor BlueOvercast { get; private set; }
        public static LifxColor BlueIce { get; private set; }
        #endregion

    }
}
