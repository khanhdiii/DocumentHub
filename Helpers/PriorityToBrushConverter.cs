using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DocumentHub.Helpers
{
    /*Color for Priority in Datagrid */
    public class PriorityToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string priority = value?.ToString() ?? "";

            return priority switch
            {
                "Thấp" => new SolidColorBrush(Colors.Green),      
                "Trung bình" => new SolidColorBrush(Colors.Blue),     
                "Cao" => new SolidColorBrush(Colors.OrangeRed),   
                "Khẩn cấp" => new SolidColorBrush(Colors.Red),        
                _ => new SolidColorBrush(Colors.Black)       
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
