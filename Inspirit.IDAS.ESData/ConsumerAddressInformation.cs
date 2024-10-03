using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.ESData
{
    [ElasticsearchType(Name = "consumeraddressinformation", IdProperty = "consumeraddressid")]
    public class ConsumerAddressInformation
    {
        [Text]
        public string consumeraddressid { get; set; }

        [Text(Name = "consumerid", Analyzer = "standard", Fielddata = true)]
        public string ConsumerId { get; set; }

        [Text(Name = "address", Analyzer = "english", Fielddata = true)]
        public string Address { get; set; }
    }
}
