using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inspirit.IDAS.Data.Production
{
    public class Director
    {
        [Key]
        public int DirectorID { get; set; }
        public string FirstInitial { get; set; }
        public string SecondInitial { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Surname { get; set; }
        public string SurnameParticular { get; set; }
        public string SurnamePrevious { get; set; }
        public string IDNo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? BirthDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? AppointmentDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DirectorStatusDate { get; set; }

        public bool? IsRSAResidentYN { get; set; }
        public string RegisterNo { get; set; }
        public string TrusteeOf { get; set; }
        public decimal? MemberSize { get; set; }
        public decimal? MemberControlPerc { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DirectorSetDate { get; set; }

        public string Profession { get; set; }
        public string Estate { get; set; }
        public string DirectorDesignationCode { get; set; }
        public string DirectorStatusCode { get; set; }
        public string DirectorTypeCode { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LastUpdatedDate { get; set; }
           
        [StringLength(1)]
        public string RecordStatusInd { get; set; }
    }

    public class DirectorAddress
    {
        public int DirectorID { get; set; }
        public Director Director { get; set; }

        [Key]
        public int DirectorAddressID { get; set; }
        public string addressTypeInd { get; set; }
        public string province { get; set; }
        public string originalAddress1 { get; set; }
        public string originalAddress2 { get; set; }
        public string originalAddress3 { get; set; }
        public string originalAddress4 { get; set; }
        public string originalPostalCode { get; set; }
        public string occupantTypeInd { get; set; }
        public DateTime lastUpdatedDate { get; set; }
        public DateTime createdOnDate { get; set; }
    }

    public class DirectorTelephone
    {
        public int DirectorID { get; set; }
        public Director Director { get; set; }

        [Key]
        public int DirectorTelephoneID { get; set; }
        public string TelephoneTypeInd { get; set; }
        public string TelephoneCode { get; set; }
        public string TelephoneNo { get; set; }
        public string RecordStatusInd { get; set; }
        public string DeletedReason { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime? CreatedOnDate { get; set; }
    }
}
