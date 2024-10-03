using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class Keyword
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
