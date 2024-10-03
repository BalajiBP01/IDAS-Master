using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class BatchProcessFileGeneration
    {
        [Key]
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public Guid CustomerUserId { get; set; }
        public string IdNumber { get; set; }
        public bool Executed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public Guid? CustomerID { get; set; }
    }
}
