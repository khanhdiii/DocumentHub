using System.Windows.Controls;
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
        }
    }
}
