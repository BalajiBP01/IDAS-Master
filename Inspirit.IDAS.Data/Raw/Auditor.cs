using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public class Auditor
    {
        [Key]
        public int AuditorID { get; set; }
        public string AuditorName { get; set; }
    }
}
