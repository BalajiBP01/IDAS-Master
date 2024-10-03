using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class Customer
    {
        public Guid Id { get; set; }
        //Company Information
        [Required]
        [StringLength(100)]
        public string TradingName { get; set; }
        [Required]
        [StringLength(100)]
        public string RegistrationName { get; set; }
        [Required]
        public string RegistrationNumber { get; set; }
        public string VATNumber { get; set; }
        [StringLength(100)]
        public string BranchLocation { get; set; }
        [StringLength(250)]
        [Required]
        public string PhysicalAddress { get; set; }
        [Required]
        public string TypeOfBusiness { get; set; }// choices - ref. to  doc
        [Required]
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        [StringLength(100)]
        public string BillingEmail { get; set; }
        public string Status { get; set; } //Pending,Active,Inactive      
        public string BillingType { get; set; } //Credit,Invoice
        public string Code { get; set; }
        public bool Turnover { get; set; }
        public string CustOwnIDNumber { get; set; }
        public string PostalAddress { get; set; }
        public string WebAddress { get; set; }
        public string AccountDeptContactPerson { get; set; }
        public string AccountDeptTelephoneNumber { get; set; }
        public string AccountDeptFaxNumber { get; set; }
        public string AuthIDNumber { get; set; }
        public string AuthPosition { get; set; }
        [StringLength(100)]
        public string AccountDeptEmail { get; set; }
        public string AuthFirstName { get; set; }
        public string AuthSurName { get; set; }
        [Required]
        public string AuthCellNumber { get; set; }
        public string AuthEmail { get; set; }
        //Extra fields added
        public string BusinessDescription { get; set; }
        public string CreditBureauInformation { get; set; }
        public string Purpose { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? ActivatedBy { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public bool? IsRestricted { get; set; }

        public string Client_Logo { get; set; }
        public List<CustomerUser> CustomerUsers { get; set; }
        public List<CustomerProduct> CustomerProducts { get; set; }

        public string TabSelected { get; set; }
        // krishna start
        public string EnquiryReason { get; set; }

        public bool IsXDS { get; set; }

        public string SubscriberId { get; set; }

        // krishna end
    }
}
