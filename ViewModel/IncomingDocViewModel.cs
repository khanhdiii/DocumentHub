using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using ClosedXML.Excel;

using DocumentHub.Data;
using DocumentHub.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace DocumentHub.ViewModel
{
    public class IncomingDocViewModel : INotifyPropertyChanged
    {
        // 🔔 Notification event
        public event Action<string, bool> Notify;

        // Commands
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
        public ICommand ExportExcelCommand
        {
            get;
        }
        public ICommand NextPageCommand
        {
            get;
        }
        public ICommand PrevPageCommand
        {
            get;
        }
        public ICommand GoToFirstPageCommand
        {
            get;
        }
        public ICommand GoToLastPageCommand
        {
            get;
        }

        // Data collections
        //Call Data from database
        public ObservableCollection<ConstructionStaff> StaffList
        {
            get;
        } =
            new ObservableCollection<ConstructionStaff>(
                new AppDbContext().ConstructionStaff.ToList()
            );

        public ObservableCollection<ReceivingOfficer> StaffReceivingOfficerList
        {
            get;
        } =
            new ObservableCollection<ReceivingOfficer>(
                new AppDbContext().ReceivingOfficers.ToList()
            );

        public ObservableCollection<Signer> SignerList
        {
            get;
        } =
            new ObservableCollection<Signer>(new AppDbContext().Signers.ToList());

        public ObservableCollection<Recipient> RecipientList
        {
            get;
        } =
            new ObservableCollection<Recipient>(new AppDbContext().Recipients.ToList());
        public ObservableCollection<DocumentTypeItem> DocumentTypes
        {
            get;
        }
        public ObservableCollection<SecurityLevelItem> SecurityLevel
        {
            get;
        }

        // Main list
        public ObservableCollection<IncomingDocument> IncomingDocs
        {
            get; private set;
        }
        public ObservableCollection<IncomingDocument> FilteredDocs
        {
            get; private set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); private string _summary; public string Summary
        {
            get => _summary; set
            {
                _summary = value;
                OnPropertyChanged(nameof(Summary));
            }
        }

        // Selected document
        private IncomingDocument _selectedDocument;
        public IncomingDocument SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                if (_selectedDocument?.Signer != null)
                    _selectedDocument.Signer.PropertyChanged -= Signer_PropertyChanged;

                _selectedDocument = value;

                if (_selectedDocument != null)
                {
                    if (StaffList != null)
                        _selectedDocument.ConstructionStaff = StaffList.FirstOrDefault(s => s.Id == _selectedDocument.ConstructionStaffId);

                    if (StaffReceivingOfficerList != null)
                        _selectedDocument.ReceivingOfficer = StaffReceivingOfficerList.FirstOrDefault(s => s.Id == _selectedDocument.ReceivingOfficerId);

                    if (SignerList != null)
                        _selectedDocument.Signer = SignerList.FirstOrDefault(s => s.Id == _selectedDocument.SignerId);

                    if (RecipientList != null)
                        _selectedDocument.Recipient = RecipientList.FirstOrDefault(s => s.Id == _selectedDocument.RecipientId);

                    if (_selectedDocument.Signer != null)
                        _selectedDocument.Signer.PropertyChanged += Signer_PropertyChanged;
                }


                OnPropertyChanged(nameof(SelectedDocument));
                OnPropertyChanged(nameof(CurrentSignerPosition));
            }
        }

        // Current signer position
        private void Signer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Signer.Position))
                OnPropertyChanged(nameof(CurrentSignerPosition));
        }
        public string CurrentSignerPosition => string.IsNullOrWhiteSpace(SelectedDocument?.Signer?.Position) ? " " : SelectedDocument.Signer.Position;

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


        // Constructor
        public IncomingDocViewModel()
        {
            SelectedDocument = new IncomingDocument();

            using var db = new AppDbContext();
            IncomingDocs = new ObservableCollection<IncomingDocument>(
                db.IncomingDocuments.Include(o => o.ConstructionStaff)
                    .Include(o => o.ReceivingOfficer)
                    .Include(o => o.Signer)
                    .Include(o => o.Recipient)
                    .ToList()
            );

            DocumentTypes = new ObservableCollection<DocumentTypeItem>
            {
                new DocumentTypeItem { Name = "Nghị quyết (cá biệt)", Abbreviation = "NQ" },
                new DocumentTypeItem { Name = "Quyết định (cá biệt)", Abbreviation = "QĐ" },
                new DocumentTypeItem { Name = "Chỉ thị", Abbreviation = "CT" },
                new DocumentTypeItem { Name = "Quy chế", Abbreviation = "QC" },
                new DocumentTypeItem { Name = "Quy định", Abbreviation = "QyĐ" },
                new DocumentTypeItem { Name = "Thông cáo", Abbreviation = "TC" },
                new DocumentTypeItem { Name = "Thông báo", Abbreviation = "TB" },
                new DocumentTypeItem { Name = "Hướng dẫn", Abbreviation = "HD" },
                new DocumentTypeItem { Name = "Chương trình", Abbreviation = "CTr" },
                new DocumentTypeItem { Name = "Kế hoạch", Abbreviation = "KH" },
                new DocumentTypeItem { Name = "Phương án", Abbreviation = "PA" },
                new DocumentTypeItem { Name = "Đề án", Abbreviation = "ĐA" },
                new DocumentTypeItem { Name = "Dự án", Abbreviation = "DA" },
                new DocumentTypeItem { Name = "Báo cáo", Abbreviation = "BC" },
                new DocumentTypeItem { Name = "Biên bản", Abbreviation = "BB" },
                new DocumentTypeItem { Name = "Tờ trình", Abbreviation = "TTr" },
                new DocumentTypeItem { Name = "Hợp đồng", Abbreviation = "HĐ" },
                new DocumentTypeItem { Name = "Công điện", Abbreviation = "CĐ" },
                new DocumentTypeItem { Name = "Bản ghi nhớ", Abbreviation = "BGN" },
                new DocumentTypeItem { Name = "Bản thỏa thuận", Abbreviation = "BTT" },
                new DocumentTypeItem { Name = "Giấy ủy quyền", Abbreviation = "GUQ" },
                new DocumentTypeItem { Name = "Giấy mời", Abbreviation = "GM" },
                new DocumentTypeItem { Name = "Giấy giới thiệu", Abbreviation = "GGT" },
                new DocumentTypeItem { Name = "Giấy nghỉ phép", Abbreviation = "GNP" },
                new DocumentTypeItem { Name = "Phiếu gửi", Abbreviation = "PG" },
                new DocumentTypeItem { Name = "Phiếu chuyển", Abbreviation = "PC" },
                new DocumentTypeItem { Name = "Phiếu báo", Abbreviation = "PB" },
            };

            SecurityLevel = new ObservableCollection<SecurityLevelItem>
            {
                new SecurityLevelItem { Name = "Thường", Abbreviation = "1" },
                new SecurityLevelItem { Name = "Mật", Abbreviation = "2" },
                new SecurityLevelItem { Name = "Tuyệt mật", Abbreviation = "3" },
                new SecurityLevelItem { Name = "Tối mật", Abbreviation = "4" },
            };

            IncomingDocs = new ObservableCollection<IncomingDocument>(
                db.IncomingDocuments.Include(o => o.ConstructionStaff)
                                    .Include(o => o.ReceivingOfficer)
                                    .Include(o => o.Signer)
                                    .Include(o => o.Recipient)
                                    .ToList()
            );

            FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);
            UpdatePagedDocs();


            SaveCommand = new RelayCommand(param => SaveDocument());
            EditCommand = new RelayCommand(param => EditDocument());
            DeleteCommand = new RelayCommand(param => DeleteDocument());
            ExportExcelCommand = new RelayCommand(param => ExportToExcel());
            NextPageCommand = new RelayCommand(param => { if (CurrentPage < TotalPages) CurrentPage++; });
            PrevPageCommand = new RelayCommand(param => { if (CurrentPage > 1) CurrentPage--; });
            GoToFirstPageCommand = new RelayCommand(param => { CurrentPage = 1; });
            GoToLastPageCommand = new RelayCommand(param => { CurrentPage = TotalPages; });
        }

        // 🔍 Filter
        public void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
                FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);
            else
            {
                var results = IncomingDocs.Where(d =>
                    d.Id.ToString().Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)
                    || d.ArrivalNumber?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.DocumentNumber?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.DocumentDate?.ToString("dd/MM/yyyy").Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.DocumentType?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.Summary?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.SecurityLevel?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.Sender?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.Signer?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.Signer?.Position?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.Recipient?.Name?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                    || d.ConstructionStaff?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();

                FilteredDocs = new ObservableCollection<IncomingDocument>(results);
            }

            CurrentPage = 1;
            UpdatePagedDocs();
        }

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
                UpdatePagedDocs();
            }
        }

        public int PageSize { get; set; } = 10;

        public int TotalPages => (int)Math.Ceiling((double)(FilteredDocs?.Count ?? 0) / PageSize);

        public ObservableCollection<IncomingDocument> PagedDocs
        {
            get; set;
        } =
            new ObservableCollection<IncomingDocument>();

        private void UpdatePagedDocs()
        {
            if (FilteredDocs == null)
                return;

            var items = FilteredDocs.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();

            PagedDocs = new ObservableCollection<IncomingDocument>(items);

            OnPropertyChanged(nameof(PagedDocs));
            OnPropertyChanged(nameof(TotalPages));
        }


        // List Staff
        public ObservableCollection<IncomingDocument> DocumentList
        {
            get; set;
        }

        /*Function Load*/
        private void LoadDocumentList()
        {
            using var db = new AppDbContext();
            var documentFromDb = db.IncomingDocuments.ToList();
            DocumentList = new ObservableCollection<IncomingDocument>(documentFromDb);
            OnPropertyChanged(nameof(DocumentList));
        }

        /*Check valid*/
        private bool IsValidDocument(IncomingDocument doc, out string error)
        {
            if (string.IsNullOrWhiteSpace(doc.ArrivalNumber))
            {
                error = "Vui lòng chọn Số đến.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(doc.DocumentNumber))
            {
                error = "Vui lòng nhập Số/Ký hiệu văn bản.";
                return false;
            }
            if (!doc.DocumentDate.HasValue)
            {
                error = "Vui lòng chọn Ngày văn bản.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(doc.DocumentType))
            {
                error = "Vui lòng chọn Loại văn bản.";
                return false;
            }
            if (doc.ReceivingOfficer == null || string.IsNullOrWhiteSpace(doc.ReceivingOfficer.FullName))
            {
                error = "Vui lòng chọn Cán bộ tiếp nhận.";
                return false;
            }
            if (doc.ConstructionStaff == null || string.IsNullOrWhiteSpace(doc.ConstructionStaff.FullName))
            {
                error = "Vui lòng chọn Cán bộ xử lý.";
                return false;
            }

            error = "";
            return true;
        }

        // 💾 Save
        private void SaveDocument()
        {
            if (!IsValidDocument(SelectedDocument, out string error))
            {
                Notify?.Invoke(error, false);
                return;
            }
            if (SelectedDocument == null)
            {
                Notify?.Invoke("Không có văn bản nào được chọn để lưu.", false);
                return;
            }
            try
            {
                using var db = new AppDbContext();

                if (SelectedDocument.Id > 0)
                {
                    var existing = db
                        .IncomingDocuments.Include(o => o.ConstructionStaff)
                        .Include(o => o.ReceivingOfficer)
                        .Include(o => o.Signer)
                        .Include(o => o.Recipient)
                        .FirstOrDefault(x => x.Id == SelectedDocument.Id);
                    if (existing != null)
                    {
                        existing.ArrivalNumber = SelectedDocument.ArrivalNumber;
                        existing.ArrivalDate = SelectedDocument.ArrivalDate;
                        existing.DocumentNumber = SelectedDocument.DocumentNumber;
                        existing.DocumentDate = SelectedDocument.DocumentDate;
                        existing.SecurityLevel = SelectedDocument.SecurityLevel;
                        existing.DocumentType = SelectedDocument.DocumentType;
                        existing.Sender = SelectedDocument.Sender;
                        existing.Position = SelectedDocument.Position;
                        existing.Summary = SelectedDocument.Summary;
                        existing.ConstructionStaffId = SelectedDocument.ConstructionStaff?.Id;
                        existing.ReceivingOfficerId = SelectedDocument.ReceivingOfficer?.Id;
                        existing.SignerId = SelectedDocument.Signer?.Id;
                        existing.RecipientId = SelectedDocument.Recipient?.Id;
                        existing.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        db.IncomingDocuments.Add(SelectedDocument);
                    }
                }
                else
                {
                    db.IncomingDocuments.Add(
                        new IncomingDocument
                        {
                            ArrivalNumber = SelectedDocument.ArrivalNumber,
                            ArrivalDate = SelectedDocument.ArrivalDate,
                            DocumentNumber = SelectedDocument.DocumentNumber,
                            DocumentDate = SelectedDocument.DocumentDate,
                            SecurityLevel = SelectedDocument.SecurityLevel,
                            DocumentType = SelectedDocument.DocumentType,
                            Sender = SelectedDocument.Sender,
                            Position = SelectedDocument.Position,
                            Summary = SelectedDocument.Summary,
                            ConstructionStaffId = SelectedDocument.ConstructionStaff?.Id,
                            ReceivingOfficerId = SelectedDocument.ReceivingOfficer?.Id,
                            SignerId = SelectedDocument.Signer?.Id,
                            RecipientId = SelectedDocument.Recipient?.Id,
                            CreatedAt = DateTime.Now
                        }
                    );
                }

                db.SaveChanges();
                OnPropertyChanged(nameof(SelectedDocument));
                Notify?.Invoke(SelectedDocument.Id > 0 ? "Sửa văn bản đến thành công" : "Thêm văn bản đến thành công", true);
                RefreshDocumentList();
                ApplyFilter();

                SelectedDocument = new IncomingDocument();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu văn bản đến: {ex.Message}", false);
            }
        }
        private void RefreshDocumentList()
        {
            using var db = new AppDbContext();
            var updatedDocs = db
                .IncomingDocuments
                .Include(o => o.ConstructionStaff)
                .Include(o => o.ReceivingOfficer)
                .Include(o => o.Signer)
                .Include(o => o.Recipient)
                .ToList();

            IncomingDocs.Clear();
            foreach (var doc in updatedDocs)
                IncomingDocs.Add(doc);

            FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);
        }


        // ✏️ Edit
        private void EditDocument()
        {
            if (SelectedDocument == null)
                return;
            SelectedDocument.Summary += " (đã chỉnh sửa)";
            SaveDocument();
            OnPropertyChanged(nameof(SelectedDocument));
        }

        // 🗑️ Delete
        private void DeleteDocument()
        {
            if (SelectedDocument == null)
            {
                Notify?.Invoke("Vui lòng chọn văn bản để xóa.", false);
                return;
            }

            var confirm = MessageBox.Show("Bạn có chắc muốn xóa văn bản này?", "Xác nhận", MessageBoxButton.YesNo);
            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var existing = db.IncomingDocuments.FirstOrDefault(d => d.Id == SelectedDocument.Id);
                if (existing == null)
                {
                    Notify?.Invoke("Không tìm thấy văn bản để xóa.", false);
                    return;
                }

                db.IncomingDocuments.Remove(existing);
                db.SaveChanges();

                IncomingDocs.Remove(SelectedDocument);
                RefreshDocumentList();
                ApplyFilter();
                OnPropertyChanged(nameof(PagedDocs));


                Notify?.Invoke("Xóa văn bản thành công", true);
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Lỗi khi xóa: {ex.Message}", false);
            }
        }

        // 📤 Export Excel
        private void ExportToExcel()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = $"VanBanDen_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
            };
            if (dialog.ShowDialog() == true)
            {
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("IncomingDocs");

                    // Header
                    ws.Cell(1, 1).Value = "ID";
                    ws.Cell(1, 2).Value = "Số đến";
                    ws.Cell(1, 3).Value = "Ngày đến";
                    ws.Cell(1, 4).Value = "Số/Ký hiệu VB";
                    ws.Cell(1, 5).Value = "Ngày VB";
                    ws.Cell(1, 6).Value = "Độ mật";
                    ws.Cell(1, 7).Value = "Loại VB";
                    ws.Cell(1, 8).Value = "Trích yếu nội dung";
                    ws.Cell(1, 9).Value = "Nơi gửi";
                    ws.Cell(1, 10).Value = "Người ký";
                    ws.Cell(1, 11).Value = "Chức vụ";
                    ws.Cell(1, 12).Value = "Nơi nhận";
                    ws.Cell(1, 13).Value = "Cán bộ tiếp nhận";
                    ws.Cell(1, 14).Value = "Cán bộ xử lý";

                    // Data
                    int row = 2;
                    foreach (var doc in FilteredDocs)
                    {
                        ws.Cell(row, 1).Value = doc.Id;
                        ws.Cell(row, 2).Value = doc.ArrivalNumber;
                        ws.Cell(row, 3).Value = doc.ArrivalDate?.ToString("dd/MM/yyyy");
                        ws.Cell(row, 4).Value = doc.DocumentNumber;
                        ws.Cell(row, 5).Value = doc.DocumentDate?.ToString("dd/MM/yyyy");
                        ws.Cell(row, 6).Value = doc.SecurityLevel;
                        ws.Cell(row, 7).Value = doc.DocumentType;
                        ws.Cell(row, 8).Value = doc.Summary;
                        ws.Cell(row, 9).Value = doc.Sender;
                        ws.Cell(row, 10).Value = doc.Signer?.FullName;
                        ws.Cell(row, 11).Value = doc.Signer?.Position;
                        ws.Cell(row, 12).Value = doc.Recipient?.Name;
                        ws.Cell(row, 13).Value = doc.ReceivingOfficer?.FullName;
                        ws.Cell(row, 14).Value = doc.ConstructionStaff?.FullName;
                        row++;
                    }

                    // Style header
                    var headerRange = ws.Range("A1:N1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Border
                    var dataRange = ws.Range($"A1:N{row - 1}");
                    dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    // Fit column
                    ws.Columns().AdjustToContents();

                    workbook.SaveAs(dialog.FileName);
                }
            }
        }
    }
}
