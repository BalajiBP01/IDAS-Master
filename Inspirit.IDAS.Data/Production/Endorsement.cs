using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class Endorsement
    {
        [Key]
        public int EndorsementID { get; set; }
        public int? PropertyDeedID { get; set; }
        public PropertyDeed propertydeed { get; set; }
        public string EndorsementNumber { get; set; }
        public string EndorsementHolder { get; set; }
        public int? EndorsementAmount { get; set; }
        public string RecordStatusInd { get; set; }
        public string CreateByUser { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string ChangedByUser { get; set; }
        public DateTime? ChangedOnDate { get; set; }
        public int? DeedsLoaderID { get; set; }
    }
}
