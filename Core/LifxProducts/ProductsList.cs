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
                case ProductType.Lifx_A19:
                    product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress,
                                            productType, true, false, new HelperClasses.KelvinRange(1500,9000));
                    break;

                case ProductType.Unknown:
                default:
                    return null;
                    
            }

            return product;
        }
    }
}
