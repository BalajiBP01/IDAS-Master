using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class HomeAffairs
    {
        [Key]        
        public int HomeAffairsID { get; set; }
        
        [StringLength(13)]
        public string IDNo { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string SecondName { get; set; }

        [StringLength(100)]
        public string ThirdName { get; set; }

        [StringLength(100)]
        public string Surname { get; set; }

        [StringLength(300)]
        public string NameCombo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }
        
        public bool HomeAffairsRunYN { get; set; }

        [StringLength(100)]
        public string HomeAffairsMessage { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DeceasedDate { get; set; }

        [Required]
        [StringLength(1)]
        public string RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }
        
        public int? HARecordChecksum { get; set; }
        
        public int? SubscriberID { get; set; }
        
        public int? LoaderID { get; set; }
        
        public int? ConsumerID { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }
        
        public bool? IsPossibleDuplicateRecordYN { get; set; }
        
        public bool? IsPossibleNameConflictYN { get; set; }

        [StringLength(50)]
        public string DeceasedStatus { get; set; }

        [StringLength(150)]
        public string MaidenName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? IDIssuedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? MarriageDate { get; set; }

        [StringLength(250)]
        public string PlaceOfMarriage { get; set; }
        
        [StringLength(20)]
        public string SpouseIdnoOrDOB { get; set; }

        [StringLength(100)]
        public string SpouseSurName { get; set; }

        [StringLength(100)]
        public string SpouseForeNames { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DivorceDate { get; set; }

        [StringLength(250)]
        public string DivorceIssuedCourt { get; set; }

        [StringLength(250)]
        public string PlaceOfDeath { get; set; }

        [StringLength(1)]
        public string CauseOfDeath { get; set; }
    }
}
