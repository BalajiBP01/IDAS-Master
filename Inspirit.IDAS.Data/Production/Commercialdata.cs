using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class Commercialdata
    {
        public int CommercialID { get; set; }
        public string CompanyRegNumber { get; set; }
        public string CompanyRegNo { get; set; }  
        public string CompanyName { get; set; } 
        public string CommercialStatusCode { get; set; }  
        public DateTime? BusinessStartDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string[] CommercialAddress { get; set; }
        public string[] CommercialTelephone { get; set; }
    }
}
