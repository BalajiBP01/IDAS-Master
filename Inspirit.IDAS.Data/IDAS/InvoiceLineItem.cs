using System;
using System.ComponentModel.DataAnnotations;

namespace Inspirit.IDAS.Data.IDAS
{
    public class InvoiceLineItem
    {
      
        public Guid ID { get; set; }
 
        public Guid InvoiceID { get; set; }

        public ProductPackageRate ProductPackageRate { get; set; }

        public Guid? ProductPackageRateID { get; set; }


        [StringLength(500)]
        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal NetAmount { get; set; }

        public decimal VatAmount { get; set; }

        [StringLength(20)]
        public string BillingType { get; set; } //Monthly,One Time Payment
        [StringLength(20)]
        public string UsageType { get; set; } //Yearly,Monthly,Credits

        public Subscription Subscription { get; set; }

        public Guid? SubscriptionID { get; set; }
    }
   
}
