using System;
using System.Collections.Generic;

namespace Inspirit.IDAS.Data.Production

{
    public class DirectorShip
    {
        //grid columns
        public string id { get; set; }
        public string companyno { get; set; }
        public long consumerId { get; set; }
        public string companyname { get; set; }
        public string status { get; set; }
        public DateTime? date { get; set; }
        public int companyid { get; set; }
        public string idNumber { get; set; }
        public string fullName { get; set; }
        public string firstInitial { get; set; }
        public string secondname { get; set; }
        public string secondInitial { get; set; }
        public string gender { get; set; }
        public string surname { get; set; }
        public string surnamePrevious { get; set; }
        public DateTime? birthDate { get; set; }
        public DateTime? appointmentDate { get; set; }
        public DateTime? directorStatusDate { get; set; }
        public string isRSAResidentYN { get; set; }
        public string registerNo { get; set; }
        public string trusteeOf { get; set; }
        public decimal? memberSize { get; set; }
        public decimal? memberControlPerc { get; set; }
        public DateTime? directorSetDate { get; set; }
        public string profession { get; set; }
        public string estate { get; set; }
        public DateTime? lastUpdatedDate { get; set; }
        public string directorDesignationCode { get; set; }
        public string directorStatusCode { get; set; }
        public string directorTypeCode { get; set; }
        public DateTime? createdOnDate { get; set; }
        public string CommDirId { get; set; }
        // krishna start
        public string CommercialTelephoneNo { get; set; }
        // krishna end
        public List<DirectorAddressVm> directoraddresses = new List<DirectorAddressVm>();
        public List<DirectorTelephoneVM> directortelephones = new List<DirectorTelephoneVM>();

    }
    public class DirectorAddressVm
    {
        public int DirectorID { get; set; }
        public Director Director { get; set; }
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
        public string directorFullAddress { get; set; }
    }

    public class DirectorTelephoneVM
    {
        public string TelephoneTypeInd { get; set; }
        public string TelephoneCode { get; set; }
        public string TelephoneNo { get; set; }
        public string DeletedReason { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public string DirectorTelephone { get; set; }

    }

}



