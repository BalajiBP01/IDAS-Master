using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class LeadsGenaration
    {
        public Guid ID { get; set; }
        public Guid CustomerUserID { get; set; }
        public DateTime LeadsDate { get; set; }
        public int RequestedRecors { get; set; }
        public bool? AdminCertified { get; set; }
        public Guid? ProFormaInvoiceId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime? ApprovedOnDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public int LeadsNumber { get; set; }
        public string OutPutFileName { get; set; }
        public string InputDetail { get; set; }
        public string OutputDetail { get; set; }
        public string MaritalStaus { get; set; }
        public string RiskCategories { get; set; }
        public string AlloyBreakDowns { get; set; }
        public string LocationServices { get; set; }
        public string AgeGroupGenders { get; set; }
        public string ProfileGender { get; set; }
        public string IncomeBrackets { get; set; }
        public string TotalRecordsAvailable { get; set; }
        public bool ProfileReport { get; set; }
    }
}
