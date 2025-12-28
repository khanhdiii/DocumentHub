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

        // Call to RecipientsViewModel
        public RecipientsViewModel RecipientsVM { get; } = new RecipientsViewModel();
        // Take List RecipientList from RecipientsViewModel
        public ObservableCollection<Recipient> RecipientList => RecipientsVM.RecipientList;

        // Call to ReceivingOfficerViewModel
        public ReceivingOfficerViewModel ReceivingOfficerVM { get; } = new ReceivingOfficerViewModel();
        // Take List ReceivingOfficer from ReceivingOfficerViewModel
        public ObservableCollection<ReceivingOfficer> StaffReceivingOfficerList => ReceivingOfficerVM.StaffReceivingOfficerList;


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
            for (int i = 1; i <= 22; i++)
            {
                IncomingDocs.Add(new IncomingDocument
                {
                    Id = i,
                    ArrivalNumber = $"ĐN-{i:000}/2025",
                    ArrivalDate = DateTime.Today.AddDays(-i),
                    DocumentNumber = $"VB-{100 + i}",
                    DocumentDate = DateTime.Today.AddDays(-(i + 1)),
                    DocumentType = DocumentTypes[(i % DocumentTypes.Count)].Name,
                    SecurityLevel = SecurityLevel[(i % SecurityLevel.Count)].Name,
                    Sender = $"Đơn vị gửi {i}",
                    Signer = SignerList[(i % SignerList.Count)],
                    Position = SignerList[(i % SignerList.Count)].Position,
                    Recipient = RecipientList[(i % RecipientList.Count)],
                    ConstructionStaff = StaffList[(i % StaffList.Count)],
                    ReceivingOfficer = StaffReceivingOfficerList[(i % StaffReceivingOfficerList.Count)],
                    Summary = $"Nội dung văn bản mẫu số {i}"
                });
            }


            FilteredDocs = new ObservableCollection<IncomingDocument>(IncomingDocs);
            UpdatePagedDocs();
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
                if (_currentPage == value) return;
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePagedDocs();
            }
        }

        public int PageSize { get; set; } = 10;

        public int TotalPages =>
            (int)Math.Ceiling((double)(FilteredDocs?.Count ?? 0) / PageSize);

        public ObservableCollection<IncomingDocument> PagedDocs { get; set; }
            = new ObservableCollection<IncomingDocument>();

        private void UpdatePagedDocs()
        {
            if (FilteredDocs == null) return;

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
                Sender = "Nơi gửi",
                Signer = SignerList.FirstOrDefault(),
                Position = "Chức vụ",
                Recipient = RecipientList.FirstOrDefault(),
                ConstructionStaff = StaffList.FirstOrDefault(),
                ReceivingOfficer = StaffReceivingOfficerList.FirstOrDefault(),
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
