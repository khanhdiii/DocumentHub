using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentHub.ViewModel;

namespace DocumentHub.Model
{
    public class WorkProgressYear
    {
        [Key]
        public int Id { get; set; }
        public int Year { get; set; }
        public int WorkProgressId { get; set; }

        [ForeignKey("WorkProgressId")]
        public WorkProgress WorkProgress { get; set; }
    }
}
