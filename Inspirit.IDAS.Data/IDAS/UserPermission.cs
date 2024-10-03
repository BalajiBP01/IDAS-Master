using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Inspirit.IDAS.Data
{
    public class UserPermission
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FormName { get; set; }
        public bool ViewAction { get; set; }
        public bool AddAction { get; set; }
        public bool EditAction { get; set; }
        public bool privileged { get; set; }
    }
}
