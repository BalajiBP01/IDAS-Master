using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class Creditnote
    {
        [Key]
        public Guid Id { get; set; }
        public Invoice Invoice { get; set; }
        public Guid InvoiceId { get; set; }
        public int CreditNoteNumber { get; set; }
        public decimal CreditNoteValue { get; set; }
        public DateTime CreditNoteDate { get; set; }
        public string Comments { get; set; }
    }
}
