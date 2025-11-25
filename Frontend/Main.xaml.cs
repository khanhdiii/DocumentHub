using DocumentHub.FrontEnd.Views;
using System.Windows;
using System.Windows.Media.Animation;

namespace DocumentHub.FrontEnd
{
    public partial class Main : Window
    {
        private const double ExpandedWidth = 250;
        private const double CollapsedWidth = 52;
        private readonly Duration _animDuration = new(TimeSpan.FromMilliseconds(230));

        public Main()
        {
            InitializeComponent();

            // Ensure ToggleButton events are handled even if handlers aren't set in XAML
            if (BtnToggleSidebar != null)
            {
                BtnToggleSidebar.Checked += BtnToggleSidebar_Checked;
                BtnToggleSidebar.Unchecked += BtnToggleSidebar_Unchecked;
            }

            // load default sub-form
            ContentArea.Content = new DashboardView();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new DashboardView();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SettingsView();
        }

        private void BtnToggleSidebar_Checked(object sender, RoutedEventArgs e)
        {
            // Collapse
            AnimateSidebarTo(CollapsedWidth);
        }

        private void BtnToggleSidebar_Unchecked(object sender, RoutedEventArgs e)
        {
            // Expand
            AnimateSidebarTo(ExpandedWidth);
        }

        private void AnimateSidebarTo(double toWidth)
        {
            var animation = new DoubleAnimation
            {
                To = toWidth,
                Duration = _animDuration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Start animation
            Sidebar.BeginAnimation(WidthProperty, animation);

            // After animation completes, set the local value so layout stays stable
            animation.Completed += (_, _) =>
            {
                // Remove animation and set final width to avoid lingering animated value
                Sidebar.BeginAnimation(WidthProperty, null);
                Sidebar.Width = toWidth;
            };
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            SearchBox.Focus();
        }
    }
}