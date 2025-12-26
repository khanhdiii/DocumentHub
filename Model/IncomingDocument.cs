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
        public Signer Signer { get; set; }
        public string Position { get; set; } = "";
        public string Recipient { get; set; } = "";
        public ConstructionStaff ConstructionStaff { get; set; }
        public string Summary { get; set; } = "";
    }
}
