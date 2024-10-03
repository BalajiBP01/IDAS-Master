using System;

namespace Inspirit.IDAS.Data
{
    public class SubscriptionItem
    {
        public Guid Id { get; set; }

        public ProductPackageRate ProductPackage { get; set; }

        public Guid ProductPackageId { get; set; }
        public string Status { get; set; } // Active,Inactive,Update


        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string BillingType { get; set; }// Monthly yearly

        public int Duration { get; set; }

        public int Quantity { get; set; }

        public decimal Rate { get; set; }

        public Subscription Subscription { get; set; }

        public Guid SubscriptionId { get; set; }

        public bool? isBilled { get; set; }

        public decimal ProRataPrice { get; set; }

        public decimal ProRataNetAmount { get; set; }

    }
}
