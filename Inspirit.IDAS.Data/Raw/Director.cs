using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class Director
    {
        [Key]        
        public int DirectorID { get; set; }

        [StringLength(100)]
        public string FirstInitial { get; set; }

        [StringLength(100)]
        public string SecondInitial { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string SecondName { get; set; }

        [StringLength(120)]
        public string Surname { get; set; }

        [StringLength(120)]
        public string SurnameParticular { get; set; }

        [StringLength(100)]
        public string SurnamePrevious { get; set; }
        
        [StringLength(13)]
        public string IDNo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }
        
        public bool? IsPossibleNameConflictYN { get; set; }
        
        public bool? IsPossibleDuplicateRecordYN { get; set; }

        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }
        
        [StringLength(10)]
        public string IDNo10 { get; set; }
        
        public int? MergedDirectorID { get; set; }

        [StringLength(20)]
        public string FileRef { get; set; }
    }
}
