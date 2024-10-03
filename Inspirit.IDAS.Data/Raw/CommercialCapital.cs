using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class CommercialCapital
    {        
        public int CommercialID { get; set; }

        [Key]        
        public int CommercialCapitalID { get; set; }
        
        [StringLength(15)]
        public string RegistrationNO { get; set; }

        public int? Type { get; set; }
        [Column(TypeName = "numeric(28, 9)")]
        public decimal? NoOfShares { get; set; }

        public int? ParriValue { get; set; }

        [Column(TypeName = "numeric(19, 1)")]
        public decimal? AmountPerShare { get; set; }

        public int? Premium { get; set; }

        [StringLength(20)]
        public string FileRef { get; set; }

        [StringLength(2)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOndate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOndate { get; set; }
    }
}
