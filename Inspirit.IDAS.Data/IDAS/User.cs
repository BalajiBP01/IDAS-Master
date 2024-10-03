using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
      
        public string Emailid { get; set; }
     
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public List<UserPermission> UserPermissionslist { get; set; } = new List<UserPermission>();
     
    }
}
