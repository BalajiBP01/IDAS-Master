using Inspirit.IDAS.Data.IDAS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class CustomerLog
    {
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public Guid CompanyId { get; set; }

        public CustomerUser CustomerUser { get; set; }

        public Guid CompanyUserId { get; set; }

        public int CreditPoints { get; set; }

        public string IdOrPassportNumber { get; set; }

        public string SearchType { get; set; } //Search,Lookup,Batch

        public string SearchCriteria { get; set; }

        public string LogType { get; set; }

        public Workorder Workorder { get; set; }

        public Guid? WorkorderId { get; set; }
        public string InputType { get; set; }

        // Krishna start

        public string CustomerRefNum { get; set; }
        public string VoucherCode { get; set; }
        public string EnquiryReason { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 RefNum { get; set; }

        public bool XDSStatus { get; set; } = true;
        // Krishna end
    }
}
