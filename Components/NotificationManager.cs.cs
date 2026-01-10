using System.Windows.Controls;
using System.Windows.Threading;

namespace DocumentHub.Components
{
    public static class NotificationManager
    {
        public static void Show(StackPanel container, string message, bool isSuccess)
        {
            var toast = new NotificationControl();
            toast.Show(message, isSuccess);

            container.Children.Add(toast);

            // Sau khi hết thời gian thì tự xóa khỏi container
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(3000) };
            timer.Tick += (s, e) =>
            {
                container.Children.Remove(toast);
                timer.Stop();
            };
            timer.Start();
        }
    }
}
