using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.LifxProducts
{
    public class ProductsList
    {
        private List<ProductBase> _ListOfProducts = new List<ProductBase>();
        //private System.Collections.Concurrent.ConcurrentBag<ProductBase> _ListOfProducts = new System.Collections.Concurrent.ConcurrentBag<ProductBase>();
        private object _ThreadLock = new object();

        #region public methods
        /// <summary>
        /// Adds the passed in product to the list (will check using mac address that it does not allready exsist)
        /// </summary>
        /// <param name="product">product to add</param>
        public void Add(ProductBase product)
        {
            lock (_ThreadLock)
            {
                // make sure it does not allready exist
                if (!this.Exists(product.MacAddress))
                    this._ListOfProducts.Add(product);
            }
        }

        /// <summary>
        /// removes the passed in product based on its mac address. if can't find mac address, it will not be removed
        /// </summary>
        /// <param name="product">product to remove</param>
        public void Remove(ProductBase product)
        {
            lock (_ThreadLock)
            {
                int ProductIndex = this._ListOfProducts.FindIndex(x => x.MacAddress == product.MacAddress);

                if (ProductIndex != -1)
                    this._ListOfProducts.RemoveAt(ProductIndex);
            }
        }

        
        public bool Exists(string MacAddress)
        {
            // this is throwing an error when run on a concel app (ok in a windows app).
            // I suspect it is due to that we are running code on differnet threads (console app wont switch
            // back to the main thread that theListOfProducts was creatd on).
            // return this._ListOfProducts.Exists(s => s.MacAddress == MacAddress);
            
            
            // Having some threading issues hear when running under Concel app (ok in windows app).
            // I suspect its because under concel app we are not switching back to the main thread.
            // Under Windows app we are switching back to main thread ok.
            // Sticking the below code inside a lock is fixing the problem but i suspect there
            // will be other places that may present issues when under a concel app.
            // Might be that using a Thread Safe list would fix the issue.
            lock (_ThreadLock)
            {
                for (int i = 0; i < this._ListOfProducts.Count; i++)
                {
                    ProductBase product = this._ListOfProducts[i];
                    if (product.MacAddress == MacAddress)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Looks for the Product based on passed in mac address and returns it
        /// </summary>
        /// <param name="MacAddress">used to look for the product we want to find</param>
        /// <returns>if found, the project, else null</returns>
        public ProductBase Find(string MacAddress)
        {
            lock (_ThreadLock)
            {
                return this._ListOfProducts.FirstOrDefault(x => x.MacAddress == MacAddress);
            }
        }

        /// <summary>
        /// The number of products that are currently in the list
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            lock (_ThreadLock)
            {
                return _ListOfProducts.Count;
            }
        }

        /// <summary>
        /// Converts the list to an array and returns the array
        /// </summary>
        /// <returns></returns>
        public ProductBase[] ToArray()
        {
            lock (_ThreadLock)
            {
                return _ListOfProducts.ToArray();
            }
        }

        #endregion

        public static ProductBase CreateProduct(string IpAddress, Packets.LifxPacket lifxPacket, ProductType productType)
        {
            ProductBase product;

            switch (productType)
            {
                case ProductType.LIFX_Original_1000_1:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 567000, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Color_650_3:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 567000, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_White_800_Low_Voltage_10:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 567000, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2700, 6500));
                    break;

                case ProductType.LIFX_White_800_High_Voltage_11:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2700, 6500));
                    break;

                case ProductType.LIFX_Color_1000_15:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_White_900_BR30_Low_Voltage_18:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_White_900_BR30_High_Voltage_19:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Color_1000_BR30_20:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Color_1000_22:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_A19_27:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_BR30_28:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_A19_Night_Vision_29:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, true, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_BR30_Night_Vision_30:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, true, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Z_31:
                    product = new LifxStrip(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, true, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Z_32:
                    product = new LifxStrip(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, true, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Downlight_36:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Downlight_37:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Beam_38:
                    product = new LifxStrip(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, true, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Downlight_White_to_Warm_39:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(1500, 6500));
                    break;

                case ProductType.LIFX_Downlight_40:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(1500, 9000));
                    break;

                case ProductType.LIFX_A19_43:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_BR30_44:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_A19_Night_Vision_45:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, true, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_BR30_Night_Vision_46:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, true, new HelperClasses.KelvinRange(2500, 9000));
                    break;

                case ProductType.LIFX_Mini_Color_49:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(1500, 9000));
                    break;

                case ProductType.LIFX_Mini_White_to_Warm_50:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(1500, 6500));
                    break;

                case ProductType.LIFX_Mini_White_51:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2700, 2700));
                    break;

                case ProductType.LIFX_Candle_68:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(1500, 9000));
                    break;

                case ProductType.LIFX_Filament_Clear_82:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2100, 2100));
                    break;

                case ProductType.LIFX_Filament_Amber_85:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2000, 2000));
                    break;

                case ProductType.LIFX_Mini_White_87:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, false, false, new HelperClasses.KelvinRange(2700, 2700));
                    break;

                /*case ProductType.LIFX_Switch_89:
                    product = new LifxSwitch(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType);
                    break;
                */

                case ProductType.LIFX_Clean_99:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress, productType, true, false, new HelperClasses.KelvinRange(1500, 9000));
                    break;

                case ProductType.Unknown:
                default:
                    return null;
            }

            return product;
        }
    }
}
