using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class Invoice
    {
        [Key]
        public Guid ID { get; set; }

        public int InvoiceNumber { get; set; }
                
        public bool ProFormaInvoice { get; set; }

        public DateTime Date { get; set; }

        public int ReferenceNumber { get; set; }
        
        public decimal SubTotal { get; set; }

        public decimal Discount { get; set; }

        public decimal VatTotal { get; set; }

        public decimal Total { get; set; }

        public string BillingType { get; set; }
        public string Remarks { get; set; }
        public List<InvoiceLineItem> InvoiceLineItems { get; set; } = new List<InvoiceLineItem>();
        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
        public Subscription Subscription { get; set; }
        public Guid? SubscriptionID { get; set; }
        public bool ispaid { get; set; }
        public bool IsCreditNoteRaised { get; set; }
        public bool isSubmited { get; set; }
        public bool isCancelled { get; set; }
        public bool IsEmailSend { get; set; }
        public DateTime EmailSendDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceDisplayNumber { get; set; }
        public bool IsTaxinvSent { get; set; }
    }
   
}
