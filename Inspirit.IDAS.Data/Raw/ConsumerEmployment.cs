using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class ConsumerEmployment
    {
        
        public long ConsumerID { get; set; }

        [Key]        
        public long ConsumerEmploymentID { get; set; }

        [Required]
        [StringLength(100)]
        public string EmployerDetail { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Accountopeneddate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FirstReportedDate { get; set; }

        public byte RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        public bool IsVerifiedYN { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? VerifiedDate { get; set; }

        public int? SubscriberID { get; set; }
        
        public int? LoaderID { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }

        public int? ChangedbyLoaderID { get; set; }
    }
}
