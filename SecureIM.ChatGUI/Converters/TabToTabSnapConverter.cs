using System;
using System.Globalization;
using System.Windows.Data;
using SecureIM.ChatGUI.ViewModel;
using SecureIM.ChatGUI.ViewModel.TabClasses;

namespace SecureIM.ChatGUI.Converters
{
    /// <summary>
    ///     This converter is to SecureIM.ChatGUInstrate how to dynamically choose what tabs can snap out to form new windows.
    /// </summary>
    public class TabToTabSnapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TabClass3) return false;
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}