using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using DocumentHub.Data;
using DocumentHub.Model;

namespace DocumentHub.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        //Action call Notification
        public event Action<string, bool> Notify;

        public ICommand SaveCommand
        {
            get;
        }
        public ICommand EditCommand
        {
            get;
        }
        public ICommand DeleteCommand
        {
            get;
        }

        private Person _selectedStaff;
        public Person SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
                OnPropertyChanged(nameof(SelectedStaff));
            }
        }

        // List Staff
        public ObservableCollection<Person> StaffList
        {
            get; set;
        }

        public PersonViewModel()
        {
            DeleteCommand = new RelayCommand(param => DeleteStaff(param as Person));
            SaveCommand = new RelayCommand(param => SaveStaff());
            SelectedStaff = new Person();
            LoadStaffList();
        }

        /*Function Load*/
        private void LoadStaffList()
        {
            using var db = new AppDbContext();
            var staffFromDb = db.People.ToList();
            StaffList = new ObservableCollection<Person>(staffFromDb);
            OnPropertyChanged(nameof(StaffList));
        }

        private void SaveStaff()
        {
            if (SelectedStaff == null || string.IsNullOrWhiteSpace(SelectedStaff.FullName))
            {
                Notify?.Invoke("Vui lòng nhập tên cán bộ.", false);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                if (SelectedStaff.Id > 0)
                {
                    // Sửa cán bộ
                    var existing = db.People.FirstOrDefault(x => x.Id == SelectedStaff.Id);
                    if (existing != null)
                    {
                        existing.FullName = SelectedStaff.FullName;
                    }
                    else
                    {
                        Notify?.Invoke("Không tìm thấy cán bộ để sửa.", false);
                        return;
                    }
                }
                else
                {
                    // Thêm mới cán bộ (KHÔNG gán Id)
                    var newPerson = new Person
                    {
                        FullName = SelectedStaff.FullName
                    };
                    db.People.Add(newPerson);
                }

                db.SaveChanges();

                Notify?.Invoke(SelectedStaff.Id > 0 ? "Sửa cán bộ thành công" : "Thêm cán bộ thành công", true);

                LoadStaffList();
                SelectedStaff = new Person(); // Reset để tránh giữ lại Id cũ
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu cán bộ: {ex.Message}", false);
            }
        }


        /*Function Edit*/
        private void EditStaff(Person person)
        {
            if (person == null)
                return;
            try
            {
                using var db = new AppDbContext();
                var existing = db.People.FirstOrDefault(x => x.Id == person.Id);
                if (existing != null)
                {
                    existing.FullName = person.FullName;
                    db.SaveChanges();
                    Notify?.Invoke("Sửa cán bộ thành công", true);
                }
                LoadStaffList();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể sửa cán bộ: {ex.Message}", false);
            }
        }

        /*Function Delete*/
        private void DeleteStaff(Person person)
        {
            if (person == null || person.Id <= 0)
            {
                Notify?.Invoke("Không tìm thấy cán bộ để xóa.", false);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa cán bộ này?", "⭐Xác nhận", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var existing = db.People.FirstOrDefault(x => x.Id == person.Id);
                if (existing != null)
                {
                    db.People.Remove(existing);
                    db.SaveChanges();
                    Notify?.Invoke($"Xóa cán bộ {person.FullName} thành công", true);
                }
                else
                {
                    Notify?.Invoke("Không tìm thấy cán bộ trong cơ sở dữ liệu.", false);
                }

                LoadStaffList();
                SelectedStaff = new Person();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa cán bộ: {ex.Message}", false);
            }
        }


        //  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
