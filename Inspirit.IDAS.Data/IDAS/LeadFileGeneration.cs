using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class LeadFileGeneration
    {
        public Guid ID { get; set; }
        public Guid LeadId { get; set; }
        public Guid CustomerUserId { get; set; }
        public string LeadOutput { get; set; }
        public bool Executed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid CustomerID { get; set; }
    }
}
