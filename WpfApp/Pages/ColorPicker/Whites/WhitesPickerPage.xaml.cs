using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp.ViewModels.ColorPicker.Whites;

namespace WpfApp.Pages.ColorPicker.Whites
{
    /// <summary>
    /// Interaction logic for WhitesPickerPage.xaml
    /// </summary>
    public partial class WhitesPickerPage : BasePage<WhitePickerPageViewModel>
    {
        public WhitesPickerPage()
        {
            InitializeComponent();
        }
    }
}
