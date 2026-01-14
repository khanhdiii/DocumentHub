using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

using DocumentHub.Components;
using DocumentHub.Data;
using DocumentHub.FrontEnd.Views;
using DocumentHub.Model;
using DocumentHub.ViewModel;

using Frontend.Views;

using FrontEnd.Views;

namespace DocumentHub.FrontEnd
{
    public partial class Main : Window
    {
        private const double ExpandedWidth = 250;
        private const double CollapsedWidth = 52;
        private readonly Duration _animDuration = new(TimeSpan.FromMilliseconds(230));

        public Main(string notificationMessage = null, bool isSuccess = false)
        {
            InitializeComponent();

            // Show notification after UI is loaded
            Loaded += (s, e) =>
            {
                //Show notify after login
                ShowWeeklyWorkScreen();

                if (!string.IsNullOrEmpty(notificationMessage))
                {
                    var toast = new NotificationControl();
                    toast.Show(notificationMessage, isSuccess);
                    NotificationContainer.Children.Add(toast);

                }
            };

            using var _context = new AppDbContext();
            _context.Database.EnsureCreated();

            // Ensure ToggleButton events are handled even if handlers aren't set in XAML
            if (BtnToggleSidebar != null)
            {
                BtnToggleSidebar.Checked += BtnToggleSidebar_Checked;
                BtnToggleSidebar.Unchecked += BtnToggleSidebar_Unchecked;
            }

            // Load default sub-form
            ContentArea.Content = new DashboardView();
        }
        private void ShowWeeklyWorkNotification()
        {
            var vm = new WorkProgressViewModel();
            vm.LoadWorkItems();
            vm.LoadWeeklyWork();
            // Filter job in week
            if (vm.WeeklyWorkList != null && vm.WeeklyWorkList.Any())
            {
                string message = "📅 Công việc trong tuần:\n" + string.Join("\n", vm.WeeklyWorkList.Select(w => "• " + w.Name));
                var toast = new NotificationControl();
                toast.Show(message, true);
                NotificationContainer.Children.Add(toast);
            }
        }
        private void ShowWeeklyWorkScreen()
        {
            var vm = new WorkProgressViewModel();
            vm.LoadWorkItems();
            vm.LoadWeeklyWork();

            if (vm.WeeklyWorkList != null && vm.WeeklyWorkList.Any())
            {
                var view = new WeeklyWorkView();
                view.DataContext = vm;

                ContentArea.Content = view;
            }
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
        private void btnGuide_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new UserGuide();
        }

        private void btnIncomingDoc_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new IncomingDocView();

            // Create ViewModel
            var vm = new IncomingDocViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
        }

        private void btnOutgoing_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new OutgoingDocView();

            // Create ViewModel
            var vm = new OutgoingDocViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
        }

        private void btnConstructionStaff_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new ConstructionStaffView();

            // Create ViewModel
            var vm = new ConstructionStaffViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
        }

        private void btnReceivingOfficer_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new ReceivingOfficerView();

            // Create ViewModel
            var vm = new ReceivingOfficerViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
        }

        private void btnSigner_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new SignerView();

            // Create ViewModel
            var vm = new SignerViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
        }

        private void btnRecpient_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new RecipientsView();

            // Create ViewModel
            var vm = new RecipientsViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
        }

        private void btnWorkProgress_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new WorkProgressView();

            // Create ViewModel
            var vm = new WorkProgressViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
        }

        private void btnPerson_Click(object sender, RoutedEventArgs e)
        {
            // Create view
            var view = new PersonView();

            // Create ViewModel
            var vm = new PersonViewModel();

            // Set DataContext
            view.DataContext = vm;

            // Set Notify in GlobalNotification
            vm.Notify += (msg, success) =>
            {
                var toast = new NotificationControl();
                toast.Show(msg, success);
                NotificationContainer.Children.Add(toast);
            };

            //Show ContentArea
            ContentArea.Content = view;
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

        public void ShowWeeklyWorkPopup()
        {
            var vm = new WorkProgressViewModel();
            vm.LoadWorkItems();
            vm.LoadWeeklyWork();

            if (vm.WeeklyWorkList != null && vm.WeeklyWorkList.Any())
            {
                var popup = new Popup
                {
                    Placement = PlacementMode.Bottom,
                    PlacementTarget = BtnNotification, 
                    StaysOpen = false,
                    AllowsTransparency = true,
                    Child = new WeeklyWorkView { DataContext = vm }
                };

                popup.IsOpen = true;
            }
            else
            {
                MessageBox.Show("Không có công việc nào trong tuần này.",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }
        private void BtnNotification_Click(object sender, RoutedEventArgs e)
        {
            var vm = new WorkProgressViewModel();
            vm.LoadWorkItems();
            vm.LoadWeeklyWork();

            if (vm.WeeklyWorkList != null && vm.WeeklyWorkList.Any())
            {
                NotificationList.ItemsSource = vm.WeeklyWorkList;
                NotificationPopup.IsOpen = true; // mở popup
            }
            else
            {
                MessageBox.Show("Không có công việc nào trong tuần này.",
                                "Thông báo",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }


    }
}
