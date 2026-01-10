using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace DocumentHub.ViewModel
{
    public class WorkProgress : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get; set;
        }

        private string _name;
        public string Name
        {
            get => _name; set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public DateTime? StartDate
        {
            get; set;
        }
        public DateTime? Deadline
        {
            get; set;
        }

        public class Person
        {
            public string FullName
            {
                get; set;
            }
        }
        public Person Assigner
        {
            get; set;
        }
        public Person PersonInCharge
        {
            get; set;
        }

        public string Status
        {
            get; set;
        }
        public string Priority
        {
            get; set;
        }
        public int Progress
        {
            get; set;
        }
        public DateTime? ActualCompletionDate
        {
            get; set;
        }

        // IMPORTANT:  PropertyChanged
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


        private bool _is3Months;
        public bool Is3Months
        {
            get => _is3Months; set
            {
                _is3Months = value;
                OnPropertyChanged();
            }
        }

        private bool _is6Months;
        public bool Is6Months
        {
            get => _is6Months; set
            {
                _is6Months = value;
                OnPropertyChanged();
            }
        }

        private bool _is9Months;
        public bool Is9Months
        {
            get => _is9Months; set
            {
                _is9Months = value;
                OnPropertyChanged();
            }
        }

        private bool _isSudden;
        public bool IsSudden
        {
            get => _isSudden; set
            {
                _isSudden = value;
                OnPropertyChanged();
            }
        }

        private bool _isSeminar;
        public bool IsSeminar
        {
            get => _isSeminar; set
            {
                _isSeminar = value;
                OnPropertyChanged();
            }
        }

        public DateTime? SeminarDate
        {
            get; set;
        }
        public DateTime? SuddenDate
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string n = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
