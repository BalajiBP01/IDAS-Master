using Inspirit.IDAS.WebApplication.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class PersonProfile
    {
        public string id { get; set; }
        public string iD10 { get; set; }
        public string idNumber { get; set; }
        public string passportNo { get; set; }
        public string firstName { get; set; }
        public string secondName { get; set; }
        public string thirdName { get; set; }
        public string surname { get; set; }
        public DateTime? birthDate { get; set; }
        public string maidenName { get; set; }
        public string genderInd { get; set; }
        public string titleCode { get; set; }
        public DateTime? createdOnDate { get; set; }
        public string firstInitial { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
        public DateTime? deceasedDate { get; set; }
        public string deceasedStatus { get; set; }
        public DateTime? iDIssuedDate { get; set; }
        public DateTime? marriageDate { get; set; }
        public string placeOfMarriage { get; set; }
        public string spouseIdnoOrDOB { get; set; }
        public string spouseSurName { get; set; }
        public string spouseForeNames { get; set; }
        public string spouseIdNumber { get; set; }
        public string nameCombo { get; set; }
        public long spouseConsumerId { get; set; }
        public DateTime? divorceDate { get; set; }
        public string divorceIssuedCourt { get; set; }
        public string placeOfDeath { get; set; }
        public string causeOfDeath { get; set; }
        public string maritalStatus { get; set; }
        public DateTime? lastupdatesHomeAffire { get; set; }
        public string recordstatusind { get; set; }
        public string contactScore { get; set; }
        public string riskScore { get; set; }
        public string lsm { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string tabs { get; set; } 
        public List<ContactDetail> contacts { get; set; } = new List<ContactDetail>();
        public List<AddressDetail> addresses { get; set; } = new List<AddressDetail>();
        public List<Employeement> employees { get; set; } = new List<Employeement>();
        public List<ConsumerEmailConfirmed> emails { get; set; } = new List<ConsumerEmailConfirmed>();
        public List<DirectorShip> directorShips { get; set; } = new List<DirectorShip>();
        public List<PropertyDeedDetail> propertyOwners { get; set; } = new List<PropertyDeedDetail>();
        public List<consumerdebtreview> consumerDebtReview { get; set; } = new List<consumerdebtreview>();
        public List<Consumerjudgement> consumerjudgements { get; set; } = new List<Consumerjudgement>();
        public List<ReletionshipLinkVm> relationships { get; set; } = new List<ReletionshipLinkVm>();
        public List<TimeLine> timelines { get; set; } = new List<TimeLine>();
        // krishna pending
        public List<AKADetail> akadet { get; set; } = new List<AKADetail>();

        public string userName { get; set; }
        public string CompanyName { get; set; }
        public string EnquiryInputIdNo { get; set; }
        public string RefNum { get; set; }
        public string CustomerRefNum { get; set; }
        public DateTime SearchDateTime { get; set; }

        public string SearchCriteria { get; set; }
    }
}