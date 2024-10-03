using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class CommercialDirector
    {        
        public int CommercialID { get; set; }

        [Key]
        public int CommercialDirectorID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? AppointmentDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DirectorStatusDate { get; set; }
        
        public bool IsRSAResidentYN { get; set; }
        
        public bool IsWithdrawnFromPublicYN { get; set; }

        [StringLength(300)]
        public string Executor { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ExecutorAppointmentDate { get; set; }

        [StringLength(14)]
        public string RegisterNo { get; set; }

        [StringLength(300)]
        public string TrusteeOf { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CM29Date { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ReceiveDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CK12Date { get; set; }

        [Column(TypeName = "numeric(19, 4)")]
        public decimal? MemberSize { get; set; }

        [Column(TypeName = "numeric(19, 4)")]
        public decimal? MemberControlPerc { get; set; }

        [StringLength(50)]
        public string MemberControlType { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FineExpiryDate { get; set; }

        [StringLength(100)]
        public string NatureOfChange { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DirectorSetDate { get; set; }

        [StringLength(50)]
        public string Profession { get; set; }

        [StringLength(50)]
        public string Estate { get; set; }

        [StringLength(50)]
        public string OccupationCode { get; set; }

        [StringLength(50)]
        public string NatCode { get; set; }

        [Required]
        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        
        public int? CIPRODIRRecordChecksum { get; set; }
        
        public int? LoaderID { get; set; }
        
        public int? SubscriberID { get; set; }

        [StringLength(10)]
        public string DirectorDesignationCode { get; set; }

        [StringLength(10)]
        public string DirectorStatusCode { get; set; }

        [StringLength(10)]
        public string DirectorTypeCode { get; set; }
        
        public int? DirectorID { get; set; }

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
        
        public bool? XDSDeletedStatus { get; set; }
    }
}
