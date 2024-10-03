using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class CommercialJudgement
    {        
        public int CommercialID { get; set; }

        [Key]        
        public int CommercialJudgmentID { get; set; }

        [StringLength(14)]
        public string RegistrationNo { get; set; }

        [StringLength(150)]
        public string CommercialName { get; set; }

        [StringLength(150)]
        public string CommercialShortName { get; set; }

        [StringLength(150)]
        public string CommercialTranslatedName { get; set; }

        [StringLength(100)]
        public string Address1 { get; set; }

        [StringLength(100)]
        public string Address2 { get; set; }

        [StringLength(100)]
        public string Address3 { get; set; }

        [StringLength(100)]
        public string Address4 { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(5)]
        public string HomeTelephoneCode { get; set; }

        [StringLength(50)]
        public string HomeTelephoneNo { get; set; }

        [StringLength(5)]
        public string WorkTelephoneCode { get; set; }

        [StringLength(50)]
        public string WorkTelephoneNo { get; set; }

        [StringLength(50)]
        public string CellularNo { get; set; }

        [StringLength(5)]
        public string FaxCode { get; set; }

        [StringLength(50)]
        public string FaxNo { get; set; }

        [StringLength(50)]
        public string CaseNumber { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CaseFilingDate { get; set; }

        [StringLength(100)]
        public string CaseType { get; set; }

        [StringLength(250)]
        public string CaseReason { get; set; }

        [Column(TypeName = "decimal(18, 9)")]
        public decimal? DisputeAmt { get; set; }

        [StringLength(200)]
        public string CourtName { get; set; }

        [StringLength(200)]
        public string CourtCity { get; set; }

        [StringLength(200)]
        public string CourtType { get; set; }
        
        public int? CourtCaseID { get; set; }

        [StringLength(200)]
        public string PlaintiffName { get; set; }

        [StringLength(100)]
        public string Plaintiff1Address { get; set; }

        [StringLength(200)]
        public string AttorneyName { get; set; }

        [StringLength(5)]
        public string AttorneyTelephoneCode { get; set; }

        [StringLength(50)]
        public string AttorneyTelephoneNo { get; set; }

        [StringLength(5)]
        public string AttorneyFaxCode { get; set; }

        [StringLength(100)]
        public string AttorneyFaxNo { get; set; }

        [StringLength(200)]
        public string AttorneyAddress1 { get; set; }

        [StringLength(100)]
        public string AttorneyAddress2 { get; set; }

        [StringLength(100)]
        public string AttorneyAddress3 { get; set; }

        [StringLength(100)]
        public string AttorneyAddress4 { get; set; }

        [StringLength(100)]
        public string AttorneyPostalCode { get; set; }

        [StringLength(100)]
        public string ReferenceNo { get; set; }

        [StringLength(50)]
        public string PartyId { get; set; }
        
        public int? FaxID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CapturedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastVerifiedDate { get; set; }

        [StringLength(10)]
        public string Fpp { get; set; }

        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        
        public bool? IsVerifiedYN { get; set; }

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

        [StringLength(10)]
        public string JudgmentTypeCode { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeResolvedDate { get; set; }

        public bool? Rescinded { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RescissionDate { get; set; }

        [StringLength(200)]
        public string RescissionReason { get; set; }

        [StringLength(100)]
        public string RescissionType { get; set; }

        [StringLength(50)]
        public string RescindedAmount { get; set; }
    }
}
