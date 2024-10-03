using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

using Microsoft.EntityFrameworkCore.Storage;
using System.IO;

namespace Inspirit.IDAS.Data.Production
{
    public class DAL
    {
        //Get ProductionConnection connection string
        public string GetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json",optional:true,reloadOnChange:true);
            return builder.Build().GetSection("ConnectionStrings").GetSection("ProductionConnection").Value;
        }

        //Get Registration Number
        public List<Commercialdata> getRegistrationNo(string registrationNo)
        {
            string regNoAddFristSlash = "";
            string regNoAddSecondSlash = "";
            string regNoFinal = "";
            //int registrationNoLenght = registrationNo.Length;

            regNoAddFristSlash = registrationNo.Insert(4, "/");
            regNoAddSecondSlash = regNoAddFristSlash;
            regNoFinal = regNoAddSecondSlash.Insert(11, "/");

            List<Commercialdata> items = new List<Commercialdata>();
            SqlConnection con = new SqlConnection(GetConnectionString());
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.GetRegistrationNo", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@registrationNo", regNoFinal);

            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    Commercialdata commercialdata = new Commercialdata();
                    commercialdata.CommercialID = reader.GetInt32(0);
                    commercialdata.CompanyName = reader.GetValue(1).ToString();
                    commercialdata.CompanyRegNumber = reader.GetValue(2).ToString();
                    commercialdata.CommercialStatusCode = reader.GetValue(3).ToString();
                    commercialdata.BusinessStartDate = reader.GetDateTime(4);
                    commercialdata.LastUpdatedDate = reader.GetDateTime(5);
                    items.Add(commercialdata);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return items;
            con.Close();

        }

        //Get IsRestricted Customer
        public int getIsRestrictedCustomer(Guid Id)
        {
            SqlConnection con = new SqlConnection(GetConnectionString());
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.getIsRestrictedCustomer", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Id);
            int Result = Convert.ToInt32(cmd.ExecuteScalar()); 
            cmd.Dispose();
            return Result;
        }

        //Get IsRestricted CustomerUse
        public int getIsRestrictedCustomerUser(Guid Id)
        {
            SqlConnection con = new SqlConnection(GetConnectionString());
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.getIsRestrictedCustomerUser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Id);
            int Result = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.Dispose();
            return Result;
        }

        //Get LastPasswordResetDate CustomerUse
        public string getIsLastPasswordResetDateCustomerUser(Guid Id)
        {
            SqlConnection con = new SqlConnection(GetConnectionString());
            con.Open();
            SqlCommand cmd = new SqlCommand("dbo.getLastPasswordResetDateCustomerUser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", Id);
            string Result = Convert.ToString(cmd.ExecuteScalar());
            cmd.Dispose();
            return Result;
        }        
    }
}
