using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class ProFormaInvoice
    {
        [Key]
        public Guid ID { get; set; }
        public int ProFormaInvoiceNumber { get; set; }
        public bool IsProformal { get; set; }
        public DateTime Date { get; set; }
        public int ReferenceNumber { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VatTotal { get; set; }
        public decimal Total { get; set; }
        public List<ProformaInvoiceLineItem> ProformaInvoiceLineItems { get; set; } = new List<ProformaInvoiceLineItem>();
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public bool IsSubmitted { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public bool IsEmailSend { get; set; }
        public DateTime EmailSendDate { get; set; }
        public Guid? InvoiceId { get; set; }
        public string ProformaDisplyNumber { get; set; }
    }
}
