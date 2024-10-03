using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class Buyer
    {
        [Key]
        public int BuyerID { get; set; }

        public int? PropertyDeedId { get; set; }

        [StringLength(70)]
        public string BuyerIDNO { get; set; }

        [StringLength(70)]
        public string BuyerName { get; set; }

        public byte? BuyerType { get; set; }

        [StringLength(12)]
        public string BuyerStatus { get; set; }

        [StringLength(50)]
        public string Share { get; set; }

        [Required]
        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(100)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedOndate { get; set; }

        [StringLength(100)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOndate { get; set; }

        [Column(TypeName = "numeric(18, 0)")]
        public decimal? BuyerSharePerc { get; set; }

        public int? DeedsLoaderID { get; set; }

        [StringLength(150)]
        public string BuyerNameSoundex { get; set; }
    }
}
