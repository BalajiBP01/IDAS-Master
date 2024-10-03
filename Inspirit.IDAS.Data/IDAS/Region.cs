using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class Region
    {
        public Guid ID { get; set; }

        public string Type { get; set; } //Province,Area,Suburb

        public string Name { get; set; }

        public Guid? UnderID { get; set; }

        public Region RegionID { get; set; }
    }
}
