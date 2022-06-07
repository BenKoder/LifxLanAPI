using Core;
using Core.LifxProducts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfApp.DataModels;

namespace WpfApp.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {
        public static ApplicationViewModel InstanceAppViewModelInstance { get; private set; }

        static ApplicationViewModel()
        {
            InstanceAppViewModelInstance = new ApplicationViewModel();
        }

        public ApplicationViewModel()
        {
            this.Lifx = new LifxCore();
            this.Lifx.LifxDeviceFound += Lifx_LifxDeviceFound;

            // set the filterd list to point to the _FoundProductsOnNetwork ObservableCollection
            this._FoundProducts = (ListCollectionView)CollectionViewSource.GetDefaultView(this._FoundProductsOnNetwork);
            // turn on live sorting (so we new staff get add they are automaticly sorted)
            this._FoundProducts.IsLiveSorting = true;
            this._FoundProducts.LiveSortingProperties.Add(nameof(ProductBase.ProductType));
            // need to change this to the name the user gives the product (not currently able to do this though because not added it to the Lifx core)
            this._FoundProducts.SortDescriptions.Add(new SortDescription(nameof(ProductBase.ProductType), ListSortDirection.Ascending));

            
        }


        /// <summary>
        /// The private property assoshiated with the CurrentPage get/set for determinening
        /// Which page the application is currently showing
        /// </summary>
        private static ApplicationPages _CurrentPage = new ApplicationPages();
        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPages CurrentPage
        {
            get
            {
                return _CurrentPage;
            }
            set
            {
                _CurrentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        private static ProductBase _SelectedProduct;
        public ProductBase SelectedProduct
        {
            get => _SelectedProduct;
            set
            {
                _SelectedProduct = value;
                OnPropertyChanged(nameof(this.SelectedProduct));
            }
        }

        public void FindProductsOnNetwork()
        {
            this.Lifx.FindDevicesOnNetwork();
        }


        public LifxCore Lifx { get; set; }



        private ObservableCollection<ProductBase> _FoundProductsOnNetwork = new ObservableCollection<ProductBase>();
        private ListCollectionView _FoundProducts;
        public ListCollectionView FoundProducts
        {
            get => this._FoundProducts;
            set
            {
                this._FoundProducts = value;
            }
        }




        /// <summary>
        /// Gets Called when a new product has been found
        /// </summary>
        /// <param name="product"></param>
        private void Lifx_LifxDeviceFound(ProductBase product)
        {
            ProductBase? foundProduct = this._FoundProductsOnNetwork.FirstOrDefault( s => s.MacAddress == product.MacAddress );
            if(foundProduct == null)
            //if (this._FoundProductsOnNetwork.IndexOf(product) == -1)
            {
                this._FoundProductsOnNetwork.Add(product);
                this.FoundProducts.Refresh();
            }
            // check they have the same ip address
            else if (foundProduct.IpAddresss != product.IpAddresss)
            {
                // remove the old product and add the new product because the ip address has changed
                this._FoundProductsOnNetwork.Remove(foundProduct);
                this._FoundProductsOnNetwork.Add(foundProduct);
                this.FoundProducts.Refresh();
                
            }
        }

        
    }
}
