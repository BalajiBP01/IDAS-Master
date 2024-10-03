using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class ConsumerDebtReview
    {
        public long ConsumerID { get; set; }

        [Key]
        public long ConsumerDebtReviewID { get; set; }

        [StringLength(50)]
        public string DebtCounsellorRegistrationNo { get; set; }

        [StringLength(100)]
        public string DebtCounsellorFirstName { get; set; }

        [StringLength(100)]
        public string DebtCounsellorSurname { get; set; }

        [StringLength(5)]
        public string DebtCounsellorTelephoneCode { get; set; }

        [StringLength(50)]
        public string DebtCounsellorTelephoneNo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ApplicationCreationDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DebtReviewStatusDate { get; set; }

        public byte RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        [StringLength(10)]
        public string DebtReviewStatusCode { get; set; }

        public int? SubscriberID { get; set; }
        
        public int? LoaderID { get; set; }
    }
}
