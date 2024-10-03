using System;

namespace Inspirit.IDAS.Data.Production

{
    public class Employeement
    {
        //grid columns
        public string id { get; set; }
        public string occupation { get; set; }
        public string employer { get; set; }
        public DateTime? date { get; set; }
        public int recordstatusind { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
    


