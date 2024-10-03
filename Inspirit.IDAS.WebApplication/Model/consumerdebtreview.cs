using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication.Model
{
    public class consumerdebtreview
    {
        
        public long consumerID { get; set; }

        
        public long consumerDebtReviewID { get; set; }

   
        public string debtCounsellorRegistrationNo { get; set; }

   
        public string debtCounsellorFirstName { get; set; }

        public string debtCounsellorSurname { get; set; }

      
        public string debtCounsellorTelephoneCode { get; set; }

      
        public string debtCounsellorTelephoneNo { get; set; }


        public DateTime? applicationCreationDate { get; set; }


        public DateTime? debtReviewStatusDate { get; set; }

        public byte recordStatusInd { get; set; }

       
        public string deletedReason { get; set; }


        public DateTime lastUpdatedDate { get; set; }

     
        public string debtReviewStatusCode { get; set; }

        public int? subscriberID { get; set; }

        public int? loaderID { get; set; }

        public string telephoneCodeandNumber { get; set; }
    }
}

