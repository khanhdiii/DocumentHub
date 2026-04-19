using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using DocumentHub.Data;
using DocumentHub.Model;

namespace DocumentHub.ViewModel
{
    public class RecipientsViewModel : INotifyPropertyChanged
    {
        //Action call Notification
        public event Action<string, bool> Notify;

        public ICommand SaveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private Recipient _selectedRecipient;
        public Recipient SelectedRecipient
        {
            get => _selectedRecipient;
            set
            {
                _selectedRecipient = value;
                OnPropertyChanged(nameof(SelectedRecipient));
            }
        }

        // List Recipient
        public ObservableCollection<Recipient> RecipientList { get; set; }
            = new ObservableCollection<Recipient>();


        public RecipientsViewModel()
        {
            DeleteCommand = new RelayCommand(param => DeleteRecipient(param as Recipient));
            SaveCommand = new RelayCommand(param => SaveRecipient());
            EditCommand = new RelayCommand(param => EditRecipient(param as Recipient));
            SelectedRecipient = new Recipient();
            OnPropertyChanged(nameof(SelectedRecipient));
            //pagination
            GoToFirstPageCommand = new RelayCommand(_ => { CurrentPage = 1; });
            GoToLastPageCommand = new RelayCommand(_ => { CurrentPage = TotalPages == 0 ? 1 : TotalPages; });
            NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPages) CurrentPage++; });
            PreviousPageCommand = new RelayCommand(_ => { if (CurrentPage > 1) CurrentPage--; });
           
            LoadRecipientList();
            ApplyFilter();
        }


        /*Function Load*/
        private void LoadRecipientList()
        {
            using var db = new AppDbContext();
            var recipientsFromDb = db.Recipients.ToList();
            RecipientList = new ObservableCollection<Recipient>(recipientsFromDb);
            OnPropertyChanged(nameof(RecipientList));
        }

        /*Function Save*/
        private void SaveRecipient()
        {
            if (SelectedRecipient == null || string.IsNullOrWhiteSpace(SelectedRecipient.Name))
            {
                Notify?.Invoke("Vui lòng nhập nơi nhận tiếp.", false);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                // Nomalize name
                string normalizedName = SelectedRecipient.Name.Trim();

                if (SelectedRecipient.Id > 0)
                {
                    // Edit staff
                    var existing = db.Recipients.FirstOrDefault(x => x.Id == SelectedRecipient.Id);
                    if (existing != null)
                    {
                        // Check name same name difference
                        bool isDuplicate = db.Recipients
                            .Any(x => x.Id != SelectedRecipient.Id && x.Name.Trim() == normalizedName);

                        if (isDuplicate)
                        {
                            Notify?.Invoke("Tên cán bộ đã tồn tại. Vui lòng chọn tên khác.", false);
                            return;
                        }

                        existing.Name = normalizedName;
                    }
                }
                else
                {
                    // Add and check same name
                    bool isDuplicate = db.Recipients
                        .Any(x => x.Name.Trim() == normalizedName);

                    if (isDuplicate)
                    {
                        Notify?.Invoke("Tên nơi nhận đã tồn tại. Vui lòng chọn tên khác.", false);
                        return;
                    }

                   db.Recipients.Add(new Recipient { Name = normalizedName });
                }

                db.SaveChanges();

                Notify?.Invoke(SelectedRecipient.Id > 0 ? "Sửa nơi nhận thành công" : "Thêm cán bộ thành công", true);

                LoadRecipientList();
                ApplyFilter();
                SelectedRecipient = new Recipient();
                OnPropertyChanged(nameof(SelectedRecipient));

            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu cán bộ: {ex.Message}", false);
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

        private void ApplyFilter()
        {
            if (RecipientList == null)
                return;

            var query = RecipientList.AsQueryable();

            // Filter keywork
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(s => s.Name.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase));
            }


            // Total page
            TotalPages = (int)Math.Ceiling((double)query.Count() / PageSize);

            if (CurrentPage > TotalPages)
                _currentPage = TotalPages == 0 ? 1 : TotalPages;
            else if (CurrentPage < 1)
                _currentPage = 1;

            //paginationg
            var paged = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            FilteredRecipientList = new ObservableCollection<Recipient>(paged);
            OnPropertyChanged(nameof(FilteredRecipientList));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPage));
        }


        /*Function Edit*/
        private void EditRecipient(Recipient recipient)
        {
            if (recipient == null)
                return;
            try
            {
                using var db = new AppDbContext();
                var existing = db.Recipients.FirstOrDefault(x => x.Id == recipient.Id);
                if (existing != null)
                {
                    existing.Name = recipient.Name;
                    db.SaveChanges();
                    Notify?.Invoke("Sửa nơi nhận thành công", true);
                }
                LoadRecipientList();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể sửa nơi nhận: {ex.Message}", false);
            }
        }

        /*Function Delete*/
        private void DeleteRecipient(Recipient recipient)
        {
            if (recipient == null || recipient.Id <= 0)
            {
                Notify?.Invoke("Không tìm thấy nơi nhận để xóa.", false);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa nơi nhận này?", "⭐ Xác nhận", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var existing = db.Recipients.FirstOrDefault(x => x.Id == recipient.Id);
                if (existing != null)
                {
                    db.Recipients.Remove(existing);
                    db.SaveChanges();
                    Notify?.Invoke($"Xóa nơi nhận {recipient.Name} thành công", true);
                }
                else
                {
                    Notify?.Invoke("Không tìm thấy nơi nhận trong cơ sở dữ liệu.", false);
                }

                LoadRecipientList();
                ApplyFilter();
                SelectedRecipient = new Recipient();
                OnPropertyChanged(nameof(SelectedRecipient));
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa nơi nhận: {ex.Message}", false);
            }
        }

        // List after filter
        public ObservableCollection<Recipient> FilteredRecipientList { get; set; } = new();

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
