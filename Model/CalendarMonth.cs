using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentHub.Model
{
    public class CalendarMonth
    {
        public string MonthName { get; set; }
        public ObservableCollection<CalendarDay> Days { get; set; }
    }
}
