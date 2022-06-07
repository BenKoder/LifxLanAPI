using Core.LifxProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using WpfApp.ViewModels.Base;

namespace WpfApp.ViewModels.Home
{
    public class HomeViewModel : BaseViewModel
    {
        /// <summary>
        /// Time that gets started when searching for products on the network.
        /// Once timer has finished it is no longer searching for products until a
        /// new request to search for products on network has been asked for by
        /// calling the <see cref="SearchForProductsCommand"/> is clicked
        /// </summary>
        private DispatcherTimer _ProductsSearchTimer;

        #region commands
        /// <summary>
        /// Gets called when we want to send a broadcast message to the network
        /// to look for lifx products on the local network
        /// </summary>
        public ICommand SearchForProductsCommand { get; set; }

        public ICommand ProductSelectedCommand { get; set; }
        #endregion


        public HomeViewModel()
        {
            this.SearchForProductsCommand = new RelayCommand(() => 
            {
                // make sure we are not allready looking for products
                if (this._ProductsSearchTimer.IsEnabled)
                    return;

                // tell viewmodel we are looking for products on the network
                this.IsSearchingForProducts = true;
                // send a broadcast message to find products on the network
                ApplicationViewModel.InstanceAppViewModelInstance.Lifx.FindDevicesOnNetwork();
                // start a 10 second timer. This is the ammount of time we will allow
                // for a response from the network to our broadcast message.
                this._ProductsSearchTimer.Start();
            });

            this.ProductSelectedCommand = new RelayParameterizedCommand((ProductSelected) => 
            {
                // What type of product we selected will determin what type of page will will move too
                switch(ProductSelected)
                {
                    // is a lifxBulb
                    case LifxBulb lb:

                        ApplicationViewModel.InstanceAppViewModelInstance.SelectedProduct = (ProductBase)ProductSelected;
                        //ApplicationViewModel.InstanceAppViewModelInstance.CurrentPage = DataModels.ApplicationPages.WhiteColorPicker;
                        ApplicationViewModel.InstanceAppViewModelInstance.CurrentPage = DataModels.ApplicationPages.FullColorPicker;
                        break;
                }
                
            });



            this._ProductsSearchTimer = new DispatcherTimer();
            // we will wait 10 seconds to get a response from the network to us
            // sending a broadcast message asking which projects are on the network
            this._ProductsSearchTimer.Interval = new TimeSpan(0, 0, 0, 5);
            this._ProductsSearchTimer.Tick += _ProductsSearchTimer_Tick;
            this.IsSearchingForProducts = true;
            // The broadcast message has allready been sent from another point in code
            // so set the timer off to indicate we are looking for products.
            this._ProductsSearchTimer.Start();
        }

        

        public ListCollectionView Products
        {
            get => ApplicationViewModel.InstanceAppViewModelInstance.FoundProducts;
            set
            {
                ApplicationViewModel.InstanceAppViewModelInstance.FoundProducts = value;
            }
        }


        private bool _IsSearchingForProducts = true;
        public bool IsSearchingForProducts 
        { 
            get => this._IsSearchingForProducts;
            set
            {
                this._IsSearchingForProducts = value;
                this.OnPropertyChanged(nameof(IsSearchingForProducts));
            }
        }


        /// <summary>
        /// Gets called when the dispacher timer has done 10 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void _ProductsSearchTimer_Tick(object sender, EventArgs e)
        {
            this._ProductsSearchTimer.Stop();
            this.IsSearchingForProducts = false;
        }
    }
}
