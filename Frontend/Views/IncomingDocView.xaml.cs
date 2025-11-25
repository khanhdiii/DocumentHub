using System.Windows;
using System.Windows.Controls;

namespace Frontend.Views
{
    public partial class IncomingDocView : UserControl
    {
        public IncomingDocView()
        {
            InitializeComponent();
            // TODO: wire up ViewModel or set DataContext here.
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder - implement save logic or command binding.
            MessageBox.Show("Save clicked", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder - implement edit logic.
            MessageBox.Show("Edit clicked", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder - implement delete logic.
            MessageBox.Show("Delete clicked", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder - implement export logic.
            MessageBox.Show("Export CSV clicked", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder - implement search.
            MessageBox.Show($"Search: {TxtSearch.Text}", "Search", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}