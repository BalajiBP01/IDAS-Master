using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication.Model
{
    public class NewsBlog
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime date { get; set; }
        public List<string> keywords { get; set; } = new List<string>();
    }
}
