using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class Subscription
    {
        public Guid Id { get; set; }

      
        public Customer Customer { get; set; }

        public Guid CustomerId { get; set; }


        public int Number { get; set; }

        public DateTime Date { get; set; }
        public string SubDisplayNumber { get; set; }
        public bool IsAutoBilled { get; set; }
        public List<SubscriptionItem> SubscriptionItems { get; set; } = new List<SubscriptionItem>();

    }
}
