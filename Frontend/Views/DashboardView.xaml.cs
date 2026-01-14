using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using DocumentHub.Components;
using DocumentHub.FrontEnd;
using DocumentHub.Model;
using DocumentHub.ViewModel;

namespace Frontend.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        //Function take event when begin choose quarter
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DashboardViewModel;
            if (vm != null)
            {
                // Event scroll from ViewModel
                vm.RequestScrollToToday += () =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ScrollToTodayInQuarter();
                    }), DispatcherPriority.Loaded);
                };
            }
        }

        //Function Scroll Quarter
        private void ScrollToTodayInQuarter()
        {
            foreach (var monthContainer in QuarterItemsControl.Items)
            {
                var monthItem = QuarterItemsControl.ItemContainerGenerator.ContainerFromItem(monthContainer) as FrameworkElement;
                if (monthItem == null)
                    continue;

                var itemsControl = FindVisualChild<ItemsControl>(monthItem);
                if (itemsControl == null)
                    continue;

                foreach (var dayContainer in itemsControl.Items)
                {
                    var dayItem = itemsControl.ItemContainerGenerator.ContainerFromItem(dayContainer) as FrameworkElement;
                    if (dayItem?.DataContext is CalendarDay day && day.IsToday)
                    {
                        dayItem.BringIntoView();
                        return;
                    }
                }
            }
        }

        // Function find Child
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    return tChild;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

       

    }
}
