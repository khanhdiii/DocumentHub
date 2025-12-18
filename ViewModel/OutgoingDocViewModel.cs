using DocumentHub.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ClosedXML.Excel;
using PdfSharp.Drawing;
using Microsoft.Win32;
using PdfSharp.Snippets.Drawing;

namespace DocumentHub.ViewModel
{

    //Interface notification when data change
    public class OutgoingDocViewModel : INotifyPropertyChanged

    {
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public ICommand ExportExcelCommand { get; }


        // List Outgoing Doc
        public ObservableCollection<OutgoingDocument> OutgoingDocs { get; }
            = new ObservableCollection<OutgoingDocument>();

        //Event of INotifyPropertyChanged 
        public event PropertyChangedEventHandler PropertyChanged;


        //Selected data when choose in table
        private OutgoingDocument _selectedDocument;
        public OutgoingDocument SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                _selectedDocument = value;
                OnPropertyChanged(nameof(SelectedDocument));
            }
        }

        //Event notification when name changed
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

        //Constructor
        public OutgoingDocViewModel()
        {
            ExportExcelCommand = new RelayCommand(param => ExportToExcel());
            //Data test
            OutgoingDocs.Add(new OutgoingDocument
            {
                Id = 1,
                DocumentNumber = "CV-001/2025",
                DocumentDate = DateTime.Today.AddDays(-1),
                DocumentType = "Công điện",
                SecurityLevel = "Tuyệt mật",
                Sender = "Phòng Hành chính",
                Handler = "Nguyễn Văn A",
                Signer = "Trần Văn B",
                SignerPosition = "Giám đốc",
                Recipient = "Sở Nội vụ",
                Summary = "Công văn gửi Sở Nội vụ về việc đề xuất bổ sung nhân lực."
            });

            OutgoingDocs.Add(new OutgoingDocument
            {
                Id = 2,
                DocumentNumber = "TB-002/2025",
                DocumentDate = DateTime.Today.AddDays(-3),
                DocumentType = "Thông báo",
                SecurityLevel = "Mật",
                Sender = "Văn phòng UBND",
                Handler = "Lê Thị C",
                Signer = "Phạm Minh D",
                SignerPosition = "Phó Chủ tịch",
                Recipient = "Các đơn vị trực thuộc",
                Summary = "Thông báo về kế hoạch triển khai quy định bảo mật thông tin."
            });

            OutgoingDocs.Add(new OutgoingDocument
            {
                Id = 3,
                DocumentNumber = "QĐ-003/2025",
                DocumentDate = DateTime.Today.AddDays(-5),
                DocumentType = "Phiếu báo",
                SecurityLevel = "Tối mật",
                Sender = "Ban Giám đốc",
                Handler = "Nguyễn Thị E",
                Signer = "Lê Văn F",
                SignerPosition = "Chủ tịch",
                Recipient = "Phòng Nhân sự",
                Summary = "Quyết định bổ nhiệm cán bộ phụ trách an ninh thông tin."
            });

            OutgoingDocs.Add(new OutgoingDocument
            {
                Id = 4,
                DocumentNumber = "HD-004/2025",
                DocumentDate = DateTime.Today.AddDays(-7),
                DocumentType = "Hướng dẫn",
                SecurityLevel = "Mật",
                Sender = "Phòng Kỹ thuật",
                Handler = "Trần Thị G",
                Signer = "Nguyễn Văn H",
                SignerPosition = "Trưởng phòng",
                Recipient = "Các đơn vị trực thuộc",
                Summary = "Hướng dẫn triển khai hệ thống quản lý văn bản điện tử."
            });

            OutgoingDocs.Add(new OutgoingDocument
            {
                Id = 5,
                DocumentNumber = "BC-005/2025",
                DocumentDate = DateTime.Today.AddDays(-10),
                DocumentType = "Báo cáo",
                SecurityLevel = "Mật",
                Sender = "Phòng Tài chính",
                Handler = "Phạm Thị I",
                Signer = "Đỗ Văn K",
                SignerPosition = "Kế toán trưởng",
                Recipient = "Ban Giám đốc",
                Summary = "Báo cáo tình hình ngân sách quý IV năm 2025."
            });

            OutgoingDocs.Add(new OutgoingDocument
            {
                Id = 6,
                DocumentNumber = "GM-006/2025",
                DocumentDate = DateTime.Today.AddDays(-12),
                DocumentType = "Giấy mời",
                SecurityLevel = "Mật",
                Sender = "Văn phòng UBND",
                Handler = "Nguyễn Văn L",
                Signer = "Trần Thị M",
                SignerPosition = "Chánh văn phòng",
                Recipient = "Các cơ quan ban ngành",
                Summary = "Giấy mời tham dự hội nghị tổng kết cuối năm."
            });
            AddCommand = new RelayCommand(param => AddDocument());
            EditCommand = new RelayCommand(param => EditDocument());
            DeleteCommand = new RelayCommand(param => DeleteDocument());
            ExportExcelCommand = new RelayCommand(param => ExportToExcel());

            FilteredDocs = new ObservableCollection<OutgoingDocument>(OutgoingDocs);
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
                ApplyFilter(); // gọi lọc mỗi khi text thay đổi
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
                    (d.SecurityLevel?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Sender?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Handler?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Signer?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.SignerPosition?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Recipient?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.Summary?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();

                FilteredDocs = new ObservableCollection<OutgoingDocument>(results);
                OnPropertyChanged(nameof(FilteredDocs));
            }
        }

        //Function Add
        private void AddDocument()
        {
            var newDoc = new OutgoingDocument
            {
                Id = OutgoingDocs.Count + 1,
                DocumentNumber = "NEW-" + DateTime.Now.Ticks,
                DocumentDate = DateTime.Today,
                DocumentType = "Mới",
                SecurityLevel = "Bình thường",
                Sender = "Người gửi",
                Handler = "Cán bộ",
                Signer = "Người ký",
                SignerPosition = "Chức vụ",
                Recipient = "Người nhận",
                Summary = "Nội dung mới"
            };
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
                FileName = $"VanBanDi_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };
            if (dialog.ShowDialog() == true)
            {
                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    var ws = workbook.Worksheets.Add("OutgoingDocs");

                    // Header
                    ws.Cell(1, 1).Value = "ID";
                    ws.Cell(1, 2).Value = "Số/Ký hiệu";
                    ws.Cell(1, 3).Value = "Ngày VB";
                    ws.Cell(1, 4).Value = "Loại VB";
                    ws.Cell(1, 5).Value = "Độ mật";
                    ws.Cell(1, 6).Value = "Nơi gửi";
                    ws.Cell(1, 7).Value = "Cán bộ xử lý";
                    ws.Cell(1, 8).Value = "Người ký";
                    ws.Cell(1, 9).Value = "Chức vụ";
                    ws.Cell(1, 10).Value = "Người nhận";
                    ws.Cell(1, 11).Value = "Nội dung";

                    // Data
                    int row = 2;
                    foreach (var doc in FilteredDocs)
                    {
                        ws.Cell(row, 1).Value = doc.Id;
                        ws.Cell(row, 2).Value = doc.DocumentNumber;
                        ws.Cell(row, 3).Value = doc.DocumentDate?.ToString("dd/MM/yyyy");
                        ws.Cell(row, 4).Value = doc.DocumentType;
                        ws.Cell(row, 5).Value = doc.SecurityLevel;
                        ws.Cell(row, 6).Value = doc.Sender;
                        ws.Cell(row, 7).Value = doc.Handler;
                        ws.Cell(row, 8).Value = doc.Signer;
                        ws.Cell(row, 9).Value = doc.SignerPosition;
                        ws.Cell(row, 10).Value = doc.Recipient;
                        ws.Cell(row, 11).Value = doc.Summary;
                        row++;
                    }
                    //Style header
                    var headerRange = ws.Range("A1:K1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray; 

                    // Border in excel
                    var dataRange = ws.Range($"A1:K{row - 1}");
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
