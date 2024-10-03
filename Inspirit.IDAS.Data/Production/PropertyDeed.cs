using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class PropertyDeed
    {
        [Key]
        public int PropertyDeedID { get; set; }
        public int DeedsOfficeId { get; set; }
        public string TitleDeedNo { get; set; }
        public string TitleDeedNoOLD { get; set; }
        public int? TitleDeedFee { get; set; }
        
        public DateTime? DatePurchase { get; set; }
       
        public DateTime? DateRegister { get; set; }
        public decimal? PurchaseAmount { get; set; }
        public string StreetAddress { get; set; }
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public decimal? Y { get; set; }
        public decimal? X { get; set; }
        public string SuburbCode { get; set; }
        public string SuburbDeeds { get; set; }
        public string Town { get; set; }
        public string Authority { get; set; }
        public string MunicipalityName { get; set; }
        public byte? ProvinceId { get; set; }
        public bool? IsCurrentOwner { get; set; }
    
        public string Extent { get; set; }
        public string AttorneyFirmNumber { get; set; }
        public string AttorneyFileNumber { get; set; }
        public int? TransferSeqNo { get; set; }

        public DateTime? DateCaptured { get; set; }
        public string BondNumber { get; set; }
        public string BondHolder { get; set; }
        public long? BondAmount { get; set; }
        public string PropertyType { get; set; }
        public string PropertyName { get; set; }
        public string SchemeId { get; set; }
        public short? SuburbId { get; set; }
        public string Erf { get; set; }
        public int? Portion { get; set; }
        public int? Unit { get; set; }
     
        public DateTime? CreatedOndate { get; set; }
   
        public string ErfSize { get; set; }
        public string StandNo { get; set; }
        public string PortionNo { get; set; }
     
        public int? TownShipNo { get; set; }
        public string PrevExtent { get; set; }
        public int? IsCurrOwnerUpdated { get; set; }
        public int? ChangedByLoaderID { get; set; }
        public char RecordStatusInd { get; set; }
    }
    public class PropertyDeedBuyer
    {
        [Key]
        public int BuyerID { get; set; }

        public int? PropertyDeedId { get; set; }
        public virtual PropertyDeed PropertyDeed { get; set; }

        public string BuyerIDNO { get; set; }

        public string BuyerName { get; set; }

        public byte? BuyerType { get; set; }

        public string BuyerStatus { get; set; }

        public string Share { get; set; }

        public DateTime CreatedOndate { get; set; }
        public char RecordStatusInd { get; set; }
    }

    public class PropertyDeedSeller
    {
        [Key]
        public int SellerID { get; set; }

        public int? PropertyDeedId { get; set; }
        public virtual PropertyDeed PropertyDeed { get; set; }

        public string SellerIDNO { get; set; }

        public string SellerName { get; set; }

        public byte? SellerType { get; set; }

        public string SellerStatus { get; set; }

        public DateTime CreatedOndate { get; set; }
        public char RecordStatusInd { get; set; }
    }
}