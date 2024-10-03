using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class CompanyProfile
    {
        public int commercialId { get; set; }
        public string registrationNo { get; set; }
        public string registrationNoOld { get; set; }
        public string commercialName { get; set; }
        public string commercialShortName { get; set; }
        public string commercialTranslatedName { get; set; }
        public string previousBusinessname { get; set; }
        public DateTime? registrationDate { get; set; }
        public DateTime? businessStartDate { get; set; }
        public int? financialYearEnd { get; set; }
        public DateTime? financialEffectiveDate { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
        public string sICCode { get; set; }
        public string businessDesc { get; set; }
        public string commercialStatusCode { get; set; }
        public string commercialTypeCode { get; set; }
        public long? vATNo { get; set; }
        public string bussEmail { get; set; }
        public string bussWebsite { get; set; }
        public DateTime? createdOnDate { get; set; }
        public decimal? noOfShares { get; set; }
        public decimal? amountPerShare { get; set; }
        public int? premium { get; set; }
        public string addressTypeInd { get; set; }
        public string originalAddress1 { get; set; }
        public string originalAddress2 { get; set; }
        public string originalAddress3 { get; set; }
        public string originalAddress4 { get; set; }
        public string originalPostalCode { get; set; }
                       
        public string tabSelected { get; set; }

        public List<ContactDetail> contacts { get; set; } = new List<ContactDetail>();
        public List<AddressDetail> addresses { get; set; } = new List<AddressDetail>();
        public List<DirectorShip> directorShips { get; set; } = new List<DirectorShip>();
        public List<CommercialJudgement> commercialJudgements { get; set; } = new List<CommercialJudgement>();
        public List<PropertyDeedDetail> propertyOwners { get; set; } = new List<PropertyDeedDetail>();
        public List<CommercialAuditorVm> commercialAuditors { get; set; } = new List<CommercialAuditorVm>();
        public List<TimeLine> timelines { get; set; } = new List<TimeLine>();


        public class CommercialAuditorVm
        {
            public int CommercialID { get; set; }
            public virtual Commercial Commercial { get; set; }
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

            public List<AuditorAddressVM> auditoraddresess = new List<AuditorAddressVM>();
        }
        public class AuditorAddressVM
        {
            public int? AuditorID { get; set; }
            public Auditor Auditor { get; set; }
            public int AuditorAddressID { get; set; }
            public string AddressTypeInd { get; set; }
            public string OriginalAddress1 { get; set; }
            public string OriginalAddress2 { get; set; }
            public string OriginalAddress3 { get; set; }
            public string OriginalAddress4 { get; set; }
            public string OriginalPostalCode { get; set; }
            public string AuditorFullAddress { get; set; }
            public DateTime? LastUpdatedDate { get; set; }
            public DateTime? CreatedOnDate { get; set; }
        }

    }
}
