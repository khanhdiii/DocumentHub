using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentHub.Model;
using Microsoft.Win32;
using PdfSharp.Drawing;
using PdfSharp.Snippets.Drawing;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace DocumentHub.ViewModel
{

    public class OutgoingDocViewModel : INotifyPropertyChanged
    {
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        //Command export excel
        public ICommand ExportExcelCommand { get; }

        // Call to RecipientsViewModel
        public RecipientsViewModel RecipientsVM { get; } = new RecipientsViewModel();
        // Take List RecipientList from RecipientsViewModel
        public ObservableCollection<Recipient> RecipientList => RecipientsVM.RecipientList;

        // Call to ReceivingOfficerViewModel
        public ReceivingOfficerViewModel ReceivingOfficerVM { get; } = new ReceivingOfficerViewModel();
        // Take List ReceivingOfficer from ReceivingOfficerViewModel
        public ObservableCollection<ReceivingOfficer> StaffReceivingOfficerList => ReceivingOfficerVM.StaffReceivingOfficerList;


        //Command go first page
        public ICommand GoToFirstPageCommand => new RelayCommand(param =>
        {
            CurrentPage = 1;
        });
        //Command go last page
        public ICommand GoToLastPageCommand => new RelayCommand(param =>
        {
            CurrentPage = TotalPages;
        });

        // List Outgoing Doc
        public ObservableCollection<OutgoingDocument> OutgoingDocs { get; }
            = new ObservableCollection<OutgoingDocument>();

        //Event of INotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;

        
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //List DocumentTypeItem data
        public ObservableCollection<DocumentTypeItem> DocumentTypes { get; } = new ObservableCollection<DocumentTypeItem>
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
            new ConstructionStaff { Id = 2, FullName = "Trần Thị B"},
            new ConstructionStaff { Id = 3, FullName = "Lê Văn C" },
            new ConstructionStaff { Id = 4, FullName = "Lê Thị C" },
            new ConstructionStaff { Id = 5, FullName = "Nguyễn Thị E" },
            new ConstructionStaff { Id = 6, FullName = "Trần Thị G" },
            new ConstructionStaff { Id = 7, FullName = "Phạm Thị I" },
            new ConstructionStaff { Id = 8, FullName = "Nguyễn Văn L"}
        };

        //List Signer
        public ObservableCollection<Signer> SignerList { get; } = new ObservableCollection<Signer>
        {
            new Signer { Id = 1, FullName = "Trần Văn B", Position = "Giám đốc" },
            new Signer { Id = 2, FullName = "Phạm Minh D", Position = "Phó Chủ tịch" }, 
            new Signer { Id = 3, FullName = "Lê Văn F", Position = "Chủ tịch" },
            new Signer { Id = 4, FullName = "Nguyễn Văn H", Position = "Trưởng phòng" },
            new Signer { Id = 5, FullName = "Đỗ Văn K", Position = "Kế toán trưởng" }, 
            new Signer { Id = 6, FullName = "Trần Thị M", Position = "Chánh văn phòng" } 
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

        //Event Selected Signer
        private OutgoingDocument _selectedDocument;
        public OutgoingDocument SelectedDocument
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
        //  Event Signer change
        private void Signer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Signer.Position))
                OnPropertyChanged(nameof(CurrentSignerPosition));
        }

        public string CurrentSignerPosition => SelectedDocument?.Signer?.Position ?? "";


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
        public OutgoingDocViewModel()
        {
            ExportExcelCommand = new RelayCommand(param => ExportToExcel());

            //Defind value for Signer
            SelectedDocument = new OutgoingDocument
            {
                Signer = SignerList.FirstOrDefault() 
            };

            // Sample data
            for (int i = 1; i <= 22; i++)
            {
                OutgoingDocs.Add(new OutgoingDocument
                {
                    Id = i,
                    DocumentNumber = $"VB-{100 + i}",
                    DocumentDate = DateTime.Today.AddDays(-(i + 1)),
                    DocumentType = DocumentTypes[(i % DocumentTypes.Count)].Name,
                    Summary = $"Nội dung văn bản mẫu số {i}",
                    SecurityLevel = SecurityLevel[(i % SecurityLevel.Count)].Name,
                    ConstructionStaff = StaffList[(i % StaffList.Count)],
                    ReceivingOfficer = StaffReceivingOfficerList[(i % StaffReceivingOfficerList.Count)],
                    Signer = SignerList[(i % SignerList.Count)],
                    Recipient = RecipientList[(i % RecipientList.Count)],
                });
            }
            

            AddCommand = new RelayCommand(param => AddDocument());
            EditCommand = new RelayCommand(param => EditDocument());
            DeleteCommand = new RelayCommand(param => DeleteDocument());
            ExportExcelCommand = new RelayCommand(param => ExportToExcel());

            FilteredDocs = new ObservableCollection<OutgoingDocument>(OutgoingDocs);
            //Show page 1 in pagination
            UpdatePagedDocs();
        }

        //Search and filtered
        private ObservableCollection<OutgoingDocument> _filteredDocs;
        public ObservableCollection<OutgoingDocument> FilteredDocs
        {
            get => _filteredDocs;
            set
            {
                _filteredDocs = value;
                OnPropertyChanged(nameof(FilteredDocs));
            }
        }

        //Keyword when search
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

        private string _searchKeyword;


        //Function filter data
        public void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                FilteredDocs = new ObservableCollection<OutgoingDocument>(OutgoingDocs);
            }
            else
            {
                var results = OutgoingDocs.Where(d =>
                    (d.Id.ToString().Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)) ||
                    (d.DocumentNumber?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.DocumentDate?.ToString("dd/MM/yyyy").Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.DocumentType?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Summary?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.SecurityLevel?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.ConstructionStaff?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.ReceivingOfficer?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Signer?.FullName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Signer?.Position?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Recipient?.Name?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) 
                ).ToList();

                FilteredDocs = new ObservableCollection<OutgoingDocument>(results);
                OnPropertyChanged(nameof(FilteredDocs));
                CurrentPage = 1;
                // reset first page
                 UpdatePagedDocs(); 
                // Update Doc
            }
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

        public ObservableCollection<OutgoingDocument> PagedDocs { get; set; }
            = new ObservableCollection<OutgoingDocument>();

        private void UpdatePagedDocs()
        {
            if (FilteredDocs == null) return;

            var items = FilteredDocs
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            PagedDocs = new ObservableCollection<OutgoingDocument>(items);

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


        //Function Add
        private void AddDocument()
        {
            var newDoc = new OutgoingDocument 
            {
                Id = OutgoingDocs.Count + 1,
                DocumentNumber = "NEW-" + DateTime.Now.Ticks, 
                DocumentDate = DateTime.Today,
                DocumentType = "Mới",
                Summary = "Nội dung mới", 
                SecurityLevel = "Thường", 
                ConstructionStaff = new ConstructionStaff { FullName = "Trần Thị B" },
                ReceivingOfficer = new ReceivingOfficer {  FullName = "Trần Nhận B" },
                Signer = new Signer {  FullName = "Phạm Minh D", Position = "Phó Chủ tịch" },
                Recipient = new Recipient { Name = "Ban Giám đốc" }
            };

            SelectedDocument = newDoc;
            OutgoingDocs.Add(newDoc);
        }


        //Function Edit
        private void EditDocument()
        {
            if (SelectedDocument == null) return;
            SelectedDocument.Summary += " (đã chỉnh sửa)";
            OnPropertyChanged(nameof(SelectedDocument));
        }

        //Function Delete
        private void DeleteDocument()
        {
            if (SelectedDocument != null)
                OutgoingDocs.Remove(SelectedDocument);
        }


        //Function Export excel
        private void ExportToExcel()
        {
            var dialog = new SaveFileDialog 
            { 
                Filter = "Excel Workbook|*.xlsx",
                FileName = $"VanBanDi_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx" };
            if (dialog.ShowDialog() == true)
            { 
                using (var workbook = new XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("OutgoingDocs");
                    ws.Cell(1, 1).Value = "ID";
                    ws.Cell(1, 2).Value = "Số/Ký hiệu";
                    ws.Cell(1, 3).Value = "Ngày VB";
                    ws.Cell(1, 4).Value = "Loại VB";
                    ws.Cell(1, 5).Value = "Nội dung";
                    ws.Cell(1, 6).Value = "Độ mật";
                    ws.Cell(1, 7).Value = "Cán bộ xử lý";
                    ws.Cell(1, 8).Value = "Cán bộ tiếp nhận";
                    ws.Cell(1, 9).Value = "Người ký";
                    ws.Cell(1, 10).Value = "Chức vụ";
                    ws.Cell(1, 11).Value = "Nơi nhận";
                    
                    int row = 2;
                    foreach (var doc in OutgoingDocs)
                    { 
                        ws.Cell(row, 1).Value = doc.Id;
                        ws.Cell(row, 2).Value = doc.DocumentNumber;
                        ws.Cell(row, 3).Value = doc.DocumentDate?.ToString("dd/MM/yyyy");
                        ws.Cell(row, 4).Value = doc.DocumentType;
                        ws.Cell(row, 5).Value = doc.Summary;
                        ws.Cell(row, 6).Value = doc.SecurityLevel;
                        ws.Cell(row, 7).Value = doc.ConstructionStaff?.FullName;
                        ws.Cell(row, 8).Value = doc.ReceivingOfficer?.FullName;
                        ws.Cell(row, 9).Value = doc.Signer?.FullName; 
                        ws.Cell(row, 10).Value = doc.Signer?.Position;
                        ws.Cell(row, 11).Value = doc.Recipient?.Name; row++;
                    }
                    ws.Columns().AdjustToContents();
                    workbook.SaveAs(dialog.FileName);
                }
            } 
        } 
    }
}
