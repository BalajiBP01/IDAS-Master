using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    [Table("HomeAffairs_Singles")]
    public partial class HomeAffairsSingles
    {
        [Key]
        public Guid Id { get; set; }

        [Column("IDNo")]
        [StringLength(20)]
        public string Idno { get; set; }
        [Column("DOB")]
        [StringLength(20)]
        public string Dob { get; set; }
        [StringLength(50)]
        public string Data1 { get; set; }
        [StringLength(100)]
        public string Surname { get; set; }
        [StringLength(100)]
        public string Firstname { get; set; }
        [StringLength(50)]
        public string Data2 { get; set; }
        [StringLength(50)]
        public string Data3 { get; set; }
        [StringLength(20)]
        public string DeceasedDate { get; set; }
        [StringLength(100)]
        public string PlaceDeceased { get; set; }
        public int? CanBeUsed { get; set; }
        [StringLength(500)]
        public string DescReason { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }
        [StringLength(50)]
        public string CreatedByUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        [StringLength(50)]
        public string UpdatedByUser { get; set; }
        [StringLength(1)]
        public string RecordStatusInd { get; set; }
    }
}
