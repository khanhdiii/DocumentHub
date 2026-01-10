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
            SelectedRecipient = new Recipient();
            LoadRecipientList();
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
                Notify?.Invoke("Vui lòng nhập tên nơi nhận.", false);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                if (SelectedRecipient.Id > 0)
                {
                    var existing = db.Recipients.FirstOrDefault(x => x.Id == SelectedRecipient.Id);
                    if (existing != null)
                    {
                        existing.Name = SelectedRecipient.Name;
                    }
                    else
                    {
                        db.Recipients.Add(new Recipient
                        {
                            Name = SelectedRecipient.Name,
                        });
                    }
                }
                else
                {
                    db.Recipients.Add(new Recipient
                    {
                        Name = SelectedRecipient.Name,
                    });
                }

                db.SaveChanges();
                if (SelectedRecipient.Id > 0)
                    Notify?.Invoke("Sửa nơi nhận thành công", true);
                else
                    Notify?.Invoke("Thêm nơi nhận thành công", true);

                // Load List View Table from DB
               LoadRecipientList ();
                SelectedRecipient = new Recipient();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu nơi nhận: {ex.Message}", false);
            }
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
                SelectedRecipient = new Recipient();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa nơi nhận: {ex.Message}", false);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
