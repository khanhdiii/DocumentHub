using DocumentHub.Model;
using System.Collections.ObjectModel;

namespace DocumentHub.ViewModel
{
    class IncomingDocViewModel
    {
        public ObservableCollection<IncomingDocument> IncomingDocs { get; set; }

        public IncomingDocViewModel()
        {
            IncomingDocs =
            [
                new IncomingDocument
                {
                    Id = 1,
                    ArrivalNumber = "123",
                    ArrivalDate = DateTime.Today,
                    DocumentNumber = "VB-001",
                    DocumentDate = DateTime.Today.AddDays(-1),
                    SecurityLevel = "Bình thường",
                    DocumentType = "Công văn",
                    Sender = "Sở TT&TT",
                    Signer = "Nguyễn Văn A",
                    Position = "Giám đốc",
                    Recipient = "Phòng HC",
                    Handler = "Trần Văn B",
                    Summary = "Nội dung công văn về ..."
                }
            ];
        }
    }
}
