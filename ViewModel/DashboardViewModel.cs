using DocumentHub.Data;
using DocumentHub.Model;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace DocumentHub.ViewModel
{
    public enum ViewMode
    {
        Week, Month, Quarter
    }

    public class DashboardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CalendarDay> CalendarDays
        {
            get; set;
        }
        public ObservableCollection<CalendarMonth> CalendarMonths
        {
            get; set;
        }
        public ObservableCollection<ViewModeItem> ViewModes
        {
            get; set;
        }

        public ICommand PreviousPeriodCommand
        {
            get;
        }
        public ICommand NextPeriodCommand
        {
            get;
        }
        public event Action RequestScrollToToday;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


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
                    _currentDate = DateTime.Today;
                    LoadCalendar();
                    OnPropertyChanged(nameof(CurrentPeriodDisplay));
                    OnPropertyChanged(nameof(SelectedViewMode));
                }
            }
        }

        public string CurrentPeriodDisplay => SelectedViewMode switch
        {
            ViewMode.Week => $"Tuần {_currentDate.AddDays(-(int)_currentDate.DayOfWeek + 1):dd/MM} - {_currentDate.AddDays(7 - (int)_currentDate.DayOfWeek):dd/MM}",
            ViewMode.Month => _currentDate.ToString("MMMM yyyy"),
            ViewMode.Quarter => $"Quý {((_currentDate.Month - 1) / 3) + 1} {_currentDate.Year}",
            _ => ""
        };

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
            _currentDate = SelectedViewMode switch
            {
                ViewMode.Week => _currentDate.AddDays(offset * 7),
                ViewMode.Month => _currentDate.AddMonths(offset),
                ViewMode.Quarter => _currentDate.AddMonths(offset * 3),
                _ => _currentDate
            };
            OnPropertyChanged(nameof(CurrentPeriodDisplay));
            LoadCalendar();
        }

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
                    CalendarDays.Add(CreateCalendarDay(date, true));
                }

                int daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
                for (int i = 1; i <= daysInMonth; i++)
                {
                    var date = new DateTime(_currentDate.Year, _currentDate.Month, i);
                    CalendarDays.Add(CreateCalendarDay(date, false));
                }
            }

            else if (SelectedViewMode == ViewMode.Week)
            {
                var startOfWeek = _currentDate.AddDays(-(int)_currentDate.DayOfWeek + 1);
                for (int i = 0; i < 7; i++)
                {
                    var date = startOfWeek.AddDays(i);
                    CalendarDays.Add(CreateCalendarDay(date, false));
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
                    var firstDay = new DateTime(year, month, 1);
                    int offset = ((int)firstDay.DayOfWeek + 6) % 7;

                    // Add placeholder first month
                    for (int i = 0; i < offset; i++)
                    {
                        var date = firstDay.AddDays(-(offset - i));
                        monthDays.Add(CreateCalendarDay(date, true));
                    }

                    int daysInMonth = DateTime.DaysInMonth(year, month);
                    for (int d = 1; d <= daysInMonth; d++)
                    {
                        var date = new DateTime(year, month, d);

                        // If date 29, 30, 31 và month is not enough -> placeholder
                        bool isPlaceholder = d > daysInMonth;
                        monthDays.Add(CreateCalendarDay(date, isPlaceholder));
                    }

                    // Add placeholder last month
                    int totalCells = offset + daysInMonth;
                    int remainder = totalCells % 7;
                    if (remainder != 0)
                    {
                        for (int i = 0; i < 7 - remainder; i++)
                        {
                            var date = firstDay.AddDays(daysInMonth + i);
                            monthDays.Add(CreateCalendarDay(date, true));
                        }
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

        private CalendarDay CreateCalendarDay(DateTime date, bool isPlaceholder)
        {
            return new CalendarDay
            {
                DayNumber = date.Day,
                Month = date.Month,
                TaskSummary = GetTaskSummary(date),
                TaskDetail = GetTaskDetail(date),
                IsPlaceholder = isPlaceholder,
                IsToday = date.Date == DateTime.Today,
                BackgroundBrush = GetTaskColor(date)
            };
        }

        private List<DateTime> ParseDates(string dateString)
        {
            return dateString?
                .Split(',')
                .Select(s => DateTime.TryParse(s.Trim(), out var d) ? d : (DateTime?)null)
                .Where(d => d.HasValue)
                .Select(d => d.Value)
                .ToList() ?? new List<DateTime>();
        }

        private string GetTaskSummary(DateTime date)
        {
            using var db = new AppDbContext();
            var allTasks = db.WorkProgresses.ToList();

            var tasks = allTasks
                .Where(w =>
                    ParseDates(w.MonthNotifyDate).Contains(date) ||
                    ParseDates(w.QuarterNotifyDate).Any(d => d.Date == date.Date) ||
                    ParseDates(w.YearNotifyDate).Contains(date) ||
                    (w.IsSudden && w.SuddenDate.HasValue && w.SuddenDate.Value.Date == date.Date) ||
                    (w.IsSeminar && w.SeminarDate.HasValue && w.SeminarDate.Value.Date == date.Date))
                .ToList();

            return $"• {tasks.Count} task{(tasks.Count > 1 ? "s" : "")}";
        }


        private string GetTaskDetail(DateTime date)
        {
            using var db = new AppDbContext();

            var allTasks = db.WorkProgresses.ToList();

            var tasks = allTasks
                .Where(w =>
                    ParseDates(w.MonthNotifyDate).Contains(date) ||
                    ParseDates(w.QuarterNotifyDate).Any(d => d.Date == date.Date) ||
                    ParseDates(w.YearNotifyDate).Contains(date) ||
                    (w.IsSudden && w.SuddenDate.HasValue && w.SuddenDate.Value.Date == date.Date) ||
                    (w.IsSeminar && w.SeminarDate.HasValue && w.SeminarDate.Value.Date == date.Date))
                .ToList();


            if (!tasks.Any())
                return $"Không có công việc nào vào ngày {date:dd/MM}";

            var lines = tasks.Select(t =>
            {
                string type =
                    ParseDates(t.MonthNotifyDate).Contains(date) ? "Thông báo tháng" :
                    ParseDates(t.QuarterNotifyDate).Any(d => d.Date == date.Date) ? "Thông báo quý" :
                    ParseDates(t.YearNotifyDate).Contains(date) ? "Thông báo năm" :
                    (t.IsSudden && t.SuddenDate.HasValue && t.SuddenDate.Value.Date == date.Date) ? "Đột xuất" :
                    (t.IsSeminar && t.SeminarDate.HasValue && t.SeminarDate.Value.Date == date.Date) ? "Chuyên đề" :
                    "Khác";

                return $"- {t.Name} ({type}, {t.Status}, {t.Priority}, {t.Progress}%)";
            });

            return $"Chi tiết công việc ngày {date:dd/MM}:\n" + string.Join("\n", lines);
        }


        private Brush GetTaskColor(DateTime date)
        {
            using var db = new AppDbContext();

            var tasks = db.WorkProgresses.ToList();

            var matchedTasks = tasks.Where(t =>
                ParseDates(t.MonthNotifyDate).Contains(date) ||
                ParseDates(t.QuarterNotifyDate).Any(d => d.Date == date.Date) ||
                ParseDates(t.YearNotifyDate).Contains(date) ||
                (t.IsSudden && t.SuddenDate.HasValue && t.SuddenDate.Value.Date == date.Date) ||
                (t.IsSeminar && t.SeminarDate.HasValue && t.SeminarDate.Value.Date == date.Date)).ToList();

            if (!matchedTasks.Any())
                return Brushes.White;

            // If complete
            if (matchedTasks.All(t => t.Status == "Hoàn thành"))
                return Brushes.LightGreen;

            // If task in next 5 date
            if (matchedTasks.Any(t =>
                 ParseDates(t.MonthNotifyDate).Any(d => d.Date == date.Date && (DateTime.Today - d).TotalDays > 5 && t.Status != "Hoàn thành") ||
                 ParseDates(t.QuarterNotifyDate).Any(d => d.Date == date.Date && (DateTime.Today - d).TotalDays > 5 && t.Status != "Hoàn thành") ||
                 ParseDates(t.YearNotifyDate).Any(d => d.Date == date.Date && (DateTime.Today - d).TotalDays > 5 && t.Status != "Hoàn thành") ||
                 (t.IsSudden && t.SuddenDate.HasValue && t.SuddenDate.Value.Date == date.Date && (DateTime.Today - t.SuddenDate.Value).TotalDays > 5 && t.Status != "Hoàn thành") ||
                 (t.IsSeminar && t.SeminarDate.HasValue && t.SeminarDate.Value.Date == date.Date && (DateTime.Today - t.SeminarDate.Value).TotalDays > 5 && t.Status != "Hoàn thành")
             ))
                return Brushes.IndianRed;

            return Brushes.LightGoldenrodYellow;

            // If date notifiction > 5 date and not complete
            if (matchedTasks.Any(t =>
                ParseDates(t.MonthNotifyDate).Any(d => d.Date == date.Date && (DateTime.Today - d).TotalDays > 5 && t.Status != "Hoàn thành") ||
                ParseDates(t.QuarterNotifyDate).Any(d => d.Date == date.Date && (DateTime.Today - d).TotalDays > 5 && t.Status != "Hoàn thành") ||
                ParseDates(t.YearNotifyDate).Any(d => d.Date == date.Date && (DateTime.Today - d).TotalDays > 5 && t.Status != "Hoàn thành") ||
                (t.IsSudden && t.SuddenDate.HasValue && t.SuddenDate.Value.Date == date.Date && (DateTime.Today - t.SuddenDate.Value).TotalDays > 5 && t.Status != "Hoàn thành") ||
                (t.IsSeminar && t.SeminarDate.HasValue && t.SeminarDate.Value.Date == date.Date && (DateTime.Today - t.SeminarDate.Value).TotalDays > 5 && t.Status != "Hoàn thành")
            ))
                return Brushes.IndianRed;

            return Brushes.IndianRed;

            return Brushes.White;
        }

    }
}
