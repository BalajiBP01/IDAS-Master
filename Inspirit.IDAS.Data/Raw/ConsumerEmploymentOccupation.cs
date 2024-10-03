using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class ConsumerEmploymentOccupation
    {
        [Key]
        public int ConsumerEmploymentOccupationID { get; set; }

        public int? ConsumerID { get; set; }

        public int? ConsumerEmploymentID { get; set; }

        [StringLength(50)]
        public string Occupation { get; set; }

        public byte? RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        
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
