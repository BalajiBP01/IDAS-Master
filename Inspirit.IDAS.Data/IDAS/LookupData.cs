using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class LookupData
    {
        public Guid ID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public bool IsActive { get; set; }
    }
}
