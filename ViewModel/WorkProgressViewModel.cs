using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using DocumentHub.Data;
using DocumentHub.Model;

using Microsoft.EntityFrameworkCore;

namespace DocumentHub.ViewModel
{
    public class WorkProgressViewModel : INotifyPropertyChanged
    {
        //Action call Notification
        public event Action<string, bool> Notify;
        public event PropertyChangedEventHandler PropertyChanged;

        //Define property
        public ObservableCollection<WorkProgress> WorkList { get; set; } = new();
        public ObservableCollection<string> StatusOptions { get; set; } = new() { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Tạm hoãn" };
        public ObservableCollection<string> PriorityOptions { get; set; } = new() { "Thấp", "Trung bình", "Cao", "Khẩn cấp" };
        public ObservableCollection<SelectableMonth> MonthOptions { get; } = new();
        public ObservableCollection<SelectableYear> YearOptions { get; } = new();
        public ObservableCollection<Person> StaffOptions { get; set; } = new();

        public WorkProgress WorkItem { get; set; } = new();

        public ICommand SaveCommand => new RelayCommand(_ => SaveWorkItem());
        public ICommand DeleteCommand => new RelayCommand(_ => DeleteWorkItem());
        public ICommand CompleteCommand => new RelayCommand(_ => MarkAsComplete());
        public WorkProgressViewModel()
        {
            WorkItem = new WorkProgress();
            WorkItem.PropertyChanged += WorkItem_PropertyChanged;

            using var db = new AppDbContext();
            var people = db.People.ToList();
            StaffOptions = new ObservableCollection<Person>(people);
            LoadWorkItems();
        }

        //Function handle Work Item change
        private void WorkItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WorkProgress.IsMonth))
            {
                if (WorkItem.IsMonth == true)
                    GenerateMonthOptions();
                else
                    MonthOptions.Clear();

            }
            else if (e.PropertyName == nameof(WorkProgress.IsYear) || e.PropertyName == nameof(WorkProgress.NotificationDate))
            {
                if (WorkItem.IsYear == true)
                    GenerateYearOptions();
                else
                    YearOptions.Clear();
            }
        }

        //Create Months selected
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

        //Create Years selected
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

        //Function handle Months Option
        private void GenerateMonthOptions()
        {
            MonthOptions.Clear();

            // Take start date notification, if date notification null return datetime now
            int startMonth = WorkItem.NotificationDate?.Month ?? DateTime.Now.Month;

            // Plus month after month now
            for (int i = startMonth + 1; i <= 12; i++)
            {
                MonthOptions.Add(new SelectableMonth
                {
                    Month = i,
                    MonthLabel = $"Tháng {i}"
                });
            }
        }


        //Function handle Years Option
        private void GenerateYearOptions()
        {
            YearOptions.Clear();
            int startYear = WorkItem.NotificationDate?.Year ?? DateTime.Now.Year;
            for (int i = 0; i < 10; i++)
            {
                YearOptions.Add(new SelectableYear { Year = startYear + i });
            }
        }


        //Show data to left form 
        private WorkProgress _selectedWorkItem;
        public WorkProgress SelectedWorkItem
        {
            get => _selectedWorkItem;
            set
            {
                _selectedWorkItem = value;
                OnPropertyChanged();

                if (_selectedWorkItem != null)
                {
                    // Set WorkItem for left form when selected
                    WorkItem = new WorkProgress
                    {
                        Id = _selectedWorkItem.Id,
                        Name = _selectedWorkItem.Name,
                        StartDate = _selectedWorkItem.StartDate,
                        Deadline = _selectedWorkItem.Deadline,
                        NotificationDate = _selectedWorkItem.NotificationDate,
                        ActualCompletionDate = _selectedWorkItem.ActualCompletionDate,
                        IsMonth = _selectedWorkItem.IsMonth,
                        IsYear = _selectedWorkItem.IsYear,
                        Is3Months = _selectedWorkItem.Is3Months,
                        Is6Months = _selectedWorkItem.Is6Months,
                        Is9Months = _selectedWorkItem.Is9Months,
                        IsYearly = _selectedWorkItem.IsYearly,
                        YearlyCount = _selectedWorkItem.YearlyCount,
                        IsSudden = _selectedWorkItem.IsSudden,
                        SuddenDate = _selectedWorkItem.SuddenDate,
                        IsSeminar = _selectedWorkItem.IsSeminar,
                        SeminarDate = _selectedWorkItem.SeminarDate,
                        Status = _selectedWorkItem.Status,
                        Priority = _selectedWorkItem.Priority,
                        Progress = _selectedWorkItem.Progress,
                        AssignerId = _selectedWorkItem.AssignerId,
                        PersonInChargeId = _selectedWorkItem.PersonInChargeId,
                        Months = _selectedWorkItem.Months,
                        Years = _selectedWorkItem.Years
                    };

                    // Synchronize Assigner and PersonInCharge from StaffOptions
                    WorkItem.Assigner = StaffOptions.FirstOrDefault(p => p.Id == WorkItem.AssignerId);
                    WorkItem.PersonInCharge = StaffOptions.FirstOrDefault(p => p.Id == WorkItem.PersonInChargeId);

                    OnPropertyChanged(nameof(WorkItem));

                    // Synchronize MonthOptions
                    GenerateMonthOptions();
                    foreach (var m in MonthOptions)
                        m.IsSelected = WorkItem.Months.Any(x => x.Month == m.Month);

                    // Synchronize YearOptions
                    GenerateYearOptions();
                    foreach (var y in YearOptions)
                        y.IsSelected = WorkItem.Years.Any(x => x.Year == y.Year);
                    WorkItem.PropertyChanged += WorkItem_PropertyChanged;

                }
            }
        }

        //Define search
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

        //Define notification
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

        //Load Work months and years
        private void LoadWorkItems()
        {
            using var db = new AppDbContext();
            var workList = db.WorkProgresses
                          .Include(w => w.Assigner)
                          .Include(w => w.PersonInCharge)
                          .Include(w => w.Months)
                          .Include(w => w.Years)
                          .ToList();

            foreach (var item in workList)
            {
                item.SelectedMonths = string.Join(", ", item.Months.Select(m => $"Tháng {m.Month}"));
                item.SelectedYears = string.Join(", ", item.Years.Select(y => $"Năm {y.Year}"));
            }

            WorkList = new ObservableCollection<WorkProgress>(workList);
            OnPropertyChanged(nameof(WorkList));
        }


        private void SaveWorkItem()
        {
            using var db = new AppDbContext();

            try
            {
                // Validate
                if (string.IsNullOrWhiteSpace(WorkItem.Name))
                {
                    Notify?.Invoke("Vui lòng nhập tên công việc trước khi lưu.", false);
                    return;
                }
                if (WorkItem.Assigner == null || WorkItem.Assigner.Id <= 0)
                {
                    Notify?.Invoke("Vui lòng chọn người giao việc trước khi lưu.", false);
                    return;
                }
                if (WorkItem.PersonInCharge == null || WorkItem.PersonInCharge.Id <= 0)
                {
                    Notify?.Invoke("Vui lòng chọn người phụ trách trước khi lưu.", false);
                    return;
                }
                if (WorkItem.Assigner == WorkItem.PersonInCharge)
                {
                    Notify?.Invoke("Người giao việc và người phụ trách không được trùng nhau", false);
                    return;
                }
                    if (string.IsNullOrWhiteSpace(WorkItem.Status))
                {
                    Notify?.Invoke("Vui lòng chọn trạng thái trước khi lưu.", false);
                    return;
                }
                if (string.IsNullOrWhiteSpace(WorkItem.Priority))
                {
                    Notify?.Invoke("Vui lòng chọn độ ưu tiên trước khi lưu.", false);
                    return;
                }

                // Check Assigner
                if (WorkItem.Assigner != null)
                {
                    if (WorkItem.Assigner.Id <= 0)
                    {
                        Notify?.Invoke("Người giao việc không hợp lệ (Id không tồn tại).", false);
                        return;
                    }
                    db.Attach(WorkItem.Assigner);
                    WorkItem.AssignerId = WorkItem.Assigner.Id;
                }
                else
                {
                    WorkItem.AssignerId = null;
                }

                if (WorkItem.PersonInCharge != null)
                {
                    if (WorkItem.PersonInCharge.Id <= 0)
                    {
                        Notify?.Invoke("Người phụ trách không hợp lệ (Id không tồn tại).", false);
                        return;
                    }
                    db.Attach(WorkItem.PersonInCharge);
                    WorkItem.PersonInChargeId = WorkItem.PersonInCharge.Id;
                }
                else
                {
                    WorkItem.PersonInChargeId = null;
                }

                // Save Dad workitem
                db.WorkProgresses.Add(WorkItem);
                db.SaveChanges(); // WorkItem.Id true value

                // Save month is choosed in checkbox
                foreach (var m in MonthOptions.Where(x => x.IsSelected))
                {
                    db.WorkProgressMonths.Add(new WorkProgressMonth
                    {
                        WorkProgressId = WorkItem.Id,
                        Month = m.Month
                    });
                }

                // List month from checkbox
                var manualYears = YearOptions.Where(x => x.IsSelected).Select(y => y.Year);

                // Year form county
                var autoYears = Enumerable.Empty<int>();
                if (WorkItem.IsYearly && WorkItem.YearlyCount > 0 && WorkItem.NotificationDate != null)
                {
                    int startYear = WorkItem.NotificationDate.Value.Year;
                    autoYears = Enumerable.Range(startYear, WorkItem.YearlyCount);
                }

                // Distinct if same data year
                var selectedYears = manualYears.Concat(autoYears).Distinct().ToList();

                // Save in DB
                foreach (var year in selectedYears)
                {
                    db.WorkProgressYears.Add(new WorkProgressYear
                    {
                        WorkProgressId = WorkItem.Id,
                        Year = year
                    });
                }

                db.SaveChanges();

                LoadWorkItems();
                Notify?.Invoke("Lưu tiến độ công việc thành công", true);
                CreateNewWorkItem();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Lỗi khi lưu tiến độ: {ex.InnerException?.Message ?? ex.Message}", false);
            }
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

        // When WorkItem.IsMonth change GenerateMonthOptions
        private void UpdateOptions()
        {
            if (WorkItem.IsMonth == true)
                GenerateMonthOptions();
            else
                MonthOptions.Clear();
            if (WorkItem.IsYear == true)
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
            public int Year
            {
                get; set;
            }
            private bool _isYear;
            public bool IsYear
            {
                get => _isYear; set
                {
                    _isYear = value;
                    OnPropertyChanged();
                }
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

            if (item.Is3Months == true)
                messages.Add($"Thông báo sau 3 tháng: {baseDate.AddMonths(3):dd/MM/yyyy}");
            if (item.Is6Months == true)
                messages.Add($"Thông báo sau 6 tháng: {baseDate.AddMonths(6):dd/MM/yyyy}");
            if (item.Is9Months == true)
                messages.Add($"Thông báo sau 9 tháng: {baseDate.AddMonths(9):dd/MM/yyyy}");
            if (item.IsYearly == true)
                messages.Add($"Nhắc hàng năm vào {baseDate:dd/MM}");

            if (item.IsSudden == true && item.SuddenDate != null)
                messages.Add($"Thông báo đột xuất vào ngày: {item.SuddenDate:dd/MM/yyyy}");

            if (item.IsSeminar == true && item.SeminarDate != null)
                messages.Add($"Thông báo chuyên đề: {item.SeminarDate:dd/MM/yyyy}");

            NotificationMessage = string.Join(" | ", messages);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
