using Inspirit.IDAS.Data.Production;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Inspirit.IDAS.ETLApplication
{
    public class Util
    {
        /// <summary>
        /// Returns a Boolean value indicating whether the given email is valid.
        /// </summary>
        public static bool IsEmailValid(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a boolean value indicating whether the given consumer Id is valid.
        /// </summary>
        public static bool IsConsumerIdValid(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            if ((id.Length < 8) || (id.Length > 13))
                return false;

            if (Regex.IsMatch(id, @"^[0-9]\d*$") == false)
                return false;

            return true;
        }

        public static bool MustbeNonNumeric(string value)
        {
            Regex nonNumericRegex = new Regex(@"\D");
            if (nonNumericRegex.IsMatch(value) == false)
            {
                return false;
            }
            return true;
        }

        public static bool AlphaNumericvalue(string value) {
            Regex r = new Regex("^[a-zA-Z0-9]*$");
            if (r.IsMatch(value) == false)
            {
                return false;
            }            
            return true;
        }


        public static bool AlphaNumericValueForAddress(string value)
        {
            if(!AlphaNumericvalue(value))
            {
                return false;
            }
            else if(MustbeNonNumeric(value))
            {
                return false;
            }
            else if(!IsNumberValid(value))
            {
                return false;
            }



            return true;
        }



        public static string RemoveNonNumericValues(string value)
        {
            StringBuilder val = new StringBuilder(value);
            int i = 0;
            while (i < val.Length)
            {
                switch (val[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        i++;
                        break;
                    default:
                        val.Remove(i, 1);
                        break;
                }
            }
            return val.ToString();
        }

        public static string RemoveBadCharacters(string value)
        {
            StringBuilder val = new StringBuilder(value);
            int i = 0;
            while (i < val.Length)
            {
                if (val[i] > 127)
                {
                    val.Remove(i, 1);
                }
                else
                {
                    i++;
                }
            }
            return val.ToString();
        }

       

        /// <summary>
        /// Returns a boolean value indicating whether the given name is having same characters.
        /// </summary>
        public static bool AreAllCharsSame(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != value[0])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a boolean value indicating whether the given Telephone number is in valid format.
        /// </summary>
        public static bool IsTelephoneValid(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (long.TryParse(value, out long telNum) == false)
                return false;

            if (value.Length != 10)
                return false;

            return true;
        }

        public static bool IsNumberValid(string value)
        {
            Regex r = new Regex(@"^(\+[0-9]{9})$");
            if (r.IsMatch(value) == false)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a boolean value indicating whether the given DateTime is not a future date.
        /// </summary>
        public static bool IsFutureDate(DateTime value)
        {
            if (value > DateTime.Now)
                return false;

            return true;
        }

        public static void Log(ProductionDbContext prodContext, LogTypeEnum logType, string logDescription, string table, bool UpdateDatabaseImmediately = false, string errortype = "")
        {
            Log log = new Data.Production.Log()
            {
                LogType = logType.ToString(),
                LogDescription = logDescription,
                TableName = table,
                //ErrorType = errortype,
                //LogTime = DateTime.Now
            };
            prodContext.Log.Add(log);

            //if (UpdateDatabaseImmediately)
            //    prodContext.SaveChanges();
        }

        public enum LogTypeEnum { Info, Error }

        public struct ErrorType
        {

            public static string Name = " The name is not in valid format ";
            //Telephone
            public static string TelephoneTenDigit = " Telephone number is not 10 digit ";
            public static string TelephoneNotNumeric = " Telephone number is not numeric ";
            public static string TelephoneDoesntExists = " The Telephone number do not exists ";
            public static string DialCodeNotNumeric = " International dialing code is not numeric ";
            public static string RecordStatusActive = " Record status is in active ";
            public static string RecordStatusInActive = " Record status is in inactive ";

            public static string IDNo = " The IDNO is not 13 digit ";
            public static string Passport = " The Passport is null or not in valid format ";

            public static string ID = " ID does not exists ";

            public static string NullValue = " Value can not be null ";
            public static string InvalidFormat = " Value is not in a valid format ";
            public static string Email = " Email ID is not valid ";
            
            public static string InValidDate = " The date is not in valid format ";
            public static string IDInvalidFormat = " The ID is not in Valid format ";
            public static string AddressInvalidFormat = " The Address is not in Valid format ";

            public static string NotNumeric = " The number is not in number format ";
            internal static string IdDoesntExists = "This ID do not exists in Consumer table";
            internal static string ConsumerIdDoesntExists = "This ID do not exists in ConsumerEmployment table";
            internal static string InValidActExpDate = " The ActExpiry date is not in valid Format ";
            internal static string InValidActEndDate = " The ActEnd date is not in valid Format ";
            internal static string InValidAccountOpenedDate = " The Account Opened date is not in valid Format ";
            internal static string InValidChangedOnDate = " The CahngedOn date is not in valid Format ";
            internal static string InValidCreatedOnDate = " The CreatedOn date is not in valid Format ";
            internal static string InValidFirstReportedDate = " The FirstReported date is not in valid Format ";
            internal static string InValidLastUpdatedDate = " The LastReported date is not in valid Format ";
            internal static string InValidVerifiedDate = " The Verified date is not in valid Format ";
            internal static string InValidAppliCreationDate = " The ApplicationCreation is not in valid Format ";
            internal static string InValidDebtReviewStatusDate = " The DebtReviewStatus date is not in valid Format ";
            internal static string InValidCaseFilingDate = " The CaseFiling date is not in valid Format ";
            internal static string InValidDisputeDate = " The Dispute date is not in valid Format ";
            internal static string InValidDisputeResolveDate = " The DisputeResolve date is not in valid Format ";
            internal static string InValidRecissionDate = " The Recission date is not in valid Format ";
            internal static string InValidAppointmentDate = " The Appointment date is not in valid Format ";
            internal static string InValidPurchaseDate = " The Purchase date is not in valid Format ";
            internal static string InValidRegisterDate = " The Registered date is not in valid Format ";
            internal static string InValidActStartDate = " The ActStart date is not in valid Format ";
            internal static string ConsumerNameID = " The ConsumerNameID is not in valid Format ";
            internal static string FirstName = " The First Name is not in valid Format ";
            internal static string SecondName = " The Second Name is not in valid Format ";
            internal static string ThirdName = " The Third Name is not in valid Format ";
            internal static string CommName = " The Commercial Name is not in valid Format ";
            internal static string CommShortName = " The Commercial short Name is not in valid Format ";
            internal static string AttorneyFaxNo = " The Attorney Fax Number is not valid ";
            internal static string AttorneyTelephoneNo = " The Attorney Telephone Number is not valid ";
            internal static string HomeTelephoneNo = " The Home Telephone Number is not valid ";
            internal static string WorkTelephoneNo = " The Work Telephone Number is not valid ";
            internal static string BuyerName = " The Buyer Name is not in valid Format ";
            internal static string SellerName = " The Seller Name is not in valid Format ";
            internal static string AuditorName = " The Auditor Name is not in valid Format ";
            internal static string Employer = " The Employer is not in valid Format ";
            internal static string Occupation = " The Occupation is not in valid Format ";
            internal static string ConsumerID = " The ConsumerID is not in Valid Format ";
        }
    }
}
