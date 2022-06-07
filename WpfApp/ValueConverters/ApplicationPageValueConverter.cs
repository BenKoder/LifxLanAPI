using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.DataModels;
using WpfApp.Pages.ColorPicker.FullColor;
using WpfApp.Pages.ColorPicker.Whites;
using WpfApp.Pages.Home;

namespace WpfApp.ValueConverters
{
    /// <summary>
    /// Converts the <see cref="DataModels.ApplicationPages"/> to an actual view/page
    /// </summary>
    class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        private static HomePage _HomePage;
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationPages)value)
            {
                case ApplicationPages.Home:
                    return _HomePage ?? (_HomePage = new HomePage());

                case ApplicationPages.WhiteColorPicker:
                    return new WhitesPickerPage();

                case ApplicationPages.FullColorPicker:
                    return new FullColorPickerPage();

                default:
                    return _HomePage ?? (_HomePage = new HomePage());
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
