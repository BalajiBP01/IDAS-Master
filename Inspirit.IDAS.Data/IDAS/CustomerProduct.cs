using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class CustomerProduct
    {
        public Guid Id { get; set; }       
        public Guid CustomerId { get; set; }         
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }        
    }
}
