using System;
using System.Collections.Generic;
using System.Text;

namespace Inspirit.IDAS.Data.IDAS
{
    public class SubscriptionLicense
    {
        public Guid ID { get; set; }

        public SubscriptionItem SubscriptionItem { get; set; }

        public Guid? SubscriptionItemId { get; set; }

        public CustomerUser CustomerUser { get; set; }

        public Guid CustomerUserId { get; set; }


        public DateTime AssignedDate { get; set; }

        public bool IsActive { get; set; }
    }
}
