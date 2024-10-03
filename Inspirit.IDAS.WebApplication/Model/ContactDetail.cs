using System;

namespace Inspirit.IDAS.Data.Production

{
    public class ContactDetail
    {
        //contact fields
        public long id { get; set; }
        public long consumerID { get; set; }
        
        public string type { get; set; }
        public string contact { get; set; }
        public DateTime date { get; set; }
        public string internationalDialingCode { get; set; }
        public string telephoneNo { get; set; }
        public string telephoneCode { get; set; }
        public DateTime? createdonDate { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
        public string email { get; set; }
        public string telephoneTypeInd { get; set; }
        public int peopleLinked { get; set; }

        public DateTime? linkedDate { get; set; }
        public int recordstatusind { get; set; }

        public DateTime? ChangedonDate { get; set; }

        public string TelephoneCodeInformation { get; set; }
        public string CodeType { get; set; }
        public string CodeRegion { get; set; }
        public string ExistingTelephoneNo { get; set; }
    }
}
    


