using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class CommercialAuditor
    {        
        public int CommercialID { get; set; }

        [Key]        
        public int CommercialAuditorID { get; set; }

        [StringLength(150)]
        public string AuditorName { get; set; }

        [StringLength(150)]
        public string AuditorNameSoundex { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActStartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActEndDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActExpiryDate { get; set; }

        [StringLength(10)]
        public string ReferenceNo { get; set; }
        
        public bool IsWithdrawnFromPublicYN { get; set; }
        
        [StringLength(10)]
        public string ActIndMPYNo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RegisteredEntryDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CM31Date { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ReceiveDate { get; set; }

        public int? FineLetter { get; set; }

        [Required]
        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        
        public int? CIPROAUDRecordChecksum { get; set; }
        
        public int? LoaderID { get; set; }
        
        public int? SubscriberID { get; set; }

        [StringLength(10)]
        public string ProfessionCode { get; set; }

        [StringLength(10)]
        public string AuditorTypeCode { get; set; }

        [StringLength(10)]
        public string AuditorStatusCode { get; set; }

        [StringLength(10)]
        public string ProfessionNo { get; set; }
        
        public int? AuditorID { get; set; }

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
