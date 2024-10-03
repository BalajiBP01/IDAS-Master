using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Inspirit.IDAS.Data.Production
{
    public class Auditor
    {
        [Key]
        public int? AuditorID { get; set; }
        public string AuditorName { get; set; }
    }
    public class AuditorAddress
    {
        public int? AuditorID { get; set; }
        public CommercialAuditor Auditor { get; set; }

        [Key]
        public int AuditorAddressID { get; set; }
        public string AddressTypeInd { get; set; }
        public string OriginalAddress1 { get; set; }
        public string OriginalAddress2 { get; set; }
        public string OriginalAddress3 { get; set; }
        public string OriginalAddress4 { get; set; }
        public string OriginalPostalCode { get; set; }
     
        public DateTime? LastUpdatedDate { get; set; }
     
        public DateTime? CreatedOnDate { get; set; }
     
    }
    public class AuditorHistory
    {
        [Key]
        public int AuditorHistoryID { get; set; }//new
        public int AuditorID { get; set; }

        public Auditor Auditor { get; set; }
        public long? CommercialID { get; set; }

        public long? CommercialAuditorID { get; set; }
        public string AuditorName { get; set; }
        public DateTime? ActStartDate { get; set; }
        public DateTime? ActEndDate { get; set; }

        public DateTime? LoadDate { get; set; }

        public string AuditorStatusCode { get; set; }
       
    }
}
