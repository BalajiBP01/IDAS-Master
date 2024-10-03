using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.ESData
{
    [ElasticsearchType(Name = "consumerindex", IdProperty = "consumerid")]
    public class ConsumerIndex
    {
        [Text]
        public string consumerid { get; set; }

        [Text(Name = "idnumber", Analyzer = "standard", Fielddata = true)]
        public string IDNumber { get; set; }

        [Text(Name = "passportno")]
        public string PassportNo { get; set; }

        [Text(Name = "surname", Analyzer = "standard")]
        public string Surname { get; set; }

        [Text(Name = "firstname", Analyzer = "standard", Fielddata = true)]
        public string Firstname { get; set; }
        [Date(Name = "dateofbirth")]
        public DateTime? DateOfBirth { get; set; }

        [Text(Name = "gender", Fielddata = true)]
        public string Gender { get; set; }
        [Boolean(Name = "isdeceased")]
        public bool isDeceased { get; set; }
        [Text(Name = "maritalstatus")]
        public string MaritalStatus { get; set; }

        [Text(Name = "riskcategory")]
        public string RiskCategory { get; set; }//from lsm
        [Text(Name = "alloy")]
        public string Alloy { get; set; }
        [Text(Name = "province")]
        public string Province { get; set; }
        [Text(Name = "incomercategoty")]
        public string IncomerCategoty { get; set; }//from lsm and compare with lookupdata

        [Text(Name = "address", Analyzer = "english", Fielddata = true)]
        public string[] Address { get; set; }
        [Text(Name = "phonenumber", Analyzer = "standard", Fielddata = true)]
        public string[] PhoneNumber { get; set; }

        [Text(Name = "email")]
        public string[] Email { get; set; }
        
        [Text(Name = "town")]
        public string Town { get; set; }
        [Text(Name = "region")]
        public string Region { get; set; }
        [Text(Name = "lsm")]
        public string LSM { get; set; }
        [Text(Name = "emailindicator")]
        public string EmailIndicator { get; set; }
        [Text(Name = "deedsindicator")]
        public string DeedsIndicator { get; set; }
        [Text(Name = "adverseindicator")]
        public string AdverseIndicator { get; set; }
        [Text(Name = "directorindicator")]
        public string DirectorIndicator { get; set; }
        [Text(Name = "cellindicator")]
        public string CellIndicator { get; set; }
        [Text(Name = "employmentindicator")]
        public string EmploymentIndicator { get; set; }

        [Text(Name = "iscontactexists")]
        public string Iscontactexists { get; set; }
        [Text(Name = "isleadeligible")]
        public string Isleadeligible { get; set; }
    }
}
