using Inspirit.IDAS.Data.IDAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class Payment
    {

        public Guid Id { get; set; }


        public Invoice Invoice { get; set; }


        public Guid InvoiceId { get; set; }


        public Customer Customer { get; set; }


        public Guid CustomerId { get; set; }


        public int Number { get; set; }

        public DateTime Date { get; set; }


        public string PaymentType { get; set; }

        public string Reference { get; set; }

        public decimal Amount { get; set; }

        public decimal? PaymentAmountReceive { get; set; }
        public DateTime? PaymentReceivedDate { get; set; }
        public string Comments { get; set; }
    }
}
