using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class Seller
    {
        [Key]        
        public int SellerID { get; set; }

        public int? PropertyDeedId { get; set; }
        
        [StringLength(73)]
        public string SellerIDNO { get; set; }

        [StringLength(70)]
        public string SellerName { get; set; }

        public byte? SellerType { get; set; }

        [StringLength(12)]
        public string SellerStatus { get; set; }

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
        
        public int? DeedsLoaderID { get; set; }
    }
}
