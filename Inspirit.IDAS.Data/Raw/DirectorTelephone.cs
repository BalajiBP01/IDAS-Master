using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class DirectorTelephone
    {        
        public int DirectorID { get; set; }

        [Key]        
        public int DirectorTelephoneID { get; set; }

        [Required]
        [StringLength(1)]
        public string TelephoneTypeInd { get; set; }

        [StringLength(5)]
        public string TelephoneCode { get; set; }

        [StringLength(50)]
        public string TelephoneNo { get; set; }

        [Required]
        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }
        
        public int? CIPROREGRecordChecksum { get; set; }
        
        public int? LoaderID { get; set; }
        
        public int? SubscriberID { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }

        [StringLength(20)]
        public string FileRef { get; set; }
    }
}
