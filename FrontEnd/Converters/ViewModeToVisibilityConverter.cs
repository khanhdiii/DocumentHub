using DocumentHub.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DocumentHub.FrontEnd.Converters
{
    public class ViewModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ViewMode mode && parameter is string target)
                return mode.ToString() == target ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
