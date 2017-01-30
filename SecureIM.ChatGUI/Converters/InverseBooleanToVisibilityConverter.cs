using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SecureIM.ChatGUI.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            =>
                    new BooleanToVisibilityConverter().Convert(!(bool?) value == true, targetType, parameter,
                                                               culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}