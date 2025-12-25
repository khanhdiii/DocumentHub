using LiveCharts;
using LiveCharts.Wpf;

namespace DocumentHub.ViewModel
{
    public class AnalyticsViewModel
    {
        public SeriesCollection Series { get; set; }

        public AnalyticsViewModel()
        {
            Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Văn bản đến",
                    Values = new ChartValues<int> { 12, 15, 20, 18 }
                },
                new LineSeries
                {
                    Title = "Văn bản đi",
                    Values = new ChartValues<int> { 10, 12, 18, 22 }
                }
            };
        }
    }
}
