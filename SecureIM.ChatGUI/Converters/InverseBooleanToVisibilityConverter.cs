using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using JetBrains.Annotations;

namespace SecureIM.ChatGUI.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        [NotNull]
        public object Convert(object value, [NotNull] Type targetType, object parameter, [NotNull] CultureInfo culture)
            =>
                    new BooleanToVisibilityConverter().Convert(!(bool?) value == true, targetType, parameter,
                                                               culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}