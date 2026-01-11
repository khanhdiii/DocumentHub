using System.Windows;
using System.Windows.Controls;

using DocumentHub.Components;
using DocumentHub.ViewModel;

namespace DocumentHub.FrontEnd.Views
{
    /// <summary>
    /// Interaction logic for WorkProgressView.xaml
    /// </summary>
    public partial class WorkProgressView : UserControl
    {
        public WorkProgressView()
        {
            InitializeComponent();
            this.DataContext = new WorkProgressViewModel();
            Loaded += (s, e) =>
            {
                if (DataContext is WorkProgressViewModel vm)
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
