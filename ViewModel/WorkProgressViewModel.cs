using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DocumentHub.ViewModel
{
    public class WorkProgressViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<WorkProgress> WorkList { get; set; } = new();
        public ObservableCollection<string> StatusOptions { get; set; } = new() { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Tạm hoãn" };
        public ObservableCollection<string> PriorityOptions { get; set; } = new() { "Thấp", "Trung bình", "Cao", "Khẩn cấp" };
        public ObservableCollection<Person> Assigners { get; set; } = new();
        public ObservableCollection<Person> StaffList { get; set; } = new();

        private WorkProgress _selectedWorkItem;
        public WorkProgress SelectedWorkItem
        {
            get => _selectedWorkItem;
            set { _selectedWorkItem = value; OnPropertyChanged(); }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set { _searchKeyword = value; OnPropertyChanged(); }
        }

        private string _notificationMessage;
        public string NotificationMessage
        {
            get => _notificationMessage;
            set { _notificationMessage = value; OnPropertyChanged(); }
        }

        public WorkProgress WorkItem { get; set; } = new();

        public ICommand SaveCommand => new RelayCommand(_ => SaveWorkItem());
        public ICommand DeleteCommand => new RelayCommand(_ => DeleteWorkItem());
        public ICommand CompleteCommand => new RelayCommand(_ => MarkAsComplete());

        private void SaveWorkItem()
        {
            if (!WorkList.Contains(WorkItem))
                WorkList.Add(WorkItem);

            CalculateNotifications(WorkItem);

            WorkItem = new WorkProgress();
            OnPropertyChanged(nameof(WorkItem));
        }

        private void DeleteWorkItem()
        {
            if (SelectedWorkItem != null)
                WorkList.Remove(SelectedWorkItem);
        }

        private void MarkAsComplete()
        {
            if (SelectedWorkItem != null)
                SelectedWorkItem.Status = "Hoàn thành";
        }

        private void CalculateNotifications(WorkProgress item)
        {
            if (item.NotificationDate == null) return;

            DateTime baseDate = item.NotificationDate.Value;
            List<string> messages = new();

            if (item.IsMonth)
                messages.Add($"Thông báo sau 1 tháng: {baseDate.AddMonths(1):dd/MM/yyyy}");
            if (item.Is3Months)
                messages.Add($"Thông báo sau 3 tháng: {baseDate.AddMonths(3):dd/MM/yyyy}");
            if (item.Is6Months)
                messages.Add($"Thông báo sau 6 tháng: {baseDate.AddMonths(6):dd/MM/yyyy}");
            if (item.Is9Months)
                messages.Add($"Thông báo sau 9 tháng: {baseDate.AddMonths(9):dd/MM/yyyy}");
            if (item.IsYear)
                messages.Add($"Thông báo sau 1 năm: {baseDate.AddYears(1):dd/MM/yyyy}");

            NotificationMessage = string.Join(" | ", messages);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
