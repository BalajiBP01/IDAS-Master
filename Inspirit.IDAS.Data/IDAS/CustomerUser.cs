using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class CustomerUser
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [StringLength(10)]
        public string Title { get; set; } //choices - Mr, Miss, Mrs, Ms
        [Required]
        public string IDNumber { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public string Status { get; set; } //Created,Active,Inactive

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Code { get; set; }

        public Subscription Subscription { get; set; }

        public Guid? SubscriptionId { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? ActivatedBy { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? BatchwithoutSub { get; set; }
        public bool? IsUserLoggedIn { get; set; }
        public bool? IsRestricted { get; set; }
        public bool? LeadswithoutSub { get; set; }
        public bool? MAchAddressCHK { get; set; }
        public string MacAddresses { get; set; }

        public DateTime? LastPasswordResetDate { get; set; }

    }
}
