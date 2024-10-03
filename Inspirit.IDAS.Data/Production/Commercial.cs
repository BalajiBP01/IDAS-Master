using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class Commercial
    {
        [Key]
        public int CommercialID { get; set; }

        public string RegistrationNo { get; set; }
        public string RegistrationNoOld { get; set; }
        public string CommercialName { get; set; }
        public string CommercialShortName { get; set; }
        public string CommercialTranslatedName { get; set; }
        public string PreviousBusinessname { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? BusinessStartDate { get; set; }
        public int? FinancialYearEnd { get; set; }
        public DateTime? FinancialEffectiveDate { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public string SICCode { get; set; }
        public string BusinessDesc { get; set; }
        public string CommercialStatusCode { get; set; }
        public string CommercialTypeCode { get; set; }
        public long? VATNo { get; set; }
        public string BussEmail { get; set; }
        public string BussWebsite { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public string RecordStatusInd { get; set; }
      
        public bool IsESSynced { get; set; }
        

        //Commercial Capital
        [Column(TypeName = "numeric(28, 9)")]
        public decimal? NoOfShares { get; set; }

        [Column(TypeName = "numeric(19, 1)")]
        public decimal? AmountPerShare { get; set; }

        public int? Premium { get; set; }

        [StringLength(20)]
        public string IDNo { get; set; }      

        public List<CommercialAddress> CommercialAddresses { get; set; }
        public List<CommercialDirector> CommercialDirectors { get; set; }
        public List<CommercialTelephone> CommercialTelephones { get; set; }
        public List<CommercialAuditor> CommercialAuditors { get; set; }
        public List<CommercialJudgement> CommercialJudgements { get; set; }

    }


    public class CommercialAddress
    {
        public int CommercialID { get; set; }
        public Commercial Commercial { get; set; }

        [Key]
        public int CommercialAddressID  { get; set; }
        public string AddressTypeInd { get; set; }
        public string province { get; set; }
        public string OriginalAddress1 { get; set; }
        public string OriginalAddress2 { get; set; }
        public string OriginalAddress3 { get; set; }
        public string OriginalAddress4 { get; set; }
        public string OriginalPostalCode { get; set; }
        public string occupantTypeInd { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
    }

    public class CommercialDirector
    {
        public int CommercialID { get; set; }
        public Commercial Commercial { get; set; }

        [Key]
        public int CommercialDirectorID  { get; set; }

        public int? DirectorID { get; set; }
        public Director Director { get; set; }

        public DateTime? AppointmentDate { get; set; }
        public DateTime? DirectorStatusDate { get; set; }
        public bool? IsRSAResidentYN { get; set; }
        public string RegisterNo { get; set; }
        public string TrusteeOf { get; set; }
        public decimal? MemberSize { get; set; }
        public decimal? MemberControlPerc { get; set; }
        public DateTime? DirectorSetDate { get; set; }
        public string Profession { get; set; }
        public string Estate { get; set; }
        public string DirectorDesignationCode { get; set; }
        public string DirectorStatusCode { get; set; }
        public string DirectorTypeCode { get; set; }

        public DateTime? CreatedOnDate { get; set; }
        public char RecordStatusInd { get; set; }

    }
    
    public class CommercialTelephone
    {
        public int CommercialID { get; set; }
        public Commercial Commercial { get; set; }

        [Key]
        public int CommercialTelephoneID { get; set; }
        public string TelephoneTypeInd { get; set; }
        public string TelephoneCode { get; set; }
        public string TelephoneNo { get; set; }
        public string RecordStatusInd { get; set; }
        public string DeletedReason { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
   
    }

    public class CommercialAuditor
    {
      
        public int CommercialID { get; set; }
        public virtual Commercial Commercial { get; set; }

        [Key]
        public int CommercialAuditorID { get; set; }
        public string AuditorName { get; set; }
       
        public DateTime? ActStartDate { get; set; }
        public DateTime? ActEndDate { get; set; }
      
        public DateTime? LastUpdatedDate { get; set; }
   
        public string ProfessionCode { get; set; }
        public string AuditorTypeCode { get; set; }
        public string AuditorStatusCode { get; set; }
        public string ProfessionNo { get; set; }
        public int? AuditorID { get; set; }
      
        public DateTime? CreatedOnDate { get; set; }
    
    }

    public class CommercialJudgement
    {
        public int CommercialID { get; set; }

        [Key]
        public int CommercialJudgmentID { get; set; }

        [StringLength(150)]
        public string CommercialName { get; set; }

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

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DisputeResolvedDate { get; set; }

        public bool? Rescinded { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RescissionDate { get; set; }

        [StringLength(200)]
        public string RescissionReason { get; set; }

        [StringLength(50)]
        public string RescindedAmount { get; set; }
    }


}