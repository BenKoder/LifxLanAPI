using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.ValueConverters
{
    public class InverseBoolConverter : BaseValueConverter<InverseBoolConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ReturnValue;

            // if the parameter has been set to ControlBoolToVisibilityConverterParameter.InverseBool
            // we want to swop the inputValue around.
            if (value != null && value is bool)
                ReturnValue = !(bool)value;
            else
                throw new InvalidCastException("Unable to convert parameter in InverseBoolConvert to bool");
                

            return ReturnValue;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
