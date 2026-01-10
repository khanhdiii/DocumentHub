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
        public class Person { public string FullName { get; set; } }
        public ObservableCollection<SelectableMonth> MonthOptions { get; } = new();
        public ObservableCollection<SelectableYear> YearOptions { get; } = new();
        public WorkProgressViewModel()
        {
            WorkItem = new WorkProgress();
            WorkItem.PropertyChanged += WorkItem_PropertyChanged;
        }

        private void WorkItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkProgress.IsMonth))
            {
                if (WorkItem.IsMonth)
                    GenerateMonthOptions();
                else
                    MonthOptions.Clear();
            }
            else if (e.PropertyName == nameof(WorkProgress.IsYear) || e.PropertyName == nameof(WorkProgress.NotificationDate))
            {
                if (WorkItem.IsYear)
                    GenerateYearOptions();
                else
                    YearOptions.Clear();
            }
        }

        private bool _isMonthSelected;
        public bool IsMonthSelected
        {
            get => _isMonthSelected;
            set
            {
                _isMonthSelected = value;
                OnPropertyChanged();
                if (value)
                    GenerateMonthOptions();
                else
                    MonthOptions.Clear();
            }
        }

        private bool _isYearSelected;
        public bool IsYearSelected
        {
            get => _isYearSelected;
            set
            {
                _isYearSelected = value;
                OnPropertyChanged();
                if (value)
                    GenerateYearOptions();
                else
                    YearOptions.Clear();
            }
        }

        private void GenerateMonthOptions()
        {
            MonthOptions.Clear();
            for (int i = 1; i <= 12; i++)
                MonthOptions.Add(new SelectableMonth { Month = i, MonthLabel = $"Tháng {i}" });
        }


        private void GenerateYearOptions()
        {
            YearOptions.Clear();
            int startYear = WorkItem.NotificationDate?.Year ?? DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
            {
                YearOptions.Add(new SelectableYear { Year = startYear + i });
            }
        }

        private WorkProgress _selectedWorkItem;
        public WorkProgress SelectedWorkItem
        {
            get => _selectedWorkItem;
            set
            {
                _selectedWorkItem = value;
                OnPropertyChanged();
            }
        }

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged();
            }
        }

        private string _notificationMessage;
        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                _notificationMessage = value;
                OnPropertyChanged();
            }
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

            CreateNewWorkItem();
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
        private void CreateNewWorkItem()
        {
            WorkItem = new WorkProgress();
            WorkItem.PropertyChanged += WorkItem_PropertyChanged;
            OnPropertyChanged(nameof(WorkItem));
            UpdateOptions();
        }

        // Khi WorkItem.IsMonth thay đổi → gọi GenerateMonthOptions
        private void UpdateOptions()
        {
            if (WorkItem.IsMonth)
                GenerateMonthOptions();
            else
                MonthOptions.Clear();
            if (WorkItem.IsYear)
                GenerateYearOptions();
            else
                YearOptions.Clear();
        }

        public class SelectableMonth : INotifyPropertyChanged
        {
            public int Month
            {
                get; set;
            }
            public string MonthLabel
            {
                get; set;
            }
            private bool _isSelected;
            public bool IsSelected
            {
                get => _isSelected; set
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string n = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        }

        public class SelectableYear : INotifyPropertyChanged
        {
            public int Year { get; set; }
            private bool _isYear;
            public bool IsYear { get => _isYear; set { _isYear = value; OnPropertyChanged(); } }
            private bool _isSelected;
            public bool IsSelected
            {
                get => _isSelected; set
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string n = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        }



        private void CalculateNotifications(WorkProgress item)
        {
            if (item.NotificationDate == null)
                return;
            DateTime baseDate = item.NotificationDate.Value;
            List<string> messages = new();

            foreach (var m in MonthOptions.Where(x => x.IsSelected))
                messages.Add($"Thông báo sau {m.Month} tháng: {baseDate.AddMonths(m.Month):dd/MM/yyyy}");

            foreach (var y in YearOptions.Where(x => x.IsSelected))
            {
                var target = new DateTime(y.Year, baseDate.Month, Math.Min(baseDate.Day, DateTime.DaysInMonth(y.Year, baseDate.Month)));
                messages.Add($"Thông báo năm {y.Year}: {target:dd/MM/yyyy}");
            }

            if (item.Is3Months)
                messages.Add($"Thông báo sau 3 tháng: {baseDate.AddMonths(3):dd/MM/yyyy}");
            if (item.Is6Months)
                messages.Add($"Thông báo sau 6 tháng: {baseDate.AddMonths(6):dd/MM/yyyy}");
            if (item.Is9Months)
                messages.Add($"Thông báo sau 9 tháng: {baseDate.AddMonths(9):dd/MM/yyyy}");
            if (item.IsYearly)
                messages.Add($"Nhắc hàng năm vào {baseDate:dd/MM}");

            if (item.IsSudden && item.SuddenDate != null)
                messages.Add($"Thông báo đột xuất vào ngày: {item.SuddenDate:dd/MM/yyyy}");

            if (item.IsSeminar && item.SeminarDate != null)
                messages.Add($"Thông báo chuyên đề: {item.SeminarDate:dd/MM/yyyy}");

            NotificationMessage = string.Join(" | ", messages);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
