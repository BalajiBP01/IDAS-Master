using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class Telephone
    {
        [Key]
        public long TelephoneID { get; set; }

        [Required]
        [StringLength(5)]
        public string InternationalDialingCode { get; set; }

        [StringLength(5)]
        public string TelephoneCode { get; set; }

        public long TelephoneNo { get; set; }

        public byte LeadingZeroCount { get; set; }

        public byte RecordStatusInd { get; set; }

        [StringLength(50)]
        public string CreatedbyUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedonDate { get; set; }

        [StringLength(50)]
        public string ChangedbyUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedonDate { get; set; }
    }
}
