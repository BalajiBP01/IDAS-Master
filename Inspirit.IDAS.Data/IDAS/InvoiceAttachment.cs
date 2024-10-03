using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS
{
    public class InvoiceAttachment
    {
        [Key]
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string Attachment { get; set; }
        public Guid? InvoiceId { get; set; }
    }
}
