using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class EmailTemplate
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public string MailContent { get; set; }

        public string Type { get; set; }
    }
}
