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

        // List Signer
        public ObservableCollection<Signer> SignerList
        {
            get; set;
        }

        public SignerViewModel()
        {
            SaveCommand = new RelayCommand(param => SaveSigner());
            DeleteCommand = new RelayCommand(param => DeleteSigner(param as Signer));
            EditCommand = new RelayCommand(param => EditSigner(param as Signer));
            SelectedSigner = new Signer();
            LoadSignerList();
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
                    else
                    {
                        db.Signers.Add(new Signer
                        {
                            FullName = SelectedSigner.FullName,
                            Position = SelectedSigner.Position,
                        });
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

                // Load List View Table from DB
                LoadSignerList();
                SelectedSigner = new Signer();
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
                SelectedSigner = new Signer();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa người ký: {ex.Message}", false);
            }
        }


        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
