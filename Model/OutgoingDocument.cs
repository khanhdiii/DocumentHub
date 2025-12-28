using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DocumentHub.Model
{
   public class OutgoingDocument
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; } = ""; 
        public DateTime? DocumentDate { get; set; }
        public string DocumentType { get; set; } = "";
        public string Summary { get; set; } = "";
        public string SecurityLevel { get; set; } = "";
        public ConstructionStaff ConstructionStaff { get; set; }
        public ReceivingOfficer ReceivingOfficer { get; set; }

        public Signer Signer { get; set; }

        public Recipient Recipient { get; set; }
    }
}
