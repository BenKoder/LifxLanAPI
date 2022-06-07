using Core.LifxProducts.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts.HelperClasses
{
    public class BulbStatus
    {
        public LifxColor CurrentColor { get; private set; }

        public BulbStatus()
        {
            CurrentColor = new LifxColor();
        }

        
    }
}
