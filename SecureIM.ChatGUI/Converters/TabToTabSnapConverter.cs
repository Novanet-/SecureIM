using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.Converters
{
    /// <summary>
    ///     This converter is to SecureIM.ChatGUInstrate how to dynamically choose what tabs can snap out to form new windows.
    /// </summary>
    public class TabToTabSnapConverter : IValueConverter
    {
        public object Convert(object value, [NotNull] Type targetType, object parameter, [NotNull] CultureInfo culture)
            => !(value is TabClass3);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}