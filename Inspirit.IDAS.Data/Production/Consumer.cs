using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Production
{
    public class Consumer
    {
        [Key]
        public long ConsumerID { get; set; }

        public string IDNO { get; set; }

        public string PassportNo { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string ThirdName { get; set; }

        public string Surname { get; set; }

        public DateTime? BirthDate { get; set; }

        public string MaidenName { get; set; }

        public byte? GenderInd { get; set; }

        public byte RecordStatusInd { get; set; }
        public string TitleCode { get; set; }
        public DateTime? CreatedOnDate { get; set; }

        public string FirstInitial { get; set; }

        public DateTime LastUpdatedDate { get; set; }


      
        public string Alloy { get; set; }
        public string LSM { get; set; }

        public List<ConsumerHomeAffair> ConsumerHomeAffairs { get; set; } = new List<ConsumerHomeAffair>();

        public List<ConsumerAddress> ConsumerAddress { get; set; } = new List<ConsumerAddress>();

        public List<ConsumerEmploymentOccupation> ConsumerEmploymentOccupations { get; set; } = new List<ConsumerEmploymentOccupation>();

        public List<ConsumerTelephone> ConsumerTelephone { get; set; } = new List<ConsumerTelephone>();

        public List<ConsumerEmailConfirmed> ConsumerEmails { get; set; } = new List<ConsumerEmailConfirmed>();

        public List<ConsumerDebtReview> ConsumerDebtReviews { get; set; } = new List<ConsumerDebtReview>();

        public List<ConsumerJudgement> ConsumerJudgements { get; set; } = new List<ConsumerJudgement>();
        public bool IsESSynced { get; set; }
    }


    public class LSM
    {
        [Key]
        public long CONSUMERID { get; set; }
        public string IDNO { get; set; }
        public string RiskCategory { get; set; }
        public string IncomeCategory { get; set; }

    }

    public class ConsumerHomeAffair
    {
        [Key]
        public int HomeAffairsID { get; set; }

        public string IDNo { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string ThirdName { get; set; }
        public string Surname { get; set; }
        public string NameCombo { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool HomeAffairsRunYN { get; set; }
        public string HomeAffairsMessage { get; set; }

        public string RecordStatusInd { get; set; }
        public string DeletedReason { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? HARecordChecksum { get; set; }
        public int? SubscriberID { get; set; }
        public int? LoaderID { get; set; }
        public long ConsumerID { get; set; }
        public Consumer Consumer { get; set; }

        public string CreatedByUser { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string ChangedByUser { get; set; }
        public DateTime? ChangedOnDate { get; set; }
        public bool? IsPossibleDuplicateRecordYN { get; set; }
        public bool? IsPossibleNameConflictYN { get; set; }

        public string MaidenName { get; set; }
        public DateTime? IDIssuedDate { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string PlaceOfMarriage { get; set; }
        public string SpouseIdnoOrDOB { get; set; }
        public string SpouseSurName { get; set; }
        public string SpouseForeNames { get; set; }
        public DateTime? DivorceDate { get; set; }
        public string DivorceIssuedCourt { get; set; }
        public DateTime? DeceasedDate { get; set; }
        public string DeceasedStatus { get; set; }
        public string PlaceOfDeath { get; set; }
        public string CauseOfDeath { get; set; }


    }

    public class ConsumerAddress
    {
        [Key]
        public long ConsumerAddressID { get; set; }
        public long ConsumerID { get; set; }
        public Consumer Consumer { get; set; }


        public int AddressTypeInd { get; set; }
        public string OriginalAddress1 { get; set; }
        public string OriginalAddress2 { get; set; }
        public string OriginalAddress3 { get; set; }
        public string OriginalAddress4 { get; set; }
        public string OriginalPostalCode { get; set; }
        public string OccupantTypeInd { get; set; }
        public int RecordStatusInd { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public DateTime? CreatedOnDate { get; set; }
        public string Town { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public DateTime? ChangedOnDate { get; set; }   
    }

    public class ConsumerEmploymentOccupation
    {
        [Key]
        public int ConsumerEmploymentOccupationID { get; set; }
        public long? ConsumerID { get; set; }
        public Consumer Consumer { get; set; }
        public int? ConsumerEmploymentID { get; set; }
        public string Occupation { get; set; }
        public string employer { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
    }

    public class ConsumerTelephone
    {
        [Key]
        public long ConsumerTelephoneID { get; set; }

        public long ConsumerID { get; set; }
        public Consumer Consumer { get; set; }
        public long TelephoneID { get; set; }

        public Telephone Telephone { get; set; }

        public byte? TelephoneTypeInd { get; set; }
        public byte RecordStatusInd { get; set; }

        public DateTime CreatedonDate { get; set; }

        public DateTime? ChangedonDate { get; set; }

        public DateTime LastUpdatedDate { get; set; }

    }
    public class ConsumerEmailConfirmed
    {
        [Key]
        public long ID { get; set; }

        [StringLength(200)]
        public string EmailID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        public long ConsumerID { get; set; }
        public Consumer Consumer { get; set; }

        public byte RecordStatusInd { get; set; }
    }

    public class ConsumerDebtReview
    {
        [Key]
        public long ConsumerDebtReviewID { get; set; }

        public long ConsumerID { get; set; }
        public Consumer Consumer { get; set; }

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

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        [StringLength(10)]
        public string DebtReviewStatusCode { get; set; }
    }

    public class ConsumerJudgement
    {
        [Key]
        public long ConsumerJudgementID { get; set; }

        public long ConsumerID { get; set; }

        [StringLength(13)]
        public string IDNo { get; set; }

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

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(10)]
        public string JudgementTypeCode { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeResolvedDate { get; set; }

        public bool? Rescinded { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RescissionDate { get; set; }

        [StringLength(100)]
        public string RescissionReason { get; set; }

        [StringLength(50)]
        public string RescindedAmount { get; set; }
    }


    public class Address
    {
        public Guid ID { get; set; }

        public string Type { get; set; } //Consumer,Commercial
                                         // public int ConsumerID { get; set; }

        public long? ConsumerAddressID { get; set; }
        public ConsumerAddress ConsumerAddress { get; set; }
        public long? CommercialAddressID { get; set; }
        public CommercialAddress CommercialAddress { get; set; }
        public long? DirectorAddressID { get; set; }
        public DirectorAddress DirectorAddress { get; set; }

        public string AddressDetial { get; set; }

        public string PostalCode { get; set; }

        public string Province { get; set; }

        public string Region { get; set; }

        public string Suburb { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class Provinces
    {
        public int ID { get; set; }

        public int Code_Start { get; set; }

        public int Code_End { get; set; }

        public string Province { get; set; }

        public string Region { get; set; }
    }

    public class Postalcode
    {
        public int ID { get; set; }

        public string Suburb { get; set; }

        public string Area { get; set; }

        public string Box_Code { get; set; }

        public string Str_Code { get; set; }
    }

    public class ConsumerEmployment
    {

        public long ConsumerID { get; set; }

        [Key]
        public long ConsumerEmploymentID { get; set; }
        public string EmployerDetail { get; set; }
        public byte RecordStatusInd { get; set; }
   
        public DateTime LastUpdatedDate { get; set; }

        public DateTime CreatedOnDate { get; set; }
    }
    public class ConsumerOccupation
    {
        [Key]
        public int ConsumerEmploymentOccupationID { get; set; }

        public int? ConsumerID { get; set; }

        public int? ConsumerEmploymentID { get; set; }
        public string Occupation { get; set; }

        public byte RecordStatusInd { get; set; }
      
        public DateTime? LastUpdatedDate { get; set; }
        
        public DateTime? CreatedOnDate { get; set; }
    }
}
