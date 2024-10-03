using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class CommercialName
    {        
        public int CommercialID { get; set; }

        [Key]        
        public int CommercialNameID { get; set; }

        [StringLength(15)]
        public string RegistrationNo { get; set; }

        [StringLength(15)]
        public string RegistrationNoOld { get; set; }

        [StringLength(15)]
        public string RegistrationNoConverted { get; set; }
        
        [StringLength(50)]
        public string IDNo { get; set; }

        [StringLength(50)]
        public string TrustNo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CommercialTypeDate { get; set; }

        [Required]
        [Column("CommercialName")]
        [StringLength(150)]
        public string CommName { get; set; }

        [StringLength(100)]
        public string CommercialShortName { get; set; }

        [StringLength(150)]
        public string CommercialTranslatedName { get; set; }

        [StringLength(150)]
        public string PreviousBusinessname { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NameChangeDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RegistrationDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? BusinessStartDate { get; set; }
        
        public bool IsWithdrawnFromPublicYN { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CommercialStatusDate { get; set; }

        public int? FinancialYearEnd { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FinancialEffectiveDate { get; set; }

        [Column(TypeName = "numeric(19, 4)")]
        public decimal? AuthorisedCapitalAmt { get; set; }

        [Column(TypeName = "numeric(19, 4)")]
        public decimal? AuthorisedNoOfShares { get; set; }

        [Column(TypeName = "numeric(19, 4)")]
        public decimal? IssuedCapitalAmt { get; set; }

        [Column(TypeName = "numeric(19, 4)")]
        public decimal? IssuedNoOfShares { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CKReceivedDate { get; set; }
        
        public DateTime? CKOnFormDate { get; set; }

        [StringLength(50)]
        public string TaxNo { get; set; }

        [StringLength(50)]
        public string CountryOfOrigin { get; set; }

        [StringLength(3)]
        public string ChangeType { get; set; }

        [Required]
        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }
        
        public int? CIPROREGRecordChecksum { get; set; }
        
        public int? SubscriberID { get; set; }
        
        public int? LoaderID { get; set; }
        
        [StringLength(10)]
        public string SICCode { get; set; }

        [StringLength(500)]
        public string BusinessDesc { get; set; }

        [StringLength(10)]
        public string CommercialStatusCode { get; set; }

        [StringLength(10)]
        public string CommercialTypeCode { get; set; }

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

        [StringLength(2)]
        public string DataSource { get; set; }
    }
}
