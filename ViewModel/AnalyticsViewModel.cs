using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DocumentHub.Model;

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

        //Define chart
        public SeriesCollection Series { get; set; }
        public string[] Labels { get; set; }
        public List<int> Years { get; set; }

        //Sum Doc
        public int TotalIncoming { get; set; }
        public int TotalOutgoing { get; set; }

        private Dictionary<int, List<Analytics>> _dataByYear;

        public AnalyticsViewModel()
        {
            // Data test
            _dataByYear = new Dictionary<int, List<Analytics>>
            {
                {
                    2024, new List<Analytics>
                    {
                        new Analytics { Month = 1, IncomingDocument = 12, OutgoingDocument = 10 },
                        new Analytics { Month = 2, IncomingDocument = 15, OutgoingDocument = 12 },
                        new Analytics { Month = 3, IncomingDocument = 20, OutgoingDocument = 18 },
                        new Analytics { Month = 4, IncomingDocument = 18, OutgoingDocument = 22 },
                        new Analytics { Month = 5, IncomingDocument = 25, OutgoingDocument = 20 },
                        new Analytics { Month = 6, IncomingDocument = 30, OutgoingDocument = 28 },
                        new Analytics { Month = 7, IncomingDocument = 22, OutgoingDocument = 24 },
                        new Analytics { Month = 8, IncomingDocument = 27, OutgoingDocument = 26 },
                        new Analytics { Month = 9, IncomingDocument = 35, OutgoingDocument = 30 },
                        new Analytics { Month = 10, IncomingDocument = 40, OutgoingDocument = 38 },
                        new Analytics { Month = 11, IncomingDocument = 32, OutgoingDocument = 35 },
                        new Analytics { Month = 12, IncomingDocument = 28, OutgoingDocument = 25 }
                    }
                },
                {
                    2025, new List<Analytics>
                    {
                        new Analytics { Month = 1, IncomingDocument = 10, OutgoingDocument = 8 },
                        new Analytics { Month = 2, IncomingDocument = 14, OutgoingDocument = 11 },
                        new Analytics { Month = 3, IncomingDocument = 19, OutgoingDocument = 15 },
                        new Analytics { Month = 4, IncomingDocument = 21, OutgoingDocument = 20 },
                        new Analytics { Month = 5, IncomingDocument = 26, OutgoingDocument = 22 },
                        new Analytics { Month = 6, IncomingDocument = 33, OutgoingDocument = 29 },
                        new Analytics { Month = 7, IncomingDocument = 24, OutgoingDocument = 23 },
                        new Analytics { Month = 8, IncomingDocument = 29, OutgoingDocument = 27 },
                        new Analytics { Month = 9, IncomingDocument = 36, OutgoingDocument = 31 },
                        new Analytics { Month = 10, IncomingDocument = 42, OutgoingDocument = 39 },
                        new Analytics { Month = 11, IncomingDocument = 34, OutgoingDocument = 36 },
                        new Analytics { Month = 12, IncomingDocument = 30, OutgoingDocument = 26 }
                    }
                }
            };

            Years = _dataByYear.Keys.ToList();
            //Default First year in combobox
            SelectedYear = Years.First(); 
        }

        private void LoadDataForYear(int year)
        {
            var data = _dataByYear[year];

            Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Văn bản đến",
                    Values = new ChartValues<int>(data.Select(d => d.IncomingDocument))
                },
                new LineSeries
                {
                    Title = "Văn bản đi",
                    Values = new ChartValues<int>(data.Select(d => d.OutgoingDocument))
                }
            };

            Labels = data.Select(d => $"Tháng {d.Month}").ToArray();
            
            //Function Sum Docs
            TotalIncoming = data.Sum(d => d.IncomingDocument); 
            TotalOutgoing = data.Sum(d => d.OutgoingDocument);
            
            OnPropertyChanged(nameof(Series));
            OnPropertyChanged(nameof(Labels));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
