using DocumentHub.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DocumentHub.ViewModel
{
    public class ConstructionStaffViewModel : INotifyPropertyChanged
    {
        public ICommand SaveCommand { get; }

        public ConstructionStaffViewModel()
        {
            SaveCommand = new RelayCommand(param => SaveStaff());
        }

        //  SelectedStaff
        private ConstructionStaff _selectedStaff;
        public ConstructionStaff SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
                OnPropertyChanged(nameof(SelectedStaff));
            }
        }

        // Data StaffList
        public ObservableCollection<ConstructionStaff> StaffList { get; }
            = new ObservableCollection<ConstructionStaff>
            {
                new ConstructionStaff { Id = 1, FullName = "Nguyễn Văn A" },
                new ConstructionStaff { Id = 2, FullName = "Trần Thị B" },
                new ConstructionStaff { Id = 3, FullName = "Lê Văn C" }
            };

        // Save Construction Staff
        private void SaveStaff()
        {
            var staff = StaffList.FirstOrDefault(s => s.Id == SelectedStaff?.Id);
            if (staff != null)
            {
                staff.FullName = SelectedStaff.FullName;
                staff.Position = SelectedStaff.Position;
                OnPropertyChanged(nameof(StaffList));
            }
        }

        //  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
