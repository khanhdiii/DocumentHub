using ClosedXML.Excel;
using DocumentHub.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace DocumentHub.ViewModel
{
    public class IncomingDocViewModel : INotifyPropertyChanged
    {
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ExportExcelCommand { get; }

        // List Incoming Doc
        public ObservableCollection<IncomingDocument> IncomingDocs { get; }
           = new ObservableCollection<IncomingDocument>();

        //Event of INotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //List DocumentTypeItem data
        public ObservableCollection<DocumentTypeItem> DocumentTypes { get; } = new ObservableCollection<DocumentTypeItem>
        {
            new DocumentTypeItem { Name = "Công văn", Abbreviation = "CV" },
            new DocumentTypeItem { Name = "Thông báo", Abbreviation = "TB" },
            new DocumentTypeItem { Name = "Quyết định", Abbreviation = "QĐ" },
            new DocumentTypeItem { Name = "Hướng dẫn", Abbreviation = "HD" }
        };

        //List SecurityLevelItem data
        public ObservableCollection<SecurityLevelItem> SecurityLevel { get; } = new ObservableCollection<SecurityLevelItem>
        {
            new SecurityLevelItem { Name = "Tuyệt mật", Abbreviation = "1" },
            new SecurityLevelItem { Name = "Tối mật", Abbreviation = "2" },
            new SecurityLevelItem { Name = "Mật", Abbreviation = "3" },
            new SecurityLevelItem { Name = "Thường", Abbreviation = "4" },
        };

        //List Construction Staff
        public ObservableCollection<ConstructionStaff> StaffList { get; } = new ObservableCollection<ConstructionStaff>
        {
            new ConstructionStaff { Id = 1, FullName = "Nguyễn Văn A" },
            new ConstructionStaff { Id = 2, FullName = "Trần Thị B" },
            new ConstructionStaff { Id = 3, FullName = "Lê Văn C" }
        };

        //List Signer
        public ObservableCollection<Signer> SignerList { get; } = new ObservableCollection<Signer>
        {
            new Signer { Id = 1, FullName = "Trần Văn B", Position = "Giám đốc" },
            new Signer { Id = 2, FullName = "Phạm Minh D", Position = "Phó Chủ tịch" }
        };

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
                OnPropertyChanged(nameof(SelectedDocument));
                OnPropertyChanged(nameof(CurrentSignerPosition));

                if (_selectedDocument?.Signer != null)
                    _selectedDocument.Signer.PropertyChanged += Signer_PropertyChanged;
            }
        }

        private void Signer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Signer.Position))
                OnPropertyChanged(nameof(CurrentSignerPosition));
        }

        public string CurrentSignerPosition => SelectedDocument?.Signer?.Position ?? "";

        // Filtered list for search
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

        //Constructor
        public IncomingDocViewModel()
        {
            //Defind value for Signer
            SelectedDocument = new IncomingDocument
            {
                Signer = SignerList.FirstOrDefault()
            };
            // Sample data
            IncomingDocs.Add(new IncomingDocument
            {
                Id = 1,
                ArrivalNumber = "ĐN-001/2025",
                ArrivalDate = DateTime.Today.AddDays(-1),
                DocumentNumber = "CV-123",
                DocumentDate = DateTime.Today.AddDays(-2),
                DocumentType = "Công văn",
                SecurityLevel = "Mật",
                Sender = "Sở Nội vụ",
                Signer = SignerList.FirstOrDefault(),
                Position = "Giám đốc",
                Recipient = "UBND tỉnh",
                ConstructionStaff = StaffList.FirstOrDefault(),
                Summary = "Công văn đề nghị bổ sung nhân sự."
            });

            IncomingDocs.Add(new IncomingDocument
            {
                Id = 2,
                ArrivalNumber = "ĐN-002/2025",
                ArrivalDate = DateTime.Today.AddDays(-3),
                DocumentNumber = "TB-456",
                DocumentDate = DateTime.Today.AddDays(-4),
                DocumentType = "Thông báo",
                SecurityLevel = "Thường",
                Sender = "Phòng Tài chính",
                Signer = SignerList.LastOrDefault(),
                Position = "Trưởng phòng",
                Recipient = "Ban Giám đốc",
                ConstructionStaff = StaffList.LastOrDefault(),
                Summary = "Thông báo tình hình ngân sách quý IV."
            });

            FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);

            AddCommand = new RelayCommand(param => AddDocument());
            EditCommand = new RelayCommand(param => EditDocument());
            DeleteCommand = new RelayCommand(param => DeleteDocument());
            ExportExcelCommand = new RelayCommand(param => ExportToExcel());
        }

        // Search filter
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
                    (d.DocumentType?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.SecurityLevel?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Sender?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Signer?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Position?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Recipient?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.ConstructionStaff?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Summary?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                FilteredDocs = new ObservableCollection<IncomingDocument>(results);
            }
        }

        // Add
        private void AddDocument()
        {
            var newDoc = new IncomingDocument
            {
                Id = IncomingDocs.Count + 1,
                ArrivalNumber = "NEW-" + DateTime.Now.Ticks,
                ArrivalDate = DateTime.Today,
                DocumentNumber = "NEW-" + DateTime.Now.Ticks,
                DocumentDate = DateTime.Today,
                DocumentType = DocumentTypes.FirstOrDefault()?.Name ?? "Mới",
                SecurityLevel = SecurityLevel.FirstOrDefault()?.Name ?? "Thường",
                Sender = "Người gửi",
                Signer = SignerList.FirstOrDefault(),
                Position = "Chức vụ",
                Recipient = "Người nhận",
                ConstructionStaff = StaffList.FirstOrDefault(),
                Summary = "Nội dung mới"
            };

            SelectedDocument = newDoc;
            IncomingDocs.Add(newDoc);
            ApplyFilter();
        }

        // Edit
        private void EditDocument()
        {
            if (SelectedDocument == null) return;
            SelectedDocument.Summary += " (đã chỉnh sửa)";
            OnPropertyChanged(nameof(SelectedDocument));
        }

        // Delete
        private void DeleteDocument()
        {
            if (SelectedDocument == null) return;
            IncomingDocs.Remove(SelectedDocument);
            ApplyFilter();
        }

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
                    ws.Cell(1, 8).Value = "Nơi gửi";
                    ws.Cell(1, 9).Value = "Người ký";
                    ws.Cell(1, 10).Value = "Chức vụ";
                    ws.Cell(1, 11).Value = "Người nhận";
                    ws.Cell(1, 12).Value = "Người xử lý";
                    ws.Cell(1, 13).Value = "Trích yếu nội dung";

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
                        ws.Cell(row, 8).Value = doc.Sender;
                        ws.Cell(row, 9).Value = doc.Signer?.FullName;
                        ws.Cell(row, 10).Value = doc.Signer?.Position;
                        ws.Cell(row, 11).Value = doc.Recipient;
                        ws.Cell(row, 12).Value = doc.ConstructionStaff?.FullName;
                        ws.Cell(row, 13).Value = doc.Summary;
                        row++;
                    }

                    // Style header
                    var headerRange = ws.Range("A1:M1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Border
                    var dataRange = ws.Range($"A1:M{row - 1}");
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
