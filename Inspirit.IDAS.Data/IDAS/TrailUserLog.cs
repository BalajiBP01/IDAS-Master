using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class TrailUserLog
    {
        public Guid ID { get; set; }
        public Guid TrailUserId { get; set; }
        public TrailUser trailUsers { get; set; }
        public DateTime Date { get; set; }
        public Guid ProductPackageId { get; set; }
        public int CreditPoints { get; set; }
        public string Idorpassportnumber { get; set; }
        public string Searchtype { get; set; }
        public string SearchCriteria { get; set; }
        public string Logtype { get; set; }
        public string InputType { get; set; }
    }
}
