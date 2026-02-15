using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using DocumentHub.Components;
using DocumentHub.FrontEnd;
using DocumentHub.ViewModel;

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

            Loaded += (s, e) =>
            {
                if (DataContext is IncomingDocViewModel vm)
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
