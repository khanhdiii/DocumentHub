using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
        //List original
        public ObservableCollection<WorkProgress> WorkList { get; set; } = new();
        public ObservableCollection<string> StatusOptions { get; set; } = new() { "Chưa bắt đầu", "Đang thực hiện", "Hoàn thành", "Tạm hoãn" };
        public ObservableCollection<string> PriorityOptions { get; set; } = new() { "Thấp", "Trung bình", "Cao", "Khẩn cấp" };
        public ObservableCollection<SelectableMonth> MonthOptions { get; } = new();
        public ObservableCollection<SelectableYear> YearOptions { get; } = new();
        public ObservableCollection<Person> StaffOptions { get; set; } = new();
        public ObservableCollection<WorkProgress> WeeklyWorkList
        {
            get; set;
        }
        //List filters
        public ObservableCollection<WorkProgress> FilteredWorkList { get; set; } = new();

        public WorkProgress WorkItem { get; set; } = new();

        public ICommand SaveCommand => new RelayCommand(_ => SaveWorkItem());
        public ICommand EditCommand
        {
            get;
        }
        public ICommand DeleteCommand
        {
            get;
        }

        public ICommand CompleteCommand => new RelayCommand(_ => MarkAsComplete());

        public ICommand ApplyFilterCommand
        {
            get;
        }
        public ICommand ClearFilterCommand
        {
            get;
        }
        public ICommand GoToFirstPageCommand
        {
            get;
        }
        public ICommand GoToLastPageCommand
        {
            get;
        }
        public WorkProgressViewModel()
        {
            WorkItem = new WorkProgress();
            WorkItem.PropertyChanged += WorkItem_PropertyChanged;

            using var db = new AppDbContext();
            var people = db.People.ToList();
            StaffOptions = new ObservableCollection<Person>(people);
            LoadWorkItems();
            ApplyFilter();

            // Create command
            EditCommand = new RelayCommand(_ => EditWorkItem());
            DeleteCommand = new RelayCommand(_ => DeleteWorkItem());
            ApplyFilterCommand = new RelayCommand(_ => ApplyFilter());
            ClearFilterCommand = new RelayCommand(_ => ClearFilter());
            GoToFirstPageCommand = new RelayCommand(_ => GoToFirstPage());
            GoToLastPageCommand = new RelayCommand(_ => GoToLastPage());
        }


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

        /*Function generate Month*/
        private void GenerateMonthOptions()
        {
            MonthOptions.Clear();
            int startMonth = WorkItem.NotificationDate?.Month ?? DateTime.Now.Month;

            // Create 12 months in checkbox
            for (int i = startMonth; i <= 12; i++)
            {
                MonthOptions.Add(new SelectableMonth
                {
                    Month = i,
                    MonthLabel = $"Tháng {i}"
                });
            }
        }

        /*Function generate Year*/
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

                if (_selectedWorkItem != null)
                {
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
                        Years = _selectedWorkItem.Years,
                        MonthNotifyDate = _selectedWorkItem.MonthNotifyDate,
                        YearNotifyDate = _selectedWorkItem.YearNotifyDate
                    };

                    WorkItem.Assigner = StaffOptions.FirstOrDefault(p => p.Id == WorkItem.AssignerId);
                    WorkItem.PersonInCharge = StaffOptions.FirstOrDefault(p => p.Id == WorkItem.PersonInChargeId);

                    OnPropertyChanged(nameof(WorkItem));

                    GenerateMonthOptions();
                    foreach (var m in MonthOptions)
                        m.IsSelected = WorkItem.Months.Any(x => x.Month == m.Month);

                    GenerateYearOptions();
                    foreach (var y in YearOptions)
                        y.IsSelected = WorkItem.Years.Any(x => x.Year == y.Year);

                    WorkItem.PropertyChanged += WorkItem_PropertyChanged;
                }
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

        public void LoadWorkItems()
        {
            using var db = new AppDbContext();
            var workList = db.WorkProgresses
                             .Include(w => w.Assigner)
                             .Include(w => w.PersonInCharge)
                             .Include(w => w.Months)
                             .Include(w => w.Years)
                             .Include(w => w.Quarters)
                             .ToList();

            foreach (var item in workList)
            {
                // Set Months Notify Date if null
                item.MonthNotifyDate = string.IsNullOrEmpty(item.MonthNotifyDate) ? "Không có" : item.MonthNotifyDate;
                item.YearNotifyDate = string.IsNullOrEmpty(item.YearNotifyDate) ? "Không có" : item.YearNotifyDate;
                item.QuarterNotifyDate = string.IsNullOrEmpty(item.QuarterNotifyDate) ? "Không có" : item.QuarterNotifyDate;

                //Show list Months is choosed
                if (item.Months != null && item.Months.Any())
                    item.SelectedMonths = string.Join(", ", item.Months.Select(m => $"Tháng {m.Month}"));
                else
                    item.SelectedMonths = "Không có";

                //Show list Years is choosed
                if (item.Years != null && item.Years.Any())
                    item.SelectedYears = string.Join(", ", item.Years.Select(y => $"Năm {y.Year}"));
                else
                    item.SelectedYears = "Không có";

                //Show list Quarters is choosed
                if (item.Quarters != null && item.Quarters.Any())
                    item.SelectedQuarters = string.Join(", ", item.Quarters.Select(q => $"Quý {q.Quarter}"));
                else
                    item.SelectedQuarters = "Không có";
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
                if ((WorkItem.Assigner?.Id ?? WorkItem.AssignerId) is null or <= 0)
                {
                    Notify?.Invoke("Vui lòng chọn người giao việc.", false);
                    return;
                }
                if ((WorkItem.PersonInCharge?.Id ?? WorkItem.PersonInChargeId) is null or <= 0)
                {
                    Notify?.Invoke("Vui lòng chọn người phụ trách.", false);
                    return;
                }
                if (string.IsNullOrWhiteSpace(WorkItem.Status))
                {
                    Notify?.Invoke("Vui lòng chọn trạng thái.", false);
                    return;
                }
                if (string.IsNullOrWhiteSpace(WorkItem.Priority))
                {
                    Notify?.Invoke("Vui lòng chọn độ ưu tiên.", false);
                    return;
                }

                // Caculate
                DateTime baseDate = WorkItem.NotificationDate ?? DateTime.Now;

                var monthDates = MonthOptions
                    .Where(x => x.IsSelected)
                    .Select(m =>
                    {
                        int year = m.Month < baseDate.Month ? baseDate.Year + 1 : baseDate.Year;
                        int day = Math.Min(baseDate.Day, DateTime.DaysInMonth(year, m.Month));
                        return new DateTime(year, m.Month, day).ToString("dd/MM/yyyy");
                    }).ToList();
                WorkItem.MonthNotifyDate = string.Join(", ", monthDates);

                var yearDates = YearOptions
                    .Where(x => x.IsSelected)
                    .Select(y =>
                    {
                        int day = Math.Min(baseDate.Day, DateTime.DaysInMonth(y.Year, baseDate.Month));
                        return new DateTime(y.Year, baseDate.Month, day).ToString("dd/MM/yyyy");
                    }).ToList();


                var quarterDates = new List<string>();

                void AddQuarter(int offset)
                {
                    var date = baseDate.AddMonths(offset);
                    var month = date.Month;
                    var label = month switch
                    {
                        <= 3 => "Quý 1",
                        <= 6 => "Quý 2",
                        <= 9 => "Quý 3",
                        _ => "Quý 4"
                    };
                    quarterDates.Add($"{date:dd/MM/yyyy} ({label})");
                }

                if (WorkItem.Is3Months)
                    AddQuarter(3);
                if (WorkItem.Is6Months)
                    AddQuarter(6);
                if (WorkItem.Is9Months)
                    AddQuarter(9);
                if (WorkItem.Is11Months)
                    AddQuarter(11);


                WorkItem.QuarterNotifyDate = string.Join(", ", quarterDates);
                WorkItem.YearNotifyDate = string.Join(", ", yearDates);

                var assignerId = WorkItem.Assigner?.Id ?? WorkItem.AssignerId;
                var picId = WorkItem.PersonInCharge?.Id ?? WorkItem.PersonInChargeId;

                if (WorkItem.Id == 0)
                {
                    var entity = new WorkProgress
                    {
                        Name = WorkItem.Name,
                        StartDate = WorkItem.StartDate,
                        Deadline = WorkItem.Deadline,
                        NotificationDate = WorkItem.NotificationDate,
                        ActualCompletionDate = WorkItem.ActualCompletionDate,
                        Status = WorkItem.Status,
                        Priority = WorkItem.Priority,
                        Progress = WorkItem.Progress,
                        AssignerId = assignerId,
                        PersonInChargeId = picId,
                        IsMonth = WorkItem.IsMonth,
                        IsYear = WorkItem.IsYear,
                        Is3Months = WorkItem.Is3Months,
                        Is6Months = WorkItem.Is6Months,
                        Is9Months = WorkItem.Is9Months,
                        Is11Months = WorkItem.Is11Months,
                        IsYearly = WorkItem.IsYearly,
                        YearlyCount = WorkItem.YearlyCount,
                        IsSudden = WorkItem.IsSudden,
                        SuddenDate = WorkItem.SuddenDate,
                        IsSeminar = WorkItem.IsSeminar,
                        SeminarDate = WorkItem.SeminarDate,
                        MonthNotifyDate = WorkItem.MonthNotifyDate,
                        YearNotifyDate = WorkItem.YearNotifyDate,
                        QuarterNotifyDate = WorkItem.QuarterNotifyDate
                    };

                    db.WorkProgresses.Add(entity);
                    db.SaveChanges();

                    foreach (var m in MonthOptions.Where(x => x.IsSelected))
                        db.WorkProgressMonths.Add(new WorkProgressMonth { WorkProgressId = entity.Id, Month = m.Month });

                    foreach (var y in YearOptions.Where(x => x.IsSelected))
                        db.WorkProgressYears.Add(new WorkProgressYear { WorkProgressId = entity.Id, Year = y.Year });

                    if (WorkItem.Is3Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = entity.Id, Quarter = 1 });
                    if (WorkItem.Is6Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = entity.Id, Quarter = 2 });
                    if (WorkItem.Is9Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = entity.Id, Quarter = 3 });
                    if (WorkItem.Is11Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = entity.Id, Quarter = 4 });
                }
                else
                {
                    var existing = db.WorkProgresses
                        .Include(w => w.Months)
                        .Include(w => w.Years)
                        .Include(w => w.Quarters)
                        .FirstOrDefault(w => w.Id == WorkItem.Id);

                    if (existing == null)
                    {
                        Notify?.Invoke("Không tìm thấy công việc để cập nhật.", false);
                        return;
                    }

                    existing.Name = WorkItem.Name;
                    existing.StartDate = WorkItem.StartDate;
                    existing.Deadline = WorkItem.Deadline;
                    existing.NotificationDate = WorkItem.NotificationDate;
                    existing.ActualCompletionDate = WorkItem.ActualCompletionDate;
                    existing.Status = WorkItem.Status;
                    existing.Priority = WorkItem.Priority;
                    existing.Progress = WorkItem.Progress;
                    existing.AssignerId = assignerId;
                    existing.PersonInChargeId = picId;
                    existing.IsMonth = WorkItem.IsMonth;
                    existing.IsYear = WorkItem.IsYear;
                    existing.Is3Months = WorkItem.Is3Months;
                    existing.Is6Months = WorkItem.Is6Months;
                    existing.Is9Months = WorkItem.Is9Months;
                    existing.Is11Months = WorkItem.Is11Months;
                    existing.IsYearly = WorkItem.IsYearly;
                    existing.YearlyCount = WorkItem.YearlyCount;
                    existing.IsSudden = WorkItem.IsSudden;
                    existing.SuddenDate = WorkItem.SuddenDate;
                    existing.IsSeminar = WorkItem.IsSeminar;
                    existing.SeminarDate = WorkItem.SeminarDate;
                    existing.MonthNotifyDate = WorkItem.MonthNotifyDate;
                    existing.YearNotifyDate = WorkItem.YearNotifyDate;
                    existing.QuarterNotifyDate = WorkItem.QuarterNotifyDate;


                    db.WorkProgressMonths.RemoveRange(existing.Months);
                    db.WorkProgressYears.RemoveRange(existing.Years);
                    db.WorkProgressQuaters.RemoveRange(existing.Quarters);

                    foreach (var m in MonthOptions.Where(x => x.IsSelected))
                        db.WorkProgressMonths.Add(new WorkProgressMonth { WorkProgressId = existing.Id, Month = m.Month });

                    foreach (var y in YearOptions.Where(x => x.IsSelected))
                        db.WorkProgressYears.Add(new WorkProgressYear { WorkProgressId = existing.Id, Year = y.Year });

                    if (WorkItem.Is3Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = existing.Id, Quarter = 1 });
                    if (WorkItem.Is6Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = existing.Id, Quarter = 2 });
                    if (WorkItem.Is9Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = existing.Id, Quarter = 3 });
                    if (WorkItem.Is11Months)
                        db.WorkProgressQuaters.Add(new WorkProgressQuater { WorkProgressId = existing.Id, Quarter = 4 });
                }

                db.SaveChanges();
                LoadWorkItems();
                ApplyFilter();

                Notify?.Invoke("Lưu công việc thành công", true);
                CreateNewWorkItem();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Lỗi khi lưu: {ex.InnerException?.Message ?? ex.Message}", false);
            }
        }



        private void EditWorkItem()
        {
            using var db = new AppDbContext();

            try
            {
                var existing = db.WorkProgresses
                                 .Include(w => w.Months)
                                 .Include(w => w.Years)
                                 .FirstOrDefault(w => w.Id == WorkItem.Id);

                if (existing == null)
                {
                    Notify?.Invoke("Không tìm thấy công việc để chỉnh sửa.", false);
                    return;
                }

                var assignerId = WorkItem.Assigner?.Id ?? WorkItem.AssignerId;
                var picId = WorkItem.PersonInCharge?.Id ?? WorkItem.PersonInChargeId;

                existing.Name = WorkItem.Name;
                existing.StartDate = WorkItem.StartDate;
                existing.Deadline = WorkItem.Deadline;
                existing.NotificationDate = WorkItem.NotificationDate;
                existing.ActualCompletionDate = WorkItem.ActualCompletionDate;
                existing.Status = WorkItem.Status;
                existing.Priority = WorkItem.Priority;
                existing.Progress = WorkItem.Progress;
                existing.AssignerId = assignerId;
                existing.PersonInChargeId = picId;

                existing.Is3Months = WorkItem.Is3Months;
                existing.Is6Months = WorkItem.Is6Months;
                existing.Is9Months = WorkItem.Is9Months;
                existing.IsYearly = WorkItem.IsYearly;
                existing.YearlyCount = WorkItem.YearlyCount;
                existing.IsSudden = WorkItem.IsSudden;
                existing.SuddenDate = WorkItem.SuddenDate;
                existing.IsSeminar = WorkItem.IsSeminar;
                existing.SeminarDate = WorkItem.SeminarDate;

                if (WorkItem.NotificationDate != null)
                {
                    DateTime baseDate = WorkItem.NotificationDate.Value;

                    // Year
                    var yearDates = YearOptions
                        .Where(x => x.IsSelected)
                        .Select(y =>
                        {
                            var day = Math.Min(baseDate.Day, DateTime.DaysInMonth(y.Year, baseDate.Month));
                            return new DateTime(y.Year, baseDate.Month, day).ToString("dd/MM/yyyy");
                        })
                        .ToList();
                    existing.YearNotifyDate = string.Join(", ", yearDates);

                    // Quarter Dates
                    var quarterDates = new List<string>();
                    if (WorkItem.Is3Months)
                        quarterDates.Add($"{baseDate.AddMonths(3):dd/MM/yyyy} (Quý 1)");
                    if (WorkItem.Is6Months)
                        quarterDates.Add($"{baseDate.AddMonths(6):dd/MM/yyyy} (Quý 2)");
                    if (WorkItem.Is9Months)
                        quarterDates.Add($"{baseDate.AddMonths(9):dd/MM/yyyy} (Quý 3)");
                    if (WorkItem.Is11Months)
                        quarterDates.Add($"{baseDate.AddMonths(11):dd/MM/yyyy} (Quý 4)");

                    existing.QuarterNotifyDate = string.Join(", ", quarterDates);
                }

                db.WorkProgressMonths.RemoveRange(existing.Months);
                db.WorkProgressYears.RemoveRange(existing.Years);

                foreach (var m in MonthOptions.Where(x => x.IsSelected))
                {
                    db.WorkProgressMonths.Add(new WorkProgressMonth
                    {
                        WorkProgressId = existing.Id,
                        Month = m.Month
                    });
                }
                foreach (var y in YearOptions.Where(x => x.IsSelected))
                {
                    db.WorkProgressYears.Add(new WorkProgressYear
                    {
                        WorkProgressId = existing.Id,
                        Year = y.Year
                    });
                }

                db.SaveChanges();
                LoadWorkItems();
                ApplyFilter();

                Notify?.Invoke("Cập nhật công việc thành công", true);
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Lỗi khi chỉnh sửa: {ex.InnerException?.Message ?? ex.Message}", false);
            }
        }

        private void DeleteWorkItem()
        {
            if (SelectedWorkItem == null)
            {
                Notify?.Invoke("Vui lòng chọn công việc để xóa.", false);
                return;
            }

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa công việc này?",
                "Xác nhận", MessageBoxButton.YesNo);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var existing = db.WorkProgresses
                                 .Include(w => w.Months)
                                 .Include(w => w.Years)
                                 .FirstOrDefault(w => w.Id == SelectedWorkItem.Id);

                if (existing == null)
                {
                    Notify?.Invoke("Không tìm thấy công việc để xóa.", false);
                    return;
                }

                db.WorkProgressMonths.RemoveRange(existing.Months);
                db.WorkProgressYears.RemoveRange(existing.Years);
                db.WorkProgresses.Remove(existing);
                db.SaveChanges();

                // Update datagrid
                WorkList.Remove(SelectedWorkItem);
                ApplyFilter(); 

                Notify?.Invoke("Xóa công việc thành công", true);
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Lỗi khi xóa: {ex.InnerException?.Message ?? ex.Message}", false);
            }
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
                get => _isSelected;
                set
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
                get => _isYear;
                set
                {
                    _isYear = value;
                    OnPropertyChanged();
                }
            }
            private bool _isSelected;
            public bool IsSelected
            {
                get => _isSelected;
                set
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


        /* Function pagination*/
        // Pagination
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                UpdatePagedWorkList();
            }
        }

        private int _itemsPerPage = 10;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set
            {
                _itemsPerPage = value;
                OnPropertyChanged();
                UpdatePagedWorkList();
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
            }
        }
        private void GoToFirstPage()
        {
            if (TotalPages > 0)
            {
                CurrentPage = 1;
                UpdatePagedWorkList();
            }
        }

        private void GoToLastPage()
        {
            if (TotalPages > 0)
            {
                CurrentPage = TotalPages;
                UpdatePagedWorkList();
            }
        }


        public ObservableCollection<WorkProgress> PagedWorkList { get; set; } = new();


        //Function update pagination
        private void UpdatePagedWorkList()
        {
            // Sum TotalPages
            TotalPages = (int)Math.Ceiling((double)FilteredWorkList.Count / ItemsPerPage);

            var items = FilteredWorkList
                .Skip((CurrentPage - 1) * ItemsPerPage)
                .Take(ItemsPerPage)
                .ToList();

            PagedWorkList.Clear();
            foreach (var item in items)
                PagedWorkList.Add(item);

            OnPropertyChanged(nameof(PagedWorkList));
        }



        //Change Page
        public ICommand PreviousPageCommand => new RelayCommand(_ =>
        {
            if (CurrentPage > 1)
                CurrentPage--;
        });

        public ICommand NextPageCommand => new RelayCommand(_ =>
        {
            if ((CurrentPage * ItemsPerPage) < FilteredWorkList.Count)
                CurrentPage++;
        });




        /*Search*/
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                ApplyFilter();
            }
        }
        //Function split Date
        private IEnumerable<DateTime> ParseDates(string dateString)
        {
            var dates = new List<DateTime>();
            if (string.IsNullOrWhiteSpace(dateString))
                return dates;

            var parts = dateString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var clean = part.Trim();

                var datePart = clean.Split(' ')[0];

                if (DateTime.TryParseExact(datePart, "dd/MM/yyyy", null,
                    System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    dates.Add(dt.Date);
                }
            }

            return dates;
        }


        //Function Filter
        private void ApplyFilter()
        {
            IEnumerable<WorkProgress> query = WorkList;

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(w =>
                    (w.Name?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true) ||
                    (w.Status?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true) ||
                    (w.Priority?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true));
            }

            // Filter date (chỉ các ngày thông báo: năm, tháng, quý, đột xuất, chuyên đề)
            if (SearchDateFrom.HasValue && SearchDateTo.HasValue)
            {
                DateTime from = SearchDateFrom.Value.Date;
                DateTime to = SearchDateTo.Value.Date;

                query = query.Where(w =>
                    // MonthNotifyDate, QuarterNotifyDate, YearNotifyDate là string => cần parse
                    (!string.IsNullOrEmpty(w.MonthNotifyDate) &&
                     ParseDates(w.MonthNotifyDate).Any(d => d >= from && d <= to)) ||

                    (!string.IsNullOrEmpty(w.QuarterNotifyDate) &&
                     ParseDates(w.QuarterNotifyDate).Any(d => d >= from && d <= to)) ||

                    (!string.IsNullOrEmpty(w.YearNotifyDate) &&
                     ParseDates(w.YearNotifyDate).Any(d => d >= from && d <= to)) ||

                    (w.SuddenDate.HasValue && w.SuddenDate.Value.Date >= from && w.SuddenDate.Value.Date <= to) ||
                    (w.SeminarDate.HasValue && w.SeminarDate.Value.Date >= from && w.SeminarDate.Value.Date <= to)
                );
            }

            FilteredWorkList = new ObservableCollection<WorkProgress>(query);

            OnPropertyChanged(nameof(FilteredWorkList));
            CurrentPage = 1;
            UpdatePagedWorkList();
        }

        private void ClearFilter()
        {
            SearchKeyword = string.Empty;
            SearchDateFrom = null;
            SearchDateTo = null;

            // Reset list WorkList
            FilteredWorkList = new ObservableCollection<WorkProgress>(WorkList);

            OnPropertyChanged(nameof(SearchKeyword));
            OnPropertyChanged(nameof(SearchDateFrom));
            OnPropertyChanged(nameof(SearchDateTo));
            OnPropertyChanged(nameof(FilteredWorkList));

            CurrentPage = 1;
            UpdatePagedWorkList();
        }

        public void LoadWeeklyWork()
        {
            var startOfWeek = DateTime.Now.Date.AddDays(-(int)DateTime.Now.DayOfWeek + 1);
            var endOfWeek = startOfWeek.AddDays(6);
            WeeklyWorkList = new ObservableCollection<WorkProgress>(WorkList.Where(w =>
                (w.StartDate.HasValue && w.StartDate.Value.Date >= startOfWeek && w.StartDate.Value.Date <= endOfWeek) ||
                (w.Deadline.HasValue && w.Deadline.Value.Date >= startOfWeek && w.Deadline.Value.Date <= endOfWeek) ||
                (w.NotificationDate.HasValue && w.NotificationDate.Value.Date >= startOfWeek && w.NotificationDate.Value.Date <= endOfWeek) ||
                (w.SeminarDate.HasValue && w.SeminarDate.Value.Date >= startOfWeek && w.SeminarDate.Value.Date <= endOfWeek) ||
                (w.SuddenDate.HasValue && w.SuddenDate.Value.Date >= startOfWeek && w.SuddenDate.Value.Date <= endOfWeek)));
        }

        public DateTime? SearchDateFrom
        {
            get; set;
        }
        public DateTime? SearchDateTo
        {
            get; set;
        }



        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
