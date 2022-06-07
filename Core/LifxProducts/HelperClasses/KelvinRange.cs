using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts.HelperClasses
{
    public class KelvinRange
    {
        public KelvinRange(int from, int too)
        {
            this.From = (ushort)from;
            this.Too = (ushort)too;
        }

        public ushort From { get; set; }
        public ushort Too { get; set; }

        public static bool IsWithinRange(int KelvinValue, KelvinRange RangeToLookAt)
        {
            // if KelvinValue is less than or greater than the From and Too values within
            // RangeToLookAt, return false, else return true
            if (KelvinValue < RangeToLookAt.From || KelvinValue > RangeToLookAt.Too)
                return false;
            else
                return true;
        }
    }
}
