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

        // Pagination commands
        public ICommand GoToFirstPageCommand
        {
            get;
        }
        public ICommand GoToLastPageCommand
        {
            get;
        }
        public ICommand NextPageCommand
        {
            get;
        }
        public ICommand PreviousPageCommand
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

        // Full list
        public ObservableCollection<Person> StaffList
        {
            get; set;
        }

        // Filtered list (after search + pagination)
        public ObservableCollection<Person> FilteredStaffList { get; set; } = new();

        // Search keyword
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                CurrentPage = 1; 
                ApplyFilter();
            }
        }

        // Pagination properties
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                    ApplyFilter();
                }
            }
        }

        public int PageSize { get; set; } = 10;
        public int TotalPages
        {
            get; set;
        }

        public PersonViewModel()
        {
            DeleteCommand = new RelayCommand(param => DeleteStaff(param as Person));
            SaveCommand = new RelayCommand(param => SaveStaff());
            EditCommand = new RelayCommand(param => EditStaff(param as Person));
            SelectedStaff = new Person();

            // Pagination commands
            GoToFirstPageCommand = new RelayCommand(_ => { CurrentPage = 1; });
            GoToLastPageCommand = new RelayCommand(_ => { CurrentPage = TotalPages; });
            NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPages) CurrentPage++; });
            PreviousPageCommand = new RelayCommand(_ => { if (CurrentPage > 1) CurrentPage--; });

            LoadStaffList();
            ApplyFilter();
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
                    var newPerson = new Person
                    {
                        FullName = SelectedStaff.FullName
                    };
                    db.People.Add(newPerson);
                }

                db.SaveChanges();

                Notify?.Invoke(SelectedStaff.Id > 0 ? "Sửa cán bộ thành công" : "Thêm cán bộ thành công", true);

                LoadStaffList();
                ApplyFilter();
                SelectedStaff = new Person();
                OnPropertyChanged(nameof(SelectedStaff));
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu cán bộ: {ex.Message}", false);
            }
        }

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
                ApplyFilter();
                SelectedStaff = new Person();
                OnPropertyChanged(nameof(SelectedStaff));
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa cán bộ: {ex.Message}", false);
            }
        }

        private void ApplyFilter()
        {
            if (StaffList == null)
                return;

            var query = StaffList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(s => s.FullName != null &&
                    s.FullName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            TotalPages = (int)Math.Ceiling((double)query.Count() / PageSize);

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages == 0 ? 1 : TotalPages;
            if (CurrentPage < 1)
                CurrentPage = 1;

            var paged = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            FilteredStaffList = new ObservableCollection<Person>(paged);
            OnPropertyChanged(nameof(FilteredStaffList));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPage));
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
