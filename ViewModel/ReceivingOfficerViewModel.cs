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
    public class ReceivingOfficerViewModel : INotifyPropertyChanged
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

        private ReceivingOfficer _selectedStaff;

        public ReceivingOfficer SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
                OnPropertyChanged(nameof(SelectedStaff));
            }
        }
        public ObservableCollection<ReceivingOfficer> StaffList
        {
            get; set;
        }

        public ReceivingOfficerViewModel()
        {
            DeleteCommand = new RelayCommand(param => DeleteStaff(param as ReceivingOfficer));
            SaveCommand = new RelayCommand(param => SaveStaff());
            EditCommand = new RelayCommand(param => EditStaff(param as ReceivingOfficer));
            SelectedStaff = new ReceivingOfficer();
            OnPropertyChanged(nameof(SelectedStaff));
            
            //pagination
            GoToFirstPageCommand = new RelayCommand(_ => { CurrentPage = 1; ApplyFilter(); });
            GoToLastPageCommand = new RelayCommand(_ => { CurrentPage = TotalPages == 0 ? 1 : TotalPages; ApplyFilter(); });
            NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPages) CurrentPage++; ApplyFilter(); });
            PreviousPageCommand = new RelayCommand(_ => { if (CurrentPage > 1) CurrentPage--; ApplyFilter(); });

            LoadStaffList();
            ApplyFilter();
        }

        /*Function Load*/
        private void LoadStaffList()
        {
            using var db = new AppDbContext();
            var staffFromDb = db.ReceivingOfficers.ToList();
            StaffList = new ObservableCollection<ReceivingOfficer>(staffFromDb);
            OnPropertyChanged(nameof(StaffList));

        }

        /*Function Save*/
        private void SaveStaff()
        {

            if (SelectedStaff == null || string.IsNullOrWhiteSpace(SelectedStaff.FullName))
            {
                Notify?.Invoke("Vui lòng nhập tên cán bộ tiếp nhận.", false);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                // Nomalize name
                string normalizedName = SelectedStaff.FullName.Trim();

                if (SelectedStaff.Id > 0)
                {
                    // Edit staff
                    var existing = db.ReceivingOfficers.FirstOrDefault(x => x.Id == SelectedStaff.Id);
                    if (existing != null)
                    {
                        // Check name same name difference
                        bool isDuplicate = db.ReceivingOfficers
                           .Any(x => x.Id != SelectedStaff.Id && x.FullName.Trim() == normalizedName);
                        if (isDuplicate)
                        {
                            Notify?.Invoke("Tên cán bộ đã tồn tại. Vui lòng chọn tên khác.", false);
                            return;
                        }
                        existing.FullName = SelectedStaff.FullName;
                    }
                }
                else
                {
                    // Add and check same name
                    bool isDuplicate = db.ReceivingOfficers
                       .Any(x => x.FullName.Trim() == normalizedName);
                    if (isDuplicate)
                    {
                        Notify?.Invoke("Tên cán bộ đã tồn tại. Vui lòng chọn tên khác.", false);
                        return;
                    }
                    db.ReceivingOfficers.Add(new ReceivingOfficer
                    {
                        FullName = SelectedStaff.FullName,
                    });
                }

                db.SaveChanges();

                if (SelectedStaff.Id > 0)
                    Notify?.Invoke("Sửa cán bộ thành công", true);
                else
                    Notify?.Invoke("Thêm cán bộ thành công", true);

                // Load List View Table from DB
                LoadStaffList();
                ApplyFilter();
                SelectedStaff = new ReceivingOfficer();
                OnPropertyChanged(nameof(SelectedStaff));

            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu cán bộ: {ex.Message}", false);
            }
        }


        private void ApplyFilter()
        {
            if (StaffList == null)
                return;

            var query = StaffList.AsQueryable();

            //Filter Name
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(s => s.FullName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            // Total page
            TotalPages = (int)Math.Ceiling((double)query.Count() / PageSize);

            if (CurrentPage > TotalPages)
                _currentPage = TotalPages == 0 ? 1 : TotalPages;
            else if (CurrentPage < 1)
                _currentPage = 1;

            // Take date page
            var paged = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            FilteredStaffList = new ObservableCollection<ReceivingOfficer>(paged);

            OnPropertyChanged(nameof(FilteredStaffList));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPage));
        }

        /*Function Edit*/
        private void EditStaff(ReceivingOfficer receivingOfficer)
        {
            if (receivingOfficer == null)
                return;
            SelectedStaff = new ReceivingOfficer { Id = receivingOfficer.Id, FullName = receivingOfficer.FullName };
            try
            {
                using var db = new AppDbContext();
                var existing = db.ReceivingOfficers.FirstOrDefault(x => x.Id == receivingOfficer.Id);
                if (existing != null)
                {
                    existing.FullName = receivingOfficer.FullName;
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
        private void DeleteStaff(ReceivingOfficer receivingOfficer)
        {
            if (receivingOfficer == null || receivingOfficer.Id <= 0)
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
                var existing = db.ReceivingOfficers.FirstOrDefault(x => x.Id == receivingOfficer.Id);
                if (existing != null)
                {
                    db.ReceivingOfficers.Remove(existing);
                    db.SaveChanges();
                    Notify?.Invoke($"Xóa cán bộ {receivingOfficer.FullName} thành công", true);
                }
                else
                {
                    Notify?.Invoke("Không tìm thấy cán bộ trong cơ sở dữ liệu.", false);
                }

                LoadStaffList();
                ApplyFilter();
                SelectedStaff = new ReceivingOfficer();
                OnPropertyChanged(nameof(SelectedStaff));

            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa cán bộ: {ex.Message}", false);
            }
        }

        // Filter keyword
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                _searchKeyword = value;
                OnPropertyChanged(nameof(SearchKeyword));
                ApplyFilter();
            }
        }

        // List after filter
        public ObservableCollection<ReceivingOfficer> FilteredStaffList { get; set; } = new();

        // Pagination
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value)
                    return;
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }



        public int PageSize { get; set; } = 10;
        public int TotalPages
        {
            get; set;
        }

        // Command pagination
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

        //  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
