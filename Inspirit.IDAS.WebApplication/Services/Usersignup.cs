using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication.Services
{

    public class SignUpRequest
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
      
        public string title { get; set; }
        public string iDNumber { get; set; }
        public string emailid { get; set; }
        public string billEmailadress { get; set; }
        public string tradingName { get; set; }
        public string registrationName { get; set; }
        public string registrationNo { get; set; }
        public string vaTNumber { get; set; }
        public string branchLocation { get; set; }
        public string physicalAddress { get; set; }
        public string typeofBusiness { get; set; }
        public string telephoneNumber { get; set; }
        public string faxNumber { get; set; }
        public string DSAVersion { get; set; }
    }

    public class SignUpResponse
    {
        public bool isSucsess { get; set; }
        public string errorMessage { get; set; }
        public string token { get; set; }
    }
}
