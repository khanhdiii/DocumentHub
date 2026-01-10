using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DocumentHub.Components
{
    public partial class NotificationControl : UserControl
    {
        private DispatcherTimer _timer;
        private int _duration = 3000;
        public StackPanel ParentContainer
        {
            get; set;
        }

        public NotificationControl()
        {
            InitializeComponent();
        }

        public void Show(string message, bool isSuccess)
        {
            // Update Notification
            NotificationText.Text = message;

            if (isSuccess)
            {
                NotificationPanel.Background = Brushes.Green;
                NotificationIcon.Text = "✔️";
            }
            else
            {
                NotificationPanel.Background = Brushes.Red;
                NotificationIcon.Text = "❗";
            }

            // Show Notification
            NotificationPanel.Visibility = Visibility.Visible;
            NotificationProgress.Value = 100;

            _timer?.Stop();

            // Restart time
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            int elapsed = 0;
            _timer.Tick += (s, e) =>
            {
                elapsed += 50;
                NotificationProgress.Value = 100 - (elapsed * 100 / _duration);
                if (elapsed >= _duration)
                {
                    NotificationPanel.Visibility = Visibility.Collapsed;
                    _timer.Stop();

                    // Delete container dad
                    ParentContainer?.Children.Remove(this);
                }

            };
            _timer.Start();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            NotificationPanel.Visibility = Visibility.Collapsed;
            _timer?.Stop();
        }
    }
}
