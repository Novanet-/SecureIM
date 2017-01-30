using System;
using System.Globalization;
using System.Windows.Data;

namespace SecureIM.ChatGUI.Converters
{
    public class BooleanToPinTabTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool?) value == true ? "Unpin Tab" : "Pin Tab";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}