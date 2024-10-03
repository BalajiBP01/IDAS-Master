using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class DataServicesAgreement
    {
        public Guid Id { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int Version { get; set; }
        public string FilePath { get; set; }
        public bool IsPublished { get; set; } // modification can be made until published
        public string Description { get; set; }
    }
}
