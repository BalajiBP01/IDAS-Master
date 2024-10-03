using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class ApplicationMessage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [StringLength(250)]
        public string Message { get; set; }

        public bool Showmessage { get; set; }


    }
}
