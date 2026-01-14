using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

using DocumentHub.Data;
using DocumentHub.Model;

using LiveCharts;
using LiveCharts.Wpf;

namespace DocumentHub.ViewModel
{
    public class AnalyticsViewModel : INotifyPropertyChanged
    {
        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set
            {
                _selectedYear = value;
                OnPropertyChanged();
                LoadDataForYear(_selectedYear);
            }
        }

        public SeriesCollection Series
        {
            get; set;
        }
        public string[] Labels
        {
            get; set;
        }
        public List<int> Years
        {
            get; set;
        }

        public int TotalIncoming
        {
            get; set;
        }
        public int TotalOutgoing
        {
            get; set;
        }
        public int TotalProgress
        {
            get; set;
        }

        public AnalyticsViewModel()
        {
            using (var db = new AppDbContext())
            {
                Years = db.WorkProgresses
                          .Where(w => w.StartDate.HasValue)
                          .Select(w => w.StartDate.Value.Year)
                          .Union(db.IncomingDocuments
                                   .Where(d => d.ArrivalDate.HasValue)
                                   .Select(d => d.ArrivalDate.Value.Year))
                          .Union(db.OutgoingDocuments
                                   .Where(d => d.DocumentDate.HasValue)
                                   .Select(d => d.DocumentDate.Value.Year))
                          .Distinct()
                          .OrderBy(y => y)
                          .ToList();
            }

            SelectedYear = Years.Any() ? Years.First() : DateTime.Now.Year;
            if (!Years.Any())
                Years = new List<int> { SelectedYear };
        }

        private void LoadDataForYear(int year)
        {
            using (var db = new AppDbContext())
            {
                var months = Enumerable.Range(1, 12).ToList();

                // Incoming theo tháng
                var incoming = db.IncomingDocuments
                    .Where(d => d.ArrivalDate.HasValue && d.ArrivalDate.Value.Year == year)
                    .GroupBy(d => d.ArrivalDate.Value.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .ToList();

                var incomingByMonth = months
                    .Select(m => incoming.FirstOrDefault(i => i.Month == m)?.Count ?? 0)
                    .ToList();

                // Outgoing theo tháng
                var outgoing = db.OutgoingDocuments
                    .Where(d => d.DocumentDate.HasValue && d.DocumentDate.Value.Year == year)
                    .GroupBy(d => d.DocumentDate.Value.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .ToList();

                var outgoingByMonth = months
                    .Select(m => outgoing.FirstOrDefault(o => o.Month == m)?.Count ?? 0)
                    .ToList();

                // Tiến độ: tính tổng Progress theo tháng (dựa vào ngày bắt đầu)
                var progress = db.WorkProgresses
                    .Where(w => w.StartDate.HasValue && w.StartDate.Value.Year == year)
                    .GroupBy(w => w.StartDate.Value.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() }) // đếm số công việc
                    .ToList();

                var progressByMonth = months
                    .Select(m => progress.FirstOrDefault(p => p.Month == m)?.Count ?? 0)
                    .ToList();


                // Gán vào Series
                Series = new SeriesCollection
{
                    new LineSeries
                    {
                        Title = "Văn bản đến",
                        Values = new ChartValues<int>(incomingByMonth),
                        Fill = new SolidColorBrush(Color.FromArgb(50, 33, 150, 243)) // xanh nhạt
                    },
                    new LineSeries
                    {
                        Title = "Văn bản đi",
                        Values = new ChartValues<int>(outgoingByMonth),
                        Fill = new SolidColorBrush(Color.FromArgb(50, 244, 67, 54)) // đỏ nhạt
                    },
                    new LineSeries
                    {
                        Title = "Tiến độ (số công việc)",
                        Values = new ChartValues<int>(progressByMonth),
                        Fill = new SolidColorBrush(Color.FromArgb(50, 76, 175, 80)) // xanh lá nhạt
                    }
                };



                Labels = months.Select(m => $"Tháng {m}").ToArray();

                TotalIncoming = incomingByMonth.Sum();
                TotalOutgoing = outgoingByMonth.Sum();
                TotalProgress = progressByMonth.Sum();

                OnPropertyChanged(nameof(Series));
                OnPropertyChanged(nameof(Labels));
                OnPropertyChanged(nameof(TotalIncoming));
                OnPropertyChanged(nameof(TotalOutgoing));
                OnPropertyChanged(nameof(TotalProgress));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
