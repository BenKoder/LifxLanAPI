using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp.ViewModels;

namespace WpfApp.Pages
{
    public class BasePage<vm> : Page
        where vm : BaseViewModel, new()
    {
        #region Private Members
        /// <summary>
        /// View Model associsated with this page
        /// </summary>
        private vm _VM;
        #endregion


        #region constructor
        public BasePage()
        {
            this._VM = new vm();
            this.DataContext = this._VM;
        }
        #endregion


        #region pulic properties
        /// <summary>
        /// View Model associsated with this page
        /// </summary>
        public vm ViewModel
        {
            get
            {
                return this._VM;
            }
            set
            {
                if (this._VM == value)
                    return;

                this._VM = value;
            }
        }

        #endregion
    }
}
