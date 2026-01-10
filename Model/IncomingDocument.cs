namespace DocumentHub.Model
{
    public class IncomingDocument
    {
        public int Id { get; set; }
        public string ArrivalNumber { get; set; } = "";
        public DateTime? ArrivalDate { get; set; }
        public string DocumentNumber { get; set; } = "";
        public DateTime? DocumentDate { get; set; }
        public string SecurityLevel { get; set; } = "";
        public string DocumentType { get; set; } = "";
        public string Sender { get; set; } = "";
        public string Position { get; set; } = "";
        public string Summary { get; set; } = "";

        // FK properties
        public int? ConstructionStaffId{ get; set;}
        public int? ReceivingOfficerId{   get; set; }
        public int? SignerId {get; set; }
        public int? RecipientId{ get; set;}

        // Navigation properties
        public Signer Signer { get; set; }

        public Recipient Recipient { get; set; }

        public ReceivingOfficer ReceivingOfficer { get; set; }

        public ConstructionStaff ConstructionStaff { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt
        {
            get; set;
        }

    }
}
