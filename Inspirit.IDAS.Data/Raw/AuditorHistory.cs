using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class AuditorHistory
    {
        [Key]
        public Guid Id { get; set; }

        public long? CommercialID { get; set; }

        public long? CommercialAuditorID { get; set; }

        [StringLength(150)]
        public string AuditorName { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActStartDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActEndDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ActExpiryDate { get; set; }

        [StringLength(10)]
        public string AuditorStatusCode { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LoadDate { get; set; }

    }
}
