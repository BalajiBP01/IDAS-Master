using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class CommercialVATInfo
    {        
        public int CommercialID { get; set; }

        [Key]        
        public int CommercialVATID { get; set; }

        [StringLength(15)]
        public string RegistrationNo { get; set; }
        
        [StringLength(20)]
        public string IDNo { get; set; }

        [StringLength(200)]
        public string CommercialName { get; set; }

        [StringLength(200)]
        public string CommercialShortName { get; set; }
        
        public long VATNumber { get; set; }
        
        [StringLength(50)]
        public string VATLiableDate { get; set; }

        [StringLength(100)]
        public string StatusDescription { get; set; }

        [StringLength(50)]
        public string StatusChangeDate { get; set; }

        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        [StringLength(100)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Createdondate { get; set; }

        [StringLength(100)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Changedondate { get; set; }
    }
}
