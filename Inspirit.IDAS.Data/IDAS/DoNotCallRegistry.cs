using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class DoNotCallRegistry
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Emailid { get; set; }
        public string Idnumber { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CurrentDate { get; set; }

        public bool IsApproved { get; set; }


    }
}
