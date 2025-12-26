using DocumentHub.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Frontend.Views
{
    /// <summary>
    /// Interaction logic for IncomingDocView.xaml
    /// </summary>
    public partial class IncomingDocView : UserControl
    {
        public IncomingDocView()
        {
            InitializeComponent();
            this.DataContext = new IncomingDocViewModel();
        }
        public class EmptyStringToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                var str = value as string;
                return string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

    }
}
