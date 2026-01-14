using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using DocumentHub.Model; // để dùng Person

namespace DocumentHub.ViewModel
{
    public class WorkProgress : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        //Percen progress
        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? StartDate { get; set; }
        public DateTime? Deadline { get; set; }
        public int? AssignerId { get; set; }

        [ForeignKey("AssignerId")]
        public Person Assigner { get; set; }

        public int? PersonInChargeId { get; set; }

        [ForeignKey("PersonInChargeId")]
        public Person PersonInCharge { get; set; }

        public string Status { get; set; }
        public string Priority { get; set; }

        public DateTime? ActualCompletionDate { get; set; }

        private bool _isMonth;
        public bool IsMonth
        {
            get => _isMonth;
            set
            {
                _isMonth = value;
                OnPropertyChanged();
            }
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

        private DateTime? _notificationDate;

        public DateTime? NotificationDate
        {
            get => _notificationDate;
            set
            {
                _notificationDate = value;
                OnPropertyChanged();
            }
        }

        private bool _isYearly;
        public bool IsYearly
        {
            get => _isYearly;
            set
            {
                _isYearly = value;
                OnPropertyChanged();
            }
        }

        private int _yearlyCount;
        public int YearlyCount
        {
            get => _yearlyCount;
            set
            {
                _yearlyCount = value;
                OnPropertyChanged();
            }
        }

        private bool _is3Months;
        public bool Is3Months
        {
            get => _is3Months;
            set
            {
                _is3Months = value;
                OnPropertyChanged();
            }
        }

        private bool _is6Months;
        public bool Is6Months
        {
            get => _is6Months;
            set
            {
                _is6Months = value;
                OnPropertyChanged();
            }
        }

        private bool _is9Months;
        public bool Is9Months
        {
            get => _is9Months;
            set
            {
                _is9Months = value;
                OnPropertyChanged();
            }
        }

        private bool _is11Months;
        public bool Is11Months
        {
            get => _is11Months;
            set
            {
                _is11Months = value;
                OnPropertyChanged();
            }
        }

        private bool _isSudden;
        public bool IsSudden
        {
            get => _isSudden;
            set
            {
                _isSudden = value;
                OnPropertyChanged();
            }
        }

        private bool _isSeminar;
        public bool IsSeminar
        {
            get => _isSeminar;
            set
            {
                _isSeminar = value;
                OnPropertyChanged();
            }
        }

        [NotMapped]
        public string SelectedMonths { get; set; }

        [NotMapped]
        public string SelectedYears { get; set; }

        public DateTime? SeminarDate { get; set; }
        public DateTime? SuddenDate { get; set; }
        public string? MonthNotifyDate { get; set; }  
        public string? YearNotifyDate { get; set; }   
        public string QuarterNotifyDate { get; set; }



        // Navigation properties  EF know relation ship 1-more
        public ICollection<WorkProgressMonth> Months { get; set; } = new List<WorkProgressMonth>();
        public ICollection<WorkProgressQuater> Quarters { get; set; } = new List<WorkProgressQuater>();

        [NotMapped]
        public string SelectedQuarters
        {
            get; set;
        }

        public ICollection<WorkProgressYear> Years { get; set; } = new List<WorkProgressYear>();


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
