using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class ProductPackageRate
    {
        public Guid Id { get; set; }
        
        public Product Product { get; set; }
        public Guid ProductId { get; set; }

        public int MinLimit { get; set; }

        public int MaxLimit { get; set; }
        public int IsDeleted { get; set; }
       
        public decimal UnitPrice { get; set; }
    }
}
