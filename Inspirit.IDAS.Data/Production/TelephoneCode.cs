using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class TelephoneCode
    {
        public long TelephoneCodeID { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public string Region { get; set; }
    }
}
    