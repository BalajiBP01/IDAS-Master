using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{


    public class Workorder
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SubscriptionItem SubscriptionItem { get; set; }

        public Guid? SubscriptionItemID { get; set; }

        public ProductPackageRate ProductPackage { get; set; }

        public Guid ProductPackageId { get; set; }


        public Customer Customer { get; set; }

        public Guid CustomerId { get; set; }

      
        public string Status { get; set; }

        public string ServiceType { get; set; }


        public int Credits { get; set; }
        public bool isCancelled { get; set; }
        public Guid? InvoiceLineItemId { get; set; }

    }
   public enum Status
    {
        Active,
        Inactive
    }


   
}
