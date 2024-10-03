using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.ESData
{
    [ElasticsearchType(Name = "commercialdata", IdProperty = "CommercialID")]
    public class CommercialData
    {
        [Number]
        public int CommercialID { get; set; }

        [Text(Name = "companyregnumber",Analyzer = "keyword", Fielddata = true)]
        public string CompanyRegNumber { get; set; }

        [Text(Name = "companyregno", Fielddata = true)]
        public string CompanyRegNo { get; set; }

        [Text(Name = "companyname", Fielddata = true)]
        public string CompanyName { get; set; }
        
        [Text(Name = "commercialstatuscode", Fielddata = true)]
        public string CommercialStatusCode { get; set; }

        [Date(Name = "businessstartdate")]
        public DateTime? BusinessStartDate { get; set; }

        [Date(Name = "lastupdateddate")]
        public DateTime? LastUpdatedDate { get; set; }

        [Text(Name = "commercialaddress")]
        public string[] CommercialAddress { get; set; }

        [Text(Name = "commercialtelephone")]
        public string[] CommercialTelephone { get; set; }

    }
}
