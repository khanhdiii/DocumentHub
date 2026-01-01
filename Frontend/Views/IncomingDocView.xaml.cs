using DocumentHub.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
        }
    }
}
