using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    [Table("HomeAffairs_Confirmed_Married")]
    public partial class HomeAffairsConfirmedMarried
    {

        [Key]
        public Guid Id { get; set; }

        [Column("IDNo")]
        [StringLength(20)]
        public string Idno { get; set; }
        [StringLength(20)]
        public string BirthDate { get; set; }
        [StringLength(100)]
        public string Col003 { get; set; }
        [StringLength(200)]
        public string Surname { get; set; }
        [StringLength(200)]
        public string FirstName { get; set; }
        [StringLength(200)]
        public string SecondName { get; set; }
        [StringLength(100)]
        public string Col007 { get; set; }
        [StringLength(100)]
        public string Col008 { get; set; }
        [StringLength(100)]
        public string Col009 { get; set; }
        [Column("SpouseIDNo")]
        [StringLength(20)]
        public string SpouseIdno { get; set; }
        [StringLength(200)]
        public string SpouseFirstname { get; set; }
        [Column("UID")]
        public int Uid { get; set; }
        [Required]
        [StringLength(10)]
        public string RecordStatusInd { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }
        [StringLength(50)]
        public string CreatedByUser { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
        [StringLength(50)]
        public string UpdatedByUser { get; set; }

    }
}
