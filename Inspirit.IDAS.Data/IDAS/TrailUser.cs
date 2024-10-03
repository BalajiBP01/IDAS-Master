using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class TrailUser
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string BusinessRegisterNumber { get; set; }
        public string Password { get; set; }
        public DateTime Date { get; set; }
        public bool isExpired { get; set; }
        public int Credits { get; set; }
    }
}
