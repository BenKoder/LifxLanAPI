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
using WpfApp.ViewModels.ColorPicker.FullColor;

namespace WpfApp.Pages.ColorPicker.FullColor
{
    /// <summary>
    /// Interaction logic for FullColorPickerPage.xaml
    /// </summary>
    public partial class FullColorPickerPage : BasePage<FullColorPickerPageViewModel>
    {
        public FullColorPickerPage()
        {
            InitializeComponent();
        }

        private void MultiColorPicker_Click(object sender, RoutedEventArgs e)
        {
            Point position = Mouse.GetPosition((IInputElement)sender);
            double buttonWidth = MultiColorPicker.ActualWidth;
            double buttonHeight = MultiColorPicker.ActualHeight;

            double Hue = (position.X / buttonWidth) * 100;
            double Sat = (position.Y / buttonHeight) * 100;

            Hue = (360 / 100) * Hue;

            translate.X = position.X;
            translate.Y = position.Y;

            ((FullColorPickerPageViewModel)this.DataContext).ChangeBulbColor((int)Hue, (int)Sat);
        }
    }
}
