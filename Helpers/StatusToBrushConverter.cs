using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DocumentHub.Helpers
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value?.ToString() ?? "";

            return status switch
            {
                "Chưa bắt đầu" => new SolidColorBrush(Colors.Gray),      
                "Đang thực hiện" => new SolidColorBrush(Colors.Blue),      
                "Hoàn thành" => new SolidColorBrush(Colors.Green),      
                "Tạm hoãn" => new SolidColorBrush(Colors.OrangeRed),  
                _ => new SolidColorBrush(Colors.Black)     
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
