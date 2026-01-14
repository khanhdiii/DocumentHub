using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DocumentHub.Helpers
{
    public class ProgressToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int progress = (int)value;

            if (progress < 25)
                return new SolidColorBrush(Colors.Red);       // Red
            else if (progress < 50)
                return new SolidColorBrush(Colors.Orange);    // Orange
            else if (progress < 75)
                return new SolidColorBrush(Colors.Goldenrod); // Goldenrod
            else
                return new SolidColorBrush(Colors.Green);     // Green
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
