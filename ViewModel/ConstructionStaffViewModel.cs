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
    public class ConstructionStaffViewModel : INotifyPropertyChanged
    {
        //Action call Notification
        public event Action<string, bool> Notify;

        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

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

        // List Staff
        public ObservableCollection<ConstructionStaff> StaffList { get; set; }

        public ConstructionStaffViewModel()
        {
            DeleteCommand = new RelayCommand(param => DeleteStaff(param as ConstructionStaff));
            SaveCommand = new RelayCommand(param => SaveStaff());
            SelectedStaff = new ConstructionStaff();
            OnPropertyChanged(nameof(SelectedStaff));
            //pagination
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
            var staffFromDb = db.ConstructionStaff.ToList();
            StaffList = new ObservableCollection<ConstructionStaff>(staffFromDb);
            OnPropertyChanged(nameof(StaffList));
        }

        /*Function Save*/
        private void SaveStaff()
        {
            if (SelectedStaff == null || string.IsNullOrWhiteSpace(SelectedStaff.FullName))
            {
                Notify?.Invoke("Vui lòng nhập tên cán bộ tiếp.", false);
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
                    var existing = db.ConstructionStaff.FirstOrDefault(x => x.Id == SelectedStaff.Id);
                    if (existing != null)
                    {
                        // Check name same name difference
                        bool isDuplicate = db.ConstructionStaff
                            .Any(x => x.Id != SelectedStaff.Id && x.FullName.Trim() == normalizedName);

                        if (isDuplicate)
                        {
                            Notify?.Invoke("Tên cán bộ đã tồn tại. Vui lòng chọn tên khác.", false);
                            return;
                        }

                        existing.FullName = normalizedName;
                    }
                }
                else
                {
                    // Add and check same name
                    bool isDuplicate = db.ConstructionStaff
                        .Any(x => x.FullName.Trim() == normalizedName);

                    if (isDuplicate)
                    {
                        Notify?.Invoke("Tên cán bộ đã tồn tại. Vui lòng chọn tên khác.", false);
                        return;
                    }

                    db.ConstructionStaff.Add(new ConstructionStaff { FullName = normalizedName });
                }

                db.SaveChanges();

                Notify?.Invoke(SelectedStaff.Id > 0 ? "Sửa cán bộ thành công" : "Thêm cán bộ thành công", true);

                LoadStaffList();
                ApplyFilter();
                SelectedStaff = new ConstructionStaff();
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

            // Filter keywork
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(s => s.FullName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }

            // Total page
            TotalPages = (int)Math.Ceiling((double)query.Count() / PageSize);

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages == 0 ? 1 : TotalPages;

            //pagination
            var paged = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            FilteredStaffList = new ObservableCollection<ConstructionStaff>(paged);
            OnPropertyChanged(nameof(FilteredStaffList));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPage));
        }



        /*Function Edit*/
        private void EditStaff(ConstructionStaff constructionStaff)
        {
            if (constructionStaff == null)
                return;
            try
            {
                using var db = new AppDbContext();
                var existing = db.ConstructionStaff.FirstOrDefault(x =>
                    x.Id == constructionStaff.Id
                );
                if (existing != null)
                {
                    existing.FullName = constructionStaff.FullName;
                    db.SaveChanges();
                    Notify?.Invoke("Sửa cán bộ thành công", true);
                }
                LoadStaffList();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể sửa cán bộ: {ex.Message}", false);
            }
        }

        /*Function Delete*/
        private void DeleteStaff(ConstructionStaff constructionStaff)
        {
            if (constructionStaff == null || constructionStaff.Id <= 0)
            {
                Notify?.Invoke("Không tìm thấy cán bộ để xóa.", false);
                return;
            }

            var confirm = MessageBox.Show(
                "Bạn có chắc muốn xóa cán bộ này?",
                "⭐Xác nhận",
                MessageBoxButton.YesNo
            );
            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var existing = db.ConstructionStaff.FirstOrDefault(x =>
                    x.Id == constructionStaff.Id
                );
                if (existing != null)
                {
                    db.ConstructionStaff.Remove(existing);
                    db.SaveChanges();
                    Notify?.Invoke($"Xóa cán bộ {constructionStaff.FullName} thành công", true);
                }
                else
                {
                    Notify?.Invoke("Không tìm thấy cán bộ trong cơ sở dữ liệu.", false);
                }

                LoadStaffList();
                ApplyFilter();
                SelectedStaff = new ConstructionStaff();
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
        public ObservableCollection<ConstructionStaff> FilteredStaffList { get; set; } = new();

        // Pagination
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                ApplyFilter();
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

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
