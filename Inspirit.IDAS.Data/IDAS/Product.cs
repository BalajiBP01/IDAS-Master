using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<ProductPackageRate> PackageRates { get; set; } = new List<ProductPackageRate>();
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ActivatedDate { get; set; }
        public DateTime DeactivatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public Service Service { get; set; }
        public Guid ServiceId { get; set; }
        public string UsageType { get; set; } // monthly,yearly,credits      
        public bool IsPostpaid { get; set; }
        public string Code { get; set; }
        public bool? BatchProduct { get; set; }
        public bool? LeadProduct { get; set; }
    }
}
