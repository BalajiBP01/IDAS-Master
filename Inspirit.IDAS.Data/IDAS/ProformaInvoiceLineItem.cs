using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class ProformaInvoiceLineItem
    {
        [Key]
        public Guid ID { get; set; }
        public ProFormaInvoice ProFormaInvoice { get; set; }
        public Guid ProformaInvoiceId { get; set; }
        public Product Product { get; set; }
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }        
        public decimal Amount { get; set; }
    }
}
