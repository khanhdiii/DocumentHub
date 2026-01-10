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
        public ICommand ExportExcelCommand
        {
            get;
        }

        //Call Data from database
        public ObservableCollection<ConstructionStaff> StaffList
        {
            get;
        }
             = new ObservableCollection<ConstructionStaff>(new AppDbContext().ConstructionStaff.ToList());

        public ObservableCollection<ReceivingOfficer> StaffReceivingOfficerList
        {
            get;
        }
            = new ObservableCollection<ReceivingOfficer>(new AppDbContext().ReceivingOfficers.ToList());

        public ObservableCollection<Signer> SignerList
        {
            get;
        }
            = new ObservableCollection<Signer>(new AppDbContext().Signers.ToList());

        public ObservableCollection<Recipient> RecipientList
        {
            get;
        }
            = new ObservableCollection<Recipient>(new AppDbContext().Recipients.ToList());

        // List Incoming Doc
        public ObservableCollection<IncomingDocument> IncomingDocs
        {
            get;
        }
           = new ObservableCollection<IncomingDocument>();

        //Event of INotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //List DocumentTypeItem data
        public ObservableCollection<DocumentTypeItem> DocumentTypes
        {
            get;
        } = new ObservableCollection<DocumentTypeItem>
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
             new DocumentTypeItem { Name = "Phiếu báo", Abbreviation = "PB" }
        };

        //List SecurityLevelItem data
        public ObservableCollection<SecurityLevelItem> SecurityLevel
        {
            get;
        } = new ObservableCollection<SecurityLevelItem>
        {
            new SecurityLevelItem { Name = "Tuyệt mật", Abbreviation = "1" },
            new SecurityLevelItem { Name = "Tối mật", Abbreviation = "2" },
            new SecurityLevelItem { Name = "Mật", Abbreviation = "3" },
            new SecurityLevelItem { Name = "Thường", Abbreviation = "4" },
        };

        // Selected incoming document
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
                    // Gán lại từ danh sách để ComboBox nhận đúng
                    _selectedDocument.ConstructionStaff = StaffList.FirstOrDefault(s => s.Id == _selectedDocument.ConstructionStaffId);
                    _selectedDocument.ReceivingOfficer = StaffReceivingOfficerList.FirstOrDefault(s => s.Id == _selectedDocument.ReceivingOfficerId);
                    _selectedDocument.Signer = SignerList.FirstOrDefault(s => s.Id == _selectedDocument.SignerId);
                    _selectedDocument.Recipient = RecipientList.FirstOrDefault(s => s.Id == _selectedDocument.RecipientId);

                    if (_selectedDocument.Signer != null)
                        _selectedDocument.Signer.PropertyChanged += Signer_PropertyChanged;
                }

                OnPropertyChanged(nameof(SelectedDocument));
                OnPropertyChanged(nameof(CurrentSignerPosition));
            }
        }


        // Property SelectedStaff 
        private ConstructionStaff _selectedStaff;
        public ConstructionStaff SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                _selectedStaff = value;
                OnPropertyChanged(nameof(SelectedStaff));

                if (SelectedDocument != null)
                {
                    SelectedDocument.ConstructionStaff = _selectedStaff;
                    OnPropertyChanged(nameof(SelectedDocument));
                }
            }
        }

        //  Event Signer change
        private void Signer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Signer.Position))
                OnPropertyChanged(nameof(CurrentSignerPosition));
        }

        public string CurrentSignerPosition
        {
            get
            {
                var position = SelectedDocument?.Signer?.Position;
                return string.IsNullOrWhiteSpace(position) ? " " : position;
            }
        }

        //Recipient
        private Recipient _recipient;
        public Recipient Recipient
        {
            get => _recipient;
            set
            {
                _recipient = value;
                OnPropertyChanged(nameof(Recipient));
            }
        }
        public string RecipientName => Recipient?.Name;

        //Constructor
        public IncomingDocViewModel()
        {
            //Defind value for Signer
            SelectedDocument = new IncomingDocument();
          

            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            IncomingDocs = new ObservableCollection<IncomingDocument>(db.IncomingDocuments
                .Include(o => o.ConstructionStaff)
                .Include(o => o.ReceivingOfficer)
                .Include(o => o.Signer)
                .Include(o => o.Recipient)
                .ToList());

            FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);
            UpdatePagedDocs();
            SaveCommand = new RelayCommand(param => SaveDocument());
            EditCommand = new RelayCommand(param => EditDocument());
            DeleteCommand = new RelayCommand(param => DeleteDocument());
            ExportExcelCommand = new RelayCommand(param => ExportToExcel());
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
                error = "Vui lòng nhập Số đến.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(doc.DocumentNumber))
            {
                error = "Vui lòng nhập Số/Ký hiệu văn bản.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(doc.Sender))
            {
                error = "Vui lòng nhập Nơi gửi.";
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

            error = "";
            return true;
        }


        /*Function Save Document*/
        private void SaveDocument()
        {
            if (!IsValidDocument(SelectedDocument, out string error))
            {
                Notify?.Invoke(error,false); return;
            }    

            try
            {
                using var db = new AppDbContext();

                if (SelectedDocument.Id > 0)
                {
                    var existing = db.IncomingDocuments
                        .Include(o => o.ConstructionStaff)
                        .Include(o => o.ReceivingOfficer)
                        .Include(o => o.Signer)
                        .Include(o => o.Recipient)
                        .FirstOrDefault(x => x.Id == SelectedDocument.Id);

                    if (existing != null)
                    {
                        existing.ArrivalNumber = SelectedDocument.ArrivalNumber;
                        existing.Sender = SelectedDocument.Sender;
                        existing.DocumentNumber = SelectedDocument.DocumentNumber;
                        existing.DocumentDate = SelectedDocument.DocumentDate;
                        existing.ArrivalDate = SelectedDocument.ArrivalDate;
                        
                        existing.DocumentType = SelectedDocument.DocumentType;
                        existing.Summary = SelectedDocument.Summary;
                        existing.SecurityLevel = SelectedDocument.SecurityLevel;

                        existing.ConstructionStaffId = SelectedDocument.ConstructionStaff?.Id;
                        existing.ReceivingOfficerId = SelectedDocument.ReceivingOfficer?.Id;
                        existing.SignerId = SelectedDocument.Signer?.Id;
                        existing.RecipientId = SelectedDocument.Recipient?.Id;

                        //  Udate time update
                        existing.UpdatedAt = DateTime.Now;
                    }
                    else
                    {
                        SelectedDocument.CreatedAt = DateTime.Now;
                        db.IncomingDocuments.Add(SelectedDocument);
                    }
                }
                else
                {
                    db.IncomingDocuments.Add(new IncomingDocument
                    {
                        ArrivalNumber = SelectedDocument.ArrivalNumber,
                        Sender = SelectedDocument.Sender,
                        DocumentNumber = SelectedDocument.DocumentNumber,
                        DocumentDate = SelectedDocument.DocumentDate,
                        ArrivalDate = SelectedDocument.ArrivalDate,
                        DocumentType = SelectedDocument.DocumentType,
                        Summary = SelectedDocument.Summary,
                        SecurityLevel = SelectedDocument.SecurityLevel,
                        ConstructionStaffId = SelectedDocument.ConstructionStaff?.Id,
                        ReceivingOfficerId = SelectedDocument.ReceivingOfficer?.Id,
                        SignerId = SelectedDocument.Signer?.Id,
                        RecipientId = SelectedDocument.Recipient?.Id,

                        // ✅ Create time
                        CreatedAt = DateTime.Now
                    });
                }

                db.SaveChanges();

                if (SelectedDocument.Id > 0)
                    Notify?.Invoke("Sửa văn bản đến thành công", true);
                else
                    Notify?.Invoke("Thêm văn bản đến thành công", true);

                RefreshDocumentList();
                SelectedDocument = new IncomingDocument();
            }
            catch (Exception ex)
            {
                Notify?.Invoke($"Không thể lưu văn bản đến: {ex.Message}", false);
            }
        }


        /*Refresh Document*/
        private void RefreshDocumentList()
        {
            using var db = new AppDbContext();
            var updatedDocs = db.IncomingDocuments
                .Include(o => o.ConstructionStaff)
                .Include(o => o.ReceivingOfficer)
                .Include(o => o.Signer)
                .Include(o => o.Recipient)
                .ToList();

            IncomingDocs.Clear();
            foreach (var doc in updatedDocs)
                IncomingDocs.Add(doc);

            FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);
            UpdatePagedDocs();
        }


        //Function Edit
        private void EditDocument()
        {
            if (SelectedDocument == null)
                return;

            SelectedDocument.Summary += " (đã chỉnh sửa)";
            SelectedDocument.UpdatedAt = DateTime.Now;
            OnPropertyChanged(nameof(SelectedDocument));
        }

        //Function Delete
        private void DeleteDocument()
        {
            if (SelectedDocument != null)
                IncomingDocs.Remove(SelectedDocument);
        }


        //Search and filtered
        private ObservableCollection<IncomingDocument> _filteredDocs;
        public ObservableCollection<IncomingDocument> FilteredDocs
        {
            get => _filteredDocs;
            set
            {
                _filteredDocs = value;
                OnPropertyChanged(nameof(FilteredDocs));
            }
        }

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


        //Function filter data
        public void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);
            }
            else
            {
                var results = IncomingDocs.Where(d =>
                    (d.Id.ToString().Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    (d.ArrivalNumber?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.DocumentNumber?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.DocumentDate?.ToString("dd/MM/yyyy").Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.DocumentType?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.SecurityLevel?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Sender?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Signer?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Position?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Recipient?.Name?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.ConstructionStaff?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Summary?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                FilteredDocs = new ObservableCollection<IncomingDocument>(results);
            }

            CurrentPage = 1;
            UpdatePagedDocs();
        }


        //Function pagination
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

        public int TotalPages =>
            (int)Math.Ceiling((double)(FilteredDocs?.Count ?? 0) / PageSize);

        public ObservableCollection<IncomingDocument> PagedDocs
        {
            get; set;
        }
            = new ObservableCollection<IncomingDocument>();

        private void UpdatePagedDocs()
        {
            if (FilteredDocs == null)
                return;

            var items = FilteredDocs
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            PagedDocs = new ObservableCollection<IncomingDocument>(items);

            OnPropertyChanged(nameof(PagedDocs));
            OnPropertyChanged(nameof(TotalPages));
        }


        public ICommand NextPageCommand => new RelayCommand(param =>
        {
            if (CurrentPage < TotalPages)
                CurrentPage++;
        });

        public ICommand PrevPageCommand => new RelayCommand(param =>
        {
            if (CurrentPage > 1)
                CurrentPage--;
        });

        // Export Excel                  
        private void ExportToExcel()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = $"VanBanDen_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
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
                        ws.Cell(row, 3).Value = doc.ArrivalDate.HasValue ? doc.ArrivalDate.Value.ToString("dd/MM/yyyy") : "";
                        ws.Cell(row, 4).Value = doc.DocumentNumber;
                        ws.Cell(row, 5).Value = doc.DocumentDate.HasValue ? doc.DocumentDate.Value.ToString("dd/MM/yyyy") : "";
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

                    // Save file
                    workbook.SaveAs(dialog.FileName);
                }
            }
        }
    }
}
