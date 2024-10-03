using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class CustomerDSA
    {
        public Guid Id { get; set; }
        public DataServicesAgreement DataServicesAgreement { get; set; }
        public Guid? DataServicesAgreementId { get; set; }

        public Customer Customer { get; set; }
        public Guid CustomerId { get; set; }
    }
}
