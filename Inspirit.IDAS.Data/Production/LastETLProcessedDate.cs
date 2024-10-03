using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class LastETLProcessedDate
    {
        [Key]
        public DateTime ProcessedDate { get; set; }
    }
}
