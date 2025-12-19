namespace DocumentHub.Model
{
    class IncomingDocument
    {
        public int Id { get; set; }
        public string ArrivalNumber { get; set; } = "";
        public DateOnly ArrivalDate { get; set; }
        public string DocumentNumber { get; set; } = "";
        public DateOnly DocumentDate { get; set; }
        public string SecurityLevel { get; set; } = "";
        public string DocumentType { get; set; } = "";
        public string Sender { get; set; } = "";
        public string Signer { get; set; } = "";
        public string Position { get; set; } = "";
        public string Recipient { get; set; } = "";
        public string Handler { get; set; } = "";
        public string Summary { get; set; } = "";
    }
}
