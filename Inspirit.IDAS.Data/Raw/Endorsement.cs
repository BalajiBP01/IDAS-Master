using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class Endorsement
    {
        [Key]
        [Column("EndorsementID")]
        public int EndorsementId { get; set; }
        [Column("PropertyDeedID")]
        public int? PropertyDeedId { get; set; }
        [StringLength(20)]
        public string EndorsementNumber { get; set; }
        [StringLength(70)]
        public string EndorsementHolder { get; set; }
        public int? EndorsementAmount { get; set; }
        [StringLength(1)]
        public string RecordStatusInd { get; set; }
        [StringLength(100)]
        public string CreateByUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedOnDate { get; set; }
        [StringLength(100)]
        public string ChangedByUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }
        [Column("DeedsLoaderID")]
        public int? DeedsLoaderId { get; set; }
    }
}
