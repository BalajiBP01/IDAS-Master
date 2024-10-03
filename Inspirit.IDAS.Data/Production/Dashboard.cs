using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.Production
{
    public class Dashboard
    {
        [Key]
        public Guid Id { get; set; }
        public long InsertCount { get; set; }
        public long UpdateCount { set; get; }
        public long YearToDateUpdate { get; set; }
        public string TableName { get; set; }
        public long TotalCount { get; set; }
    }
}
