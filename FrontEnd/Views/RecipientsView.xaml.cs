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

using DocumentHub.Components;
using DocumentHub.ViewModel;

namespace DocumentHub.FrontEnd.Views
{
    /// <summary>
    /// Interaction logic for RecipientsView.xaml
    /// </summary>
    public partial class RecipientsView : UserControl
    {
        public RecipientsView()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (DataContext is ConstructionStaffViewModel vm)
                {
                    vm.Notify += ShowNotification;
                }
            };
        }

        public void ShowNotification(string message, bool isSuccess)
        {
            if (Application.Current.MainWindow is Main mainWindow)
            {
                var toast = new NotificationControl();
                toast.Show(message, isSuccess);
                mainWindow.NotificationContainer.Children.Add(toast);

                System.Diagnostics.Debug.WriteLine($"Notify: {message} - {isSuccess}");
            }
        }
    }
}
