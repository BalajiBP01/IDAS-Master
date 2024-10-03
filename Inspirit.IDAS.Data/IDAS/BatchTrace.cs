using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class BatchTrace
    {
        public Guid ID { get; set; }

        public CustomerUser CustomerUser { get; set; }

        public Guid CustomerUserID { get; set; }

        public string FileName { get; set; }
        public string OutPutFileName { get; set; }
        public int BatchNumber { get; set; }
        public DateTime UploadDate { get; set; }

        public int TotalRecords { get; set; }

        public int FoundRecords { get; set; }

        public string AgeGroupGenders { get; set; }
        public string ProfileGender { get; set; }
        public string IncomeBrackets { get; set; }
        public string MaritalStaus { get; set; }
        public string RiskCategories { get; set; }
        public string AlloyBreakDowns { get; set; }
        public string LocationServices { get; set; }
        public string TotalRecordsAvailable { get; set; }

        public bool IsDataDownloaded { get; set; }
        public Guid? ProFormaInvoiceId { get; set; }
        public bool AdminCertified { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime ApprovedOnDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string IdNumbers { get; set; }
    }
}
