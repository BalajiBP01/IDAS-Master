using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class CommercialAddress
    {

        public int CommercialID { get; set; }
        public virtual Commercial Commercial { get; set;}

        [Key]        
        public int CommercialAddressID { get; set; }

        [Required]
        [StringLength(1)]
        public string AddressTypeInd { get; set; }

        [StringLength(100)]
        public string OriginalAddress1 { get; set; }

        [StringLength(100)]
        public string OriginalAddress2 { get; set; }

        [StringLength(100)]
        public string OriginalAddress3 { get; set; }

        [StringLength(100)]
        public string OriginalAddress4 { get; set; }

        [StringLength(10)]
        public string OriginalPostalCode { get; set; }

        [StringLength(100)]
        public string CleanAddress1 { get; set; }

        [StringLength(100)]
        public string CleanAddress2 { get; set; }

        [StringLength(100)]
        public string CleanAddress3 { get; set; }

        [StringLength(100)]
        public string CleanAddress4 { get; set; }

        [StringLength(10)]
        public string CleanPostalCode { get; set; }

        [StringLength(200)]
        public string CleanAddressDupCode { get; set; }

        [StringLength(1)]
        public string CleanAddressValidTypeInd { get; set; }

        [Required]
        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        
        public int? LoaderID { get; set; }
        
        public int? SubscriberID { get; set; }

        [StringLength(5)]
        public string CountryCode { get; set; }

        [StringLength(10)]
        public string CountryRegionCode { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }

        public bool? Confirmed { get; set; }

        [StringLength(20)]
        public string FileRef { get; set; }
    }
}
