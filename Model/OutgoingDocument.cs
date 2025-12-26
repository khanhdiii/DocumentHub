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
        public string SecurityLevel { get; set; } = ""; 
        public string Sender { get; set; } = "";
        public ConstructionStaff Handler { get; set; }
        public Signer Signer { get; set; } 
        public string Recipient { get; set; } = ""; 
        public string Summary { get; set; } = "";

    }
}
