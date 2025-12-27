using DocumentHub.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DocumentHub.ViewModel
{
    public class ReceivingOfficerViewModel : INotifyPropertyChanged
    {
        public ICommand SaveCommand { get; }

        public ReceivingOfficerViewModel()
        {
            SaveCommand = new RelayCommand(param => SaveStaffReceivingOfficer());
        }

        //  SelectedStaffReceivingOfficer
        private ReceivingOfficer _selectedStaffReceivingOfficer;
        public ReceivingOfficer SelectedStaffReceivingOfficer
        {
            get => _selectedStaffReceivingOfficer;
            set
            {
                _selectedStaffReceivingOfficer = value;
                OnPropertyChanged(nameof(SelectedStaffReceivingOfficer));
            }
        }

        // Data StaffList
        public ObservableCollection<ReceivingOfficer> StaffReceivingOfficerList { get; }
            = new ObservableCollection<ReceivingOfficer>
            {
                new ReceivingOfficer { Id = 1, FullName = "Nguyễn Nhận A" },
                new ReceivingOfficer { Id = 2, FullName = "Trần Nhận B" },
                new ReceivingOfficer { Id = 3, FullName = "Lê Nhận C" }
            };

        // Save Construction Staff
        private void SaveStaffReceivingOfficer()
        {
            var staffReceivingOfficer = StaffReceivingOfficerList.FirstOrDefault(s => s.Id == SelectedStaffReceivingOfficer?.Id);
            if (staffReceivingOfficer != null)
            {
                staffReceivingOfficer.FullName = SelectedStaffReceivingOfficer.FullName;
                staffReceivingOfficer.Position = SelectedStaffReceivingOfficer.Position;
                OnPropertyChanged(nameof(StaffReceivingOfficerList));
            }
        }

        //  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
