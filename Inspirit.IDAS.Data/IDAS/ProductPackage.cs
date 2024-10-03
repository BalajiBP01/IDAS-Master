using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class ProductPackage
    {
        public Guid Id { get; set; }
        public string UsageType { get; set; } // per user/ per search

        public DateTime EffectiveDate;
        public ProductCategory ProductCategory { get; set; }
        public Guid ProductCategoryId { get; set; }


        public Product Product { get; set; }
        public Guid ProductId { get; set; }


        public ProductDataType ProductDataType { get; set; }
        public Guid ProductDataTypeId { get; set; }


        public int DayLimit { get; set; } // required only tracing


        
        public List<ProductPackageRate> ProductPackageRates { get; set; } 

        public ProductPackage()
        {
            ProductPackageRates = new List<ProductPackageRate>();
        }

        ///Api integration --- TODO
    }
}
