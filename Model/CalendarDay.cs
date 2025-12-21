using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentHub.Model
{
    public class CalendarDay
    {
        public int DayNumber { get; set; }
        public int Month { get; set; }
        public string TaskSummary { get; set; }
        public string TaskDetail { get; set; }
        public bool IsPlaceholder { get;  set; }
        public bool IsToday { get; set; }

    }

}
