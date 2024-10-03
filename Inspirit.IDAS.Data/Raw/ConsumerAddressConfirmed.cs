using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class ConsumerAddressConfirmed
    {
        [Key]        
        public long ID { get; set; }
        
        [StringLength(13)]
        public string IDNO { get; set; }
        
        [StringLength(10)]
        public string IDNO10 { get; set; }

        [StringLength(10)]
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

        [StringLength(200)]
        public string Status { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        [StringLength(500)]
        public string Extra1 { get; set; }

        [StringLength(500)]
        public string Extra2 { get; set; }

        [StringLength(500)]
        public string Extra3 { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? VerifiedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }

        public byte? RecordStatusInd { get; set; }

        [StringLength(200)]
        public string Supplier { get; set; }
        
        public int? CCACkeckSum { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }
        
        public int? LoaderID { get; set; }
    }
}
