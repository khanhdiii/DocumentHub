using DocumentHub.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace DocumentHub.ViewModel
{
    public enum ViewMode { Week, Month, Quarter }

    public class DashboardViewModel : INotifyPropertyChanged
    {
        public event Action RequestScrollToToday;
        public ObservableCollection<CalendarDay> CalendarDays { get; set; }
        public ObservableCollection<CalendarMonth> CalendarMonths { get; set; }
        public ObservableCollection<ViewModeItem> ViewModes { get; set; }

        public ICommand PreviousPeriodCommand { get; }
        public ICommand NextPeriodCommand { get; }

        private DateTime _currentDate;
        private ViewMode _selectedViewMode;
        public ViewMode SelectedViewMode
        {
            get => _selectedViewMode;
            set
            {
                if (_selectedViewMode != value)
                {
                    _selectedViewMode = value;

                    if (_selectedViewMode == ViewMode.Quarter)
                    {
                        _currentDate = DateTime.Today;
                        LoadCalendar();
                        OnPropertyChanged(nameof(CurrentPeriodDisplay));

                        // Event scroll
                        RequestScrollToToday?.Invoke();
                    }
                    else
                    {
                        LoadCalendar();
                        OnPropertyChanged(nameof(CurrentPeriodDisplay));
                    }

                    OnPropertyChanged(nameof(SelectedViewMode));
                }
            }
        }

        public string CurrentPeriodDisplay
        {
            get
            {
                switch (SelectedViewMode)
                {
                    case ViewMode.Week:
                        var dayOfWeek = _currentDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)_currentDate.DayOfWeek;
                        var startOfWeek = _currentDate.AddDays(-dayOfWeek + 1);
                        var endOfWeek = startOfWeek.AddDays(6);
                        return $"Tuần {startOfWeek:dd/MM} - {endOfWeek:dd/MM}";

                    case ViewMode.Month:
                        return _currentDate.ToString("MMMM yyyy");

                    case ViewMode.Quarter:
                        return $"Quý {((_currentDate.Month - 1) / 3) + 1} {_currentDate.Year}";

                    default:
                        return "";
                }
            }
        }

        public DashboardViewModel()
        {
            _currentDate = DateTime.Today;
            ViewModes = new ObservableCollection<ViewModeItem>
            {
                new ViewModeItem { Display = "Tuần", Value = ViewMode.Week },
                new ViewModeItem { Display = "Tháng", Value = ViewMode.Month },
                new ViewModeItem { Display = "Quý", Value = ViewMode.Quarter }
            };
            SelectedViewMode = ViewMode.Month;
            PreviousPeriodCommand = new RelayCommand(_ => ChangePeriod(-1));
            NextPeriodCommand = new RelayCommand(_ => ChangePeriod(1));
            LoadCalendar();
        }
        private void ChangePeriod(int offset)
        {
            switch (SelectedViewMode)
            {
                case ViewMode.Week: _currentDate = _currentDate.AddDays(offset * 7); break;
                case ViewMode.Month: _currentDate = _currentDate.AddMonths(offset); break;
                case ViewMode.Quarter: _currentDate = _currentDate.AddMonths(offset * 3); break;
            }
            OnPropertyChanged(nameof(CurrentPeriodDisplay));
            LoadCalendar();
        }

        //Funtion handle calendar
        private void LoadCalendar()
        {
            CalendarDays = new ObservableCollection<CalendarDay>();
            CalendarMonths = new ObservableCollection<CalendarMonth>();

            if (SelectedViewMode == ViewMode.Month)
            {
                var firstDay = new DateTime(_currentDate.Year, _currentDate.Month, 1);
                int offset = ((int)firstDay.DayOfWeek + 6) % 7;

                for (int i = 0; i < offset; i++)
                {
                    var date = firstDay.AddDays(-(offset - i));
                    CalendarDays.Add(new CalendarDay
                    {
                        DayNumber = date.Day,
                        Month = date.Month,
                        TaskSummary = GetTaskSummary(date),
                        TaskDetail = GetTaskDetail(date),
                        IsPlaceholder = true,
                        IsToday = date.Date == DateTime.Today
                    });
                }

                int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
                for (int i = 1; i <= daysInMonth; i++)
                {
                    var date = new DateTime(_currentDate.Year, _currentDate.Month, i);
                    CalendarDays.Add(new CalendarDay
                    {
                        DayNumber = i,
                        Month = date.Month,
                        TaskSummary = GetTaskSummary(date),
                        TaskDetail = GetTaskDetail(date),
                        IsPlaceholder = false,
                        IsToday = date.Date == DateTime.Today
                    });
                }
            }
            else if (SelectedViewMode == ViewMode.Week)
            {
                var dayOfWeek = _currentDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)_currentDate.DayOfWeek;
                var startOfWeek = _currentDate.AddDays(-dayOfWeek + 1);

                for (int i = 0; i < 7; i++)
                {
                    var date = startOfWeek.AddDays(i);
                    CalendarDays.Add(new CalendarDay
                    {
                        DayNumber = date.Day,
                        Month = date.Month,
                        TaskSummary = GetTaskSummary(date),
                        TaskDetail = GetTaskDetail(date),
                        IsPlaceholder = false,
                        IsToday = date.Date == DateTime.Today
                    });
                }
            }
            else if (SelectedViewMode == ViewMode.Quarter)
            {
                int year = _currentDate.Year;
                int quarter = ((_currentDate.Month - 1) / 3) + 1;  
                int startMonth = (quarter - 1) * 3 + 1;             

                for (int m = 0; m < 3; m++)
                {
                    int month = startMonth + m;
                    var monthDays = new ObservableCollection<CalendarDay>();
                    int daysInMonth = DateTime.DaysInMonth(year, month);

                    for (int d = 1; d <= daysInMonth; d++)
                    {
                        var date = new DateTime(year, month, d);
                        monthDays.Add(new CalendarDay
                        {
                            DayNumber = d,
                            Month = date.Month,
                            TaskSummary = GetTaskSummary(date),
                            TaskDetail = GetTaskDetail(date),
                            IsPlaceholder = false,
                            IsToday = date.Date == DateTime.Today
                        });
                    }

                    CalendarMonths.Add(new CalendarMonth
                    {
                        MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy"),
                        Days = monthDays
                    });
                }
            }



            OnPropertyChanged(nameof(CalendarDays));
            OnPropertyChanged(nameof(CalendarMonths));
        }

        private string GetTaskSummary(DateTime date) => "• 2 tasks";
        private string GetTaskDetail(DateTime date) => $"Chi tiết công việc ngày {date:dd/MM}: \n- Họp nhóm\n- Gửi báo cáo";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    }
}
