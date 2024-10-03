using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class ConsumerName
    {
        public long ConsumerID { get; set; }

        [Key]
        public long ConsumerNameID { get; set; }

        public byte IDSequence { get; set; }

        [StringLength(10)]
        public string ID10 { get; set; }

        [StringLength(13)]
        public string IDNo { get; set; }

        [StringLength(16)]
        public string PassportNo { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string SecondName { get; set; }

        [StringLength(100)]
        public string ThirdName { get; set; }

        [StringLength(100)]
        public string Surname { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        public byte? GenderInd { get; set; }

        [StringLength(1)]
        public string FirstInitial { get; set; }

        [StringLength(1)]
        public string SecondInitial { get; set; }

        [StringLength(100)]
        public string FirstNameSoundex { get; set; }

        [StringLength(100)]
        public string SecondNameSoundex { get; set; }

        [StringLength(100)]
        public string ThirdNameSoundex { get; set; }

        [StringLength(100)]
        public string SurnameSoundex { get; set; }

        public bool? IsIDNoValidYN { get; set; }

        [StringLength(10)]
        public string TitleCode { get; set; }

        public byte RecordStatusInd { get; set; }

        [StringLength(500)]
        public string DeletedReason { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime LastUpdatedDate { get; set; }

        public bool IsVerifiedYN { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? VerifiedDate { get; set; }

        public int? SubscriberID { get; set; }

        public int? LoaderID { get; set; }

        [StringLength(50)]
        public string CreatedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOnDate { get; set; }

        [StringLength(50)]
        public string ChangedByUser { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ChangedOnDate { get; set; }

        public int? ChangedbyLoaderID { get; set; }
    }
}
