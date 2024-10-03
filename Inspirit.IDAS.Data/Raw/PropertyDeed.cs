using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class PropertyDeed
    {
        [Key]        
        public int PropertyDeedID { get; set; }

        public byte DeedsOfficeId { get; set; }

        [StringLength(50)]
        public string TitleDeedNo { get; set; }
        
        [StringLength(50)]
        public string TitleDeedNoOLD { get; set; }

        public int? TitleDeedFee { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DatePurchase { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateRegister { get; set; }

        [Column(TypeName = "numeric(18, 0)")]
        public decimal? PurchaseAmount { get; set; }

        [StringLength(201)]
        public string StreetAddress { get; set; }

        [StringLength(100)]
        public string StreetNumber { get; set; }

        [StringLength(100)]
        public string StreetName { get; set; }

        [StringLength(25)]
        public string StreetType { get; set; }

        [Column(TypeName = "decimal(12, 10)")]
        public decimal? Y { get; set; }

        [Column(TypeName = "decimal(12, 10)")]
        public decimal? X { get; set; }

        [StringLength(13)]
        public string SuburbCode { get; set; }

        [StringLength(100)]
        public string SuburbDeeds { get; set; }

        [StringLength(100)]
        public string Town { get; set; }

        [StringLength(100)]
        public string Authority { get; set; }

        [StringLength(100)]
        public string MunicipalityName { get; set; }

        public byte? ProvinceId { get; set; }

        public bool? IsCurrentOwner { get; set; }

        [StringLength(20)]
        public string PurchaseReference { get; set; }

        [StringLength(30)]
        public string AddDescription { get; set; }

        [StringLength(1)]
        public string ExemptIndicator { get; set; }

        [StringLength(200)]
        public string Extent { get; set; }

        [StringLength(20)]
        public string AttorneyFirmNumber { get; set; }

        [StringLength(50)]
        public string AttorneyFileNumber { get; set; }

        public int? TransferSeqNo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateCaptured { get; set; }

        [StringLength(50)]
        public string BondNumber { get; set; }

        [StringLength(150)]
        public string BondHolder { get; set; }

        public long? BondAmount { get; set; }

        [StringLength(5)]
        public string PropertyType { get; set; }

        [StringLength(70)]
        public string PropertyName { get; set; }

        [StringLength(200)]
        public string SchemeId { get; set; }

        public short? SuburbId { get; set; }

        [StringLength(200)]
        public string Erf { get; set; }

        public int? Portion { get; set; }

        public int? Unit { get; set; }

        public short? PropertyYear { get; set; }

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
        public DateTime? ChangedOnDate { get; set; }

        [StringLength(200)]
        public string ErfSize { get; set; }

        [StringLength(50)]
        public string StandNo { get; set; }

        [StringLength(50)]
        public string PortionNo { get; set; }

        public int? DeedsLoaderID { get; set; }

        public int? TownShipNo { get; set; }

        [StringLength(20)]
        public string PrevExtent { get; set; }

        public int? IsCurrOwnerUpdated { get; set; }

        public int? ChangedByLoaderID { get; set; }
    }
}
