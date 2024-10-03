using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class ConsumerTelephone
    {
        [Key]
        public long ConsumerTelephoneID { get; set; }

        public long ConsumerID { get; set; }

        public long TelephoneID { get; set; }

        public byte TelephoneTypeInd { get; set; }

        public byte RecordStatusInd { get; set; }

        [Column(TypeName = "date")]
        public DateTime? AccountopenedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FirstReportedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedbyUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedonDate { get; set; }

        [StringLength(50)]
        public string ChangedbyUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedonDate { get; set; }

        public bool IsVerifiedYN { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? VerifiedDate { get; set; }

        public int SubscriberID { get; set; }

        public int LoaderID { get; set; }

        public int? ChangedbyLoaderID { get; set; }
    }
}
