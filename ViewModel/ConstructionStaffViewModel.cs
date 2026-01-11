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
            LoadStaffList();
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
                Notify?.Invoke("Vui lòng nhập tên cán bộ xử lý.", false);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                if (SelectedStaff.Id > 0)
                {
                    var existing = db.ConstructionStaff.FirstOrDefault(x =>
                        x.Id == SelectedStaff.Id
                    );
                    if (existing != null)
                    {
                        existing.FullName = SelectedStaff.FullName;
                    }
                    else
                    {
                        db.ConstructionStaff.Add(
                            new ConstructionStaff { FullName = SelectedStaff.FullName }
                        );
                    }
                }
                else
                {
                    db.ConstructionStaff.Add(
                        new ConstructionStaff { FullName = SelectedStaff.FullName }
                    );
                }

                db.SaveChanges();
                if (SelectedStaff.Id > 0)
                    Notify?.Invoke("Sửa cán bộ thành công", true);
                else
                    Notify?.Invoke("Thêm cán bộ thành công", true);

                // Load List View Table from DB
                LoadStaffList();
                SelectedStaff = new ConstructionStaff();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu cán bộ: {ex.Message}", false);
            }
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
                SelectedStaff = new ConstructionStaff();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể xóa cán bộ: {ex.Message}", false);
            }
        }

        //  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
