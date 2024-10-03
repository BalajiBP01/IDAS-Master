using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class ConsumerJudgement
    {
        public long ConsumerID { get; set; }

        [Key]
        public long ConsumerJudgementID { get; set; }

        public int? CourtCaseID { get; set; }

        [StringLength(13)]
        public string IDNo { get; set; }

        [StringLength(16)]
        public string PassportNo { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string SecondName { get; set; }

        [StringLength(100)]
        public string ThirdName { get; set; }

        [StringLength(100)]
        public string Surname { get; set; }

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

        [Required]
        [StringLength(50)]
        public string CaseNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CaseFilingDate { get; set; }

        [StringLength(250)]
        public string CaseReason { get; set; }

        [StringLength(100)]
        public string CaseType { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal? DisputeAmt { get; set; }

        [StringLength(100)]
        public string CourtName { get; set; }

        [StringLength(100)]
        public string CourtCity { get; set; }

        [StringLength(100)]
        public string CourtType { get; set; }

        [StringLength(200)]
        public string PlaintiffName { get; set; }

        [StringLength(100)]
        public string PlaintiffAddress1 { get; set; }

        [StringLength(100)]
        public string PlaintiffAddress2 { get; set; }

        [StringLength(100)]
        public string PlaintiffAddress3 { get; set; }

        [StringLength(100)]
        public string PlaintiffAddress4 { get; set; }

        [StringLength(10)]
        public string PlaintiffPostalCode { get; set; }

        [StringLength(200)]
        public string AttorneyName { get; set; }

        [StringLength(5)]
        public string AttorneyTelephoneCode { get; set; }

        [StringLength(50)]
        public string AttorneyTelephoneNo { get; set; }

        [StringLength(5)]
        public string AttorneyFaxCode { get; set; }

        [StringLength(50)]
        public string AttorneyFaxNo { get; set; }

        [StringLength(100)]
        public string AttorneyAddress1 { get; set; }

        [StringLength(100)]
        public string AttorneyAddress2 { get; set; }

        [StringLength(100)]
        public string AttorneyAddress3 { get; set; }

        [StringLength(100)]
        public string AttorneyAddress4 { get; set; }

        [StringLength(10)]
        public string AttorneyPostalCode { get; set; }

        [StringLength(50)]
        public string ReferenceNo { get; set; }

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

        [StringLength(10)]
        public string JudgementTypeCode { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeResolvedDate { get; set; }

        [StringLength(50)]
        public string FAX_ID { get; set; }

        [StringLength(50)]
        public string PARTYID { get; set; }

        public bool? Rescinded { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RescissionDate { get; set; }

        [StringLength(100)]
        public string RescissionReason { get; set; }

        [StringLength(50)]
        public string RescissionType { get; set; }

        [StringLength(50)]
        public string RescindedAmount { get; set; }

        [StringLength(50)]
        public string FPP { get; set; }

        public long? NewconsumerID { get; set; }

        [StringLength(1000)]
        public string InternalXDSComment { get; set; }

        [StringLength(500)]
        public string SubscriberName { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(500)]
        public string FileName { get; set; }
    }
}
