using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Production
{
    public class Telephone
    {
       
        [Key]
        public long TelephoneID { get; set; }

        public string InternationalDialingCode { get; set; }

        public string TelephoneCode { get; set; }

        public long TelephoneNo { get; set; }
        public byte RecordStatusInd { get; set; }
    
        public DateTime? CreatedonDate { get; set; }

    }
}
