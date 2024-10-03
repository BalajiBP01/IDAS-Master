using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspirit.IDAS.WebApplication.Model
{
    public class Consumerjudgement
    {
        public string id { get; set; }
        public string idno { get; set; }
        public string casenumber { get; set; }
        public DateTime? casefilingdate { get; set; }
        public string casereason { get; set; }
        public string propertytype { get; set; }
        public string propertyname { get; set; }
        public string casetype { get; set; }
        public decimal? disputeamt { get; set; }
        public string courtname { get; set; }
        public string courtcity { get; set; }

        public string courttype { get; set; }
        public string plaintiffname { get; set; }
        public string plaintiffaddress1 { get; set; }
        public string plaintiffaddress2 { get; set; }
        public string plaintiffaddress3 { get; set; }
        public string plaintiffaddress4 { get; set; }
        public string plaintifpostalcode { get; set; }
        public string attorneyname { get; set; }
        public string attorneytelephonecode { get; set; }
        public string attorneytelephoneno { get; set; }
        public string attorneyfaxcode { get; set; }
        public string attorneyfaxno { get; set; }
        public string attorneyaddress1 { get; set; }
        public string attorneyaddress2 { get; set; }
        public string attorneyaddress3 { get; set; }
        public string attorneyaddress4 { get; set; }
        public string attorneypostalcode { get; set; }
        public DateTime lastupdateddate { get; set; }
        public DateTime? createdodate { get; set; }
        public string judgementtypecode { get; set; }
        public DateTime? disputedate { get; set; }
        public DateTime? disputeresolveddate { get; set; }
        public bool? rescinded { get; set; }
        public DateTime? rescissiondate { get; set; }
        public string rescissionreason { get; set; }
        public string rescindedamount { get; set; }
    
}
}
