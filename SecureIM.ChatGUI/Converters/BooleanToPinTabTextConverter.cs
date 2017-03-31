using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace SecureIM.ChatGUI.Converters
{
    public class BooleanToPinTabTextConverter : IValueConverter
    {
        [NotNull]
        public object Convert(object value, [NotNull] Type targetType, object parameter, [NotNull] CultureInfo culture)
            => (bool?) value == true ? "Unpin Tab" : "Pin Tab";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}