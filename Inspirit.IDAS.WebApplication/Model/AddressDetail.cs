using System;

namespace Inspirit.IDAS.Data.Production

{
    public class AddressDetail
    {
        //address fields
        public string id { get; set; }
        public string addressTypeInd { get; set; }
        public string province { get; set; }
        public string originalAddress1 { get; set; }
        public string originalAddress2 { get; set; }
        public string originalAddress3 { get; set; }
        public string originalAddress4 { get; set; }
        public string fullAddress { get; set; }
        public string originalPostalCode { get; set; }
        public string occupantTypeInd { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
        public DateTime? createdOnDate { get; set; }
        public int recordstatusind { get; set; }
        public string town { get; set; }
        public string region { get; set; }
    }
}
    


