using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DocumentHub.FrontEnd;

using Frontend.Views;

namespace DocumentHub.Components
{
    /// <summary>
    /// Interaction logic for WeeklyWorkView.xaml
    /// </summary>
    public partial class WeeklyWorkView : UserControl
    {
        public WeeklyWorkView()
        {
            InitializeComponent();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Take Main window now
            var mainWindow = Application.Current.Windows.OfType<Main>().FirstOrDefault();
            if (mainWindow != null)
            {
                // Back dashboard
                mainWindow.ContentArea.Content = new DashboardView();
            }
        }
    }
}
