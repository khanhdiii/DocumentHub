using System.Windows;
using System.Windows.Controls;

using DocumentHub.Components;
using DocumentHub.FrontEnd;
using DocumentHub.ViewModel;

namespace FrontEnd.Views
{
    /// <summary>
    /// Interaction logic for OutgoingDocView.xaml
    /// </summary>
    public partial class OutgoingDocView : UserControl
    {
        public OutgoingDocView()
        {
            InitializeComponent();
            this.DataContext = new OutgoingDocViewModel();

            Loaded += (s, e) =>
            {
                if (DataContext is ConstructionStaffViewModel vm)
                {
                    vm.Notify -= ShowNotification;
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
