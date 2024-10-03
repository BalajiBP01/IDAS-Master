using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class RawDataSource
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string Source { get; set; }

        public string ProcessDetial { get; set; } // JSON data of ProcessDetials
    }

     public class ProcessDetial
    {
        public Guid SourceId { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        
        public int TotalRecords { get; set; }

            
    }

}
