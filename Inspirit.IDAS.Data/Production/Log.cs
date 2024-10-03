using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Production
{
    public class Log
    {
        [Key]
        public long LogId { get; set; }

        public DateTime LogTime { get; set; }

      
        public string LogType { get; set; }

        public string TableName { get; set; }

        public string LogDescription { get; set; }
    }
}
