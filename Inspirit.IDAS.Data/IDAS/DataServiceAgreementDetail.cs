using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class DataServiceAgreementDetail
    {
        public Guid Id { get; set; }
        public DataServicesAgreement DataServicesAgreement { get; set; }
        public Guid DataServicesAgreementId { get; set; }
        public int SNo { get; set; }
        public string Description { get; set; }
    }
}
