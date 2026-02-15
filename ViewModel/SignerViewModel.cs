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
    public class SignerViewModel : INotifyPropertyChanged
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

        private Signer _selectedSigner;
        public Signer SelectedSigner
        {
            get => _selectedSigner;
            set
            {
                _selectedSigner = value;
                OnPropertyChanged(nameof(SelectedSigner));
            }
        }

        // Full list
        public ObservableCollection<Signer> SignerList
        {
            get; set;
        }

        // Filtered list (after search + pagination)
        public ObservableCollection<Signer> FilteredSignerList { get; set; } = new();

        // Search keyword
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

        public SignerViewModel()
        {
            SaveCommand = new RelayCommand(param => SaveSigner());
            DeleteCommand = new RelayCommand(param => DeleteSigner(param as Signer));
            EditCommand = new RelayCommand(param => EditSigner(param as Signer));
            SelectedSigner = new Signer();
            OnPropertyChanged(nameof(SelectedSigner));

            // Pagination commands
            GoToFirstPageCommand = new RelayCommand(_ => { CurrentPage = 1; ApplyFilter(); });
            GoToLastPageCommand = new RelayCommand(_ => { CurrentPage = TotalPages; ApplyFilter(); });
            NextPageCommand = new RelayCommand(_ => { if (CurrentPage < TotalPages) CurrentPage++; ApplyFilter(); });
            PreviousPageCommand = new RelayCommand(_ => { if (CurrentPage > 1) CurrentPage--; ApplyFilter(); });

            LoadSignerList();
            ApplyFilter();
        }

        /*Function Load*/
        private void LoadSignerList()
        {
            using var db = new AppDbContext();
            var signerFromDb = db.Signers.ToList();
            SignerList = new ObservableCollection<Signer>(signerFromDb);
            OnPropertyChanged(nameof(SignerList));
        }

        /*Function Save*/
        private void SaveSigner()
        {
            if (SelectedSigner == null || string.IsNullOrWhiteSpace(SelectedSigner.FullName))
            {
                Notify?.Invoke("Vui lòng nhập tên người ký.", false);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                if (SelectedSigner.Id > 0)
                {
                    var existing = db.Signers.FirstOrDefault(x => x.Id == SelectedSigner.Id);
                    if (existing != null)
                    {
                        existing.FullName = SelectedSigner.FullName;
                        existing.Position = SelectedSigner.Position;
                    }
                }
                else
                {
                    db.Signers.Add(new Signer
                    {
                        FullName = SelectedSigner.FullName,
                        Position = SelectedSigner.Position,
                    });
                }

                db.SaveChanges();

                if (SelectedSigner.Id > 0)
                    Notify?.Invoke("Sửa người ký thành công", true);
                else
                    Notify?.Invoke("Thêm người ký thành công", true);

                LoadSignerList();
                ApplyFilter();
                SelectedSigner = new Signer();
                OnPropertyChanged(nameof(SelectedSigner));
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu người ký: {ex.Message}", false);
            }
        }

        /*Function Edit*/
        private void EditSigner(Signer signer)
        {
            if (signer == null)
                return;
            try
            {
                using var db = new AppDbContext();
                var existing = db.Signers.FirstOrDefault(x => x.Id == signer.Id);
                if (existing != null)
                {
                    existing.FullName = signer.FullName;
                    existing.Position = signer.Position;
                    db.SaveChanges();
                    Notify?.Invoke("Sửa người ký thành công", true);
                }
                LoadSignerList();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể sửa người ký: {ex.Message}", false);
            }
        }

        /*Function Delete*/
        private void DeleteSigner(Signer signer)
        {
            if (signer == null || signer.Id <= 0)
            {
                Notify?.Invoke("Không tìm thấy người ký để xóa.", false);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa người ký này?", "⭐Xác nhận", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var existing = db.Signers.FirstOrDefault(x => x.Id == signer.Id);
                if (existing != null)
                {
                    db.Signers.Remove(existing);
                    db.SaveChanges();
                    Notify?.Invoke($"Xóa người ký {signer.FullName} thành công", true);
                }
                else
                {
                    Notify?.Invoke("Không tìm thấy người ký trong cơ sở dữ liệu.", false);
                }

                LoadSignerList();
                ApplyFilter();
                SelectedSigner = new Signer();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa người ký: {ex.Message}", false);
            }
        }

        /*Function Filter + Pagination*/
        private void ApplyFilter()
        {
            if (SignerList == null)
                return;

            var query = SignerList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                query = query.Where(s =>
                    (s.FullName != null && s.FullName.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    (s.Position != null && s.Position.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase))
                );
            }

            TotalPages = (int)Math.Ceiling((double)query.Count() / PageSize);

            if (CurrentPage > TotalPages)
            {
                _currentPage = TotalPages == 0 ? 1 : TotalPages;
            }
            else if (CurrentPage < 1)
            {
                _currentPage = 1;
            }

            var paged = query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            FilteredSignerList = new ObservableCollection<Signer>(paged);

            OnPropertyChanged(nameof(FilteredSignerList));
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CurrentPage));
        }



        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
