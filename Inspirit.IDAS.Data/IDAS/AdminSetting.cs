using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class ApplicationSetting
    {
        public Guid Id { get; set; }

        public string SettingName { get; set; } // Email,...

        public string SettingValue {get;set;}

        public string Remarks { get; set; }

        
    }
}
