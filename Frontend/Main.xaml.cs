using DocumentHub.FrontEnd.Views;
using DocumentHub.Model;
using Frontend.Views;
using FrontEnd.Views;
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

        private void BtnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AnalyticsView();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AboutView();
        }

        private void btnIncomingDoc_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new IncomingDocView();
        }

        private void btnOutgoing_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new OutgoingDocView();
        }

        private void btnConstructionStaff_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ConstructionStaffView();
        }

        private void btnReceivingOfficer_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ReceivingOfficerView();
        }

        private void btnSigner_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SignerView();
        }

        private void btnRecpient_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new RecipientsView();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new ProcessView();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát?",
                "Thoát",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
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

        //    private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        //    {
        //        SearchBox.Text = string.Empty;
        //        SearchBox.Focus();
        //    }
    }
}