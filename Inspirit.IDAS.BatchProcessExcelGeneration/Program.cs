using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Inspirit.IDAS.ESData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;


namespace Inspirit.IDAS.BatchProcessExcelGeneration
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                string strProcess = ConfigurationManager.AppSettings["Process"].ToString();
                if (strProcess.ToLower() == "all" || strProcess.ToLower() == "batch")
                {
                    Console.WriteLine("Starting Batch Process Excel Generation");
                    StartProcess();
                    Console.WriteLine("Batch Process Excel Generation Completed.");
                }
                if (strProcess.ToLower() == "all" || strProcess.ToLower() == "lead")
                {
                    Console.WriteLine("Starting Leads Process Excel Generation");
                    StartLeads();
                    Console.WriteLine("Leads Process Excel Generation Completed.");
                }
            }
            catch (Exception ex) { Console.WriteLine("Error Occured."); Console.WriteLine(ex.Message); Console.ReadKey(); }
        }
        public static void StartProcess()
        {
            //Remove Variables
            string Temp = string.Empty;


            SqlConnection IDASDBConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["IDASDBConnection"].ConnectionString);
            SqlCommand objBatchProcessCommand = new SqlCommand();
            SqlDataAdapter objBatchAdapter = new SqlDataAdapter();
            DataTable dtBatchProcess = new DataTable();
            SqlCommand objBatchExcelCommand = new SqlCommand();


            SqlConnection ProdBConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ProdDBConnection"].ConnectionString);
            SqlCommand objProdCommand = new SqlCommand();
            SqlDataAdapter objProdAdapter = new SqlDataAdapter();
            DataTable dtBatchOutput = new DataTable();

            objBatchProcessCommand.CommandText = "select BatchProcessFileGeneration.ID,Customers.Code, BatchTraces.BatchNumber,BatchTraces.FileName,Cast(IdNumber as varchar(max)) IdNumber from BatchProcessFileGeneration inner join BatchTraces on BatchProcessFileGeneration.BatchId = BatchTraces.ID inner join Customers on BatchTraces.CustomerID = Customers.Id  where Executed = 0;";
            objBatchProcessCommand.CommandType = CommandType.Text;
            objBatchProcessCommand.Connection = IDASDBConnection;
            objBatchProcessCommand.CommandTimeout = 0;

            objBatchAdapter.SelectCommand = objBatchProcessCommand;

            try
            {
                if (IDASDBConnection.State != ConnectionState.Open)
                    IDASDBConnection.Open();

                if (ProdBConnection.State != ConnectionState.Open)
                    ProdBConnection.Open();

                objBatchAdapter.Fill(dtBatchProcess);
                foreach (DataRow dr in dtBatchProcess.Rows)
                {
                    objProdCommand = new SqlCommand();
                    objProdAdapter = new SqlDataAdapter();
                    dtBatchOutput = new DataTable();
                    Temp = dr["Code"].ToString() + "_" + dr["BatchNumber"].ToString();
                    Console.WriteLine("Processing for : " + dr["Code"].ToString() + "_" + dr["BatchNumber"].ToString());

                    if (ProdBConnection.State != ConnectionState.Open)
                        ProdBConnection.Open();
                    objProdCommand.Connection = ProdBConnection;
                    objProdCommand.CommandText = "QSPBATCHTRACINGREPORT";
                    objProdCommand.CommandType = CommandType.StoredProcedure;

                    objProdCommand.Parameters.Add(new SqlParameter("@IDNUMBER", dr["IdNumber"].ToString().Trim().Replace(' ', ',')));
                    objProdCommand.Parameters.Add(new SqlParameter("@dbname", IDASDBConnection.Database));
                    objProdCommand.CommandTimeout = 0;
                    objProdAdapter.SelectCommand = objProdCommand;
                    if (ProdBConnection.State != ConnectionState.Open)
                        ProdBConnection.Open();

                    objProdAdapter.Fill(dtBatchOutput);

                    if (File.Exists(ConfigurationManager.AppSettings["Path"] + "\\" + dr["Code"].ToString() + "_" + dr["BatchNumber"].ToString() + "_" + dr["FileName"].ToString().Replace(".txt", ".xlsx")))
                        File.Delete(ConfigurationManager.AppSettings["Path"] + "\\" + dr["Code"].ToString() + "_" + dr["BatchNumber"].ToString() + "_" + dr["FileName"].ToString().Replace(".txt", ".xlsx"));

                    ExportDataFromDataTable(dtBatchOutput, ConfigurationManager.AppSettings["Path"] + "\\" + dr["Code"].ToString() + "_" + dr["BatchNumber"].ToString() + "_" + dr["FileName"].ToString().Replace(".txt", ".xlsx"));

                    objBatchExcelCommand = new SqlCommand();
                    objBatchExcelCommand.Connection = IDASDBConnection;
                    objBatchExcelCommand.CommandText = "update BatchProcessFileGeneration set Executed = 1, ExecutedDate = GETDATE() where ID = '" + dr["ID"].ToString() + "'";
                    objBatchExcelCommand.CommandType = CommandType.Text;
                    objBatchExcelCommand.CommandTimeout = 0;
                    if (IDASDBConnection.State != ConnectionState.Open)
                        IDASDBConnection.Open();
                    objBatchExcelCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occured while processing : " + Temp);
                Console.WriteLine("Error : " + ex.Message);
                Console.ReadLine();
            }
            finally
            {
                if (IDASDBConnection.State != ConnectionState.Closed)
                    IDASDBConnection.Close();

                if (ProdBConnection.State != ConnectionState.Closed)
                    ProdBConnection.Close();
            }


        }

        public static void ExportDataFromDataTable(DataTable dataTable, string filepath)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
                sheets.Append(sheet);
                Row headerRow = new Row();
                List<String> columns = new List<string>();
                DataTable table = dataTable;
                foreach (System.Data.DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                    Cell cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column.ColumnName);
                    headerRow.AppendChild(cell);
                }
                sheetData.AppendChild(headerRow);
                foreach (DataRow dsrow in table.Rows)
                {
                    Row newRow = new Row();
                    foreach (String col in columns)
                    {
                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(dsrow[col].ToString());
                        newRow.AppendChild(cell);
                    }
                    sheetData.AppendChild(newRow);
                }
                workbookPart.Workbook.Save();
            }
        }

        public static void StartLeads()
        {
            SqlConnection IDASDBConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["IDASDBConnection"].ConnectionString);
            SqlCommand objLeadProcessCommand = new SqlCommand();
            SqlDataAdapter objLeadAdapter = new SqlDataAdapter();
            DataTable dtLeadProcess = new DataTable();
            ESService _eSService = new ESService(ConfigurationManager.AppSettings["ElasticDBIP"]);

            objLeadProcessCommand.CommandText = "select LeadFileGeneration.ID,LeadId, LeadsGenaration.LeadsNumber, LeadsGenaration.OutPutFileName, LeadFileGeneration.LeadOutput,LeadsGenaration.InputDetail from LeadFileGeneration inner join LeadsGenaration on LeadFileGeneration.LeadId = LeadsGenaration.ID where Executed = 0";
            objLeadProcessCommand.CommandType = CommandType.Text;
            objLeadProcessCommand.Connection = IDASDBConnection;
            objLeadProcessCommand.CommandTimeout = 0;


            SqlCommand objBatchProcessCommand = new SqlCommand();
            SqlDataAdapter objBatchAdapter = new SqlDataAdapter();
            SqlCommand objBatchExcelCommand = new SqlCommand();


            SqlConnection ProdBConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ProdDBConnection"].ConnectionString);
            SqlCommand objProdCommand = new SqlCommand();
            SqlDataAdapter objProdAdapter = new SqlDataAdapter();
            DataTable dtBatchOutput = new DataTable();

            string Temp = string.Empty;

            try
            {
                if (IDASDBConnection.State != ConnectionState.Open)
                    IDASDBConnection.Open();

                objLeadAdapter.SelectCommand = objLeadProcessCommand;
                objLeadAdapter.Fill(dtLeadProcess);
                string idno = string.Empty;
                List<MorrisGender> lstGender = new List<MorrisGender>();
                List<MorrisMaritalStaus> lstMaritalStatus = new List<MorrisMaritalStaus>();
                List<MorrisRiskCategories> lstMorrisRisks = new List<MorrisRiskCategories>();
                List<MorrisAlloyBreakDown> lstAlloy = new List<MorrisAlloyBreakDown>();
                List<LocationDistributorAgeGroup> lstLocationAgeGroup = new List<LocationDistributorAgeGroup>();
                IncomeBrackets incomeBrackets = new IncomeBrackets();
                AgeGrouGenders AgeGroup = new AgeGrouGenders();
                TotalRecordsAvailable totalRecordsAvailable = new TotalRecordsAvailable();
                MorrisGender objgender = new MorrisGender();
                MorrisRiskCategories objMorrisRisk = new MorrisRiskCategories();
                MorrisAlloyBreakDown objAlloy = new MorrisAlloyBreakDown();
                LocationDistributorAgeGroup objLocationAgeGroup = new LocationDistributorAgeGroup();
                MorrisMaritalStaus objStatus = new MorrisMaritalStaus();
                List<TotalRecordsAvailable> lstTotalRecords = new List<TotalRecordsAvailable>();

                foreach (DataRow dr in dtLeadProcess.Rows)
                {
                    Temp = dr["OutPutFileName"].ToString().Split('.')[0];
                    Console.WriteLine("Processing " + Temp);
                    
                    idno = GetIdnoAsync(dr["InputDetail"].ToString(), dr["LeadOutput"].ToString(), _eSService);
                    lstGender = new List<MorrisGender>();
                    if (!string.IsNullOrEmpty(idno))
                    {
                        objProdCommand = new SqlCommand();
                        objProdAdapter = new SqlDataAdapter();
                        dtBatchOutput = new DataTable();

                        if (ProdBConnection.State != ConnectionState.Open)
                            ProdBConnection.Open();
                        objProdCommand.Connection = ProdBConnection;
                        objProdCommand.CommandText = "QSPBATCHTRACINGREPORT";
                        objProdCommand.CommandType = CommandType.StoredProcedure;

                        objProdCommand.Parameters.Add(new SqlParameter("@IDNUMBER", idno));
                        objProdCommand.Parameters.Add(new SqlParameter("@dbname", IDASDBConnection.Database));
                        objProdCommand.CommandTimeout = 0;
                        objProdAdapter.SelectCommand = objProdCommand;
                        if (ProdBConnection.State != ConnectionState.Open)
                            ProdBConnection.Open();

                        objProdAdapter.Fill(dtBatchOutput);

                        if (File.Exists(ConfigurationManager.AppSettings["Path"] + "\\" + dr["OutPutFileName"].ToString()))
                            File.Delete(ConfigurationManager.AppSettings["Path"] + "\\" + dr["OutPutFileName"].ToString());

                        if (dtBatchOutput.Columns.Contains("consumerid"))
                            dtBatchOutput.Columns.Remove("consumerid");

                        ExportDataFromDataTable(dtBatchOutput, ConfigurationManager.AppSettings["Path"] + "\\" + dr["OutPutFileName"].ToString());

                        foreach (var gender in dtBatchOutput.AsEnumerable().GroupBy(t => t.Field<string>("PROFILE_GENDER")).Select(t => new { Gender = t.Key, Count = t.Count() }))
                        {
                            objgender = new MorrisGender();
                            objgender.value = gender.Count;

                            if (!string.IsNullOrEmpty(gender.Gender))
                            {
                                switch (gender.Gender.ToUpper())
                                {
                                    case "MALE":
                                        objgender.label = "Male";
                                        objgender.color = "#0710B1";
                                        break;
                                    case "FEMALE":
                                        objgender.label = "Female";
                                        objgender.color = "#ff80ff";
                                        break;
                                    default:
                                        objgender.label = "Unknown";
                                        objgender.color = null;
                                        break;
                                }
                            }
                            else
                            {
                                objgender.label = "Unknown";
                                objgender.color = null;
                            }
                            lstGender.Add(objgender);
                        }

                        lstMaritalStatus = new List<MorrisMaritalStaus>();

                        foreach (var status in dtBatchOutput.AsEnumerable().GroupBy(t => t.Field<string>("PROFILE_MARITAL_STATUS")).Select(t => new { Status = t.Key, Count = t.Count() }))
                        {
                            objStatus = new MorrisMaritalStaus();
                            objStatus.value = status.Count;

                            if (!string.IsNullOrEmpty(status.Status))
                            {
                                switch (status.Status.ToLower())
                                {
                                    case "married":
                                        objStatus.label = "Married";
                                        objStatus.color = "#FF1E00";
                                        break;
                                    case "single":
                                        objStatus.label = "Single";
                                        objStatus.color = "#94DE00";
                                        break;
                                    case "divorced":
                                        objStatus.label = "Divorced";
                                        objStatus.color = "#00767C";
                                        break;
                                    default:
                                        objStatus.color = null;
                                        break;
                                }
                            }
                            else
                            {
                                objStatus.label = "Unknown";
                            }
                            lstMaritalStatus.Add(objStatus);
                        }
                    }

                    lstMorrisRisks = new List<MorrisRiskCategories>();

                    foreach (var risk in dtBatchOutput.AsEnumerable().GroupBy(t => t.Field<string>("RISK_SCORE")).Select(t => new { Risk = t.Key, Count = t.Count() }))
                    {
                        objMorrisRisk = new MorrisRiskCategories();
                        objMorrisRisk.value = risk.Count;
                        if (!string.IsNullOrEmpty(risk.Risk))
                        {
                            switch (risk.Risk.ToLower())
                            {
                                case "low risk":
                                    objMorrisRisk.label = "Low Risk";
                                    objMorrisRisk.color = "#41DB00";
                                    break;
                                case "extremely low risk":
                                    objMorrisRisk.label = "Extremely Low Risk";
                                    objMorrisRisk.color = "#05390B";
                                    break;
                                case "high risk":
                                    objMorrisRisk.label = "High Risk";
                                    objMorrisRisk.color = "#CF0018";
                                    break;
                                case "should not be contacted":
                                    objMorrisRisk.label = "Should Not be Contacted";
                                    objMorrisRisk.color = "#5E0390";
                                    break;
                                case "medium risk":
                                    objMorrisRisk.label = "Medium Risk";
                                    objMorrisRisk.color = "#FF1800";
                                    break;
                                default:
                                    objMorrisRisk.label = "Unknown";
                                    objMorrisRisk.color = null;
                                    break;
                            }
                        }
                        else
                        {
                            objMorrisRisk.label = "Unknown";
                        }
                        lstMorrisRisks.Add(objMorrisRisk);
                    }

                    lstAlloy = new List<MorrisAlloyBreakDown>();

                    foreach (var alloy in dtBatchOutput.AsEnumerable().GroupBy(t => t.Field<string>("PROFILE_CONTACT_ABILITY")).Select(t => new { Alloy = t.Key, Count = t.Count() }))
                    {
                        objAlloy = new MorrisAlloyBreakDown();
                        objAlloy.value = alloy.Count;
                        if (!string.IsNullOrEmpty(alloy.Alloy))
                        {
                            switch (alloy.Alloy.ToLower())
                            {
                                case "highly contactable":
                                    objAlloy.label = "Highly contactable";
                                    objAlloy.color = "#008000";
                                    break;
                                case "very good":
                                    objAlloy.label = "Very Good";
                                    objAlloy.color = "#86592d";
                                    break;
                                case "fair":
                                    objAlloy.label = "Fair";
                                    objAlloy.color = "#e6b800";
                                    break;
                                case "average":
                                    objAlloy.label = "Average";
                                    objAlloy.color = "#ff751a";
                                    break;
                                case "poor":
                                    objAlloy.label = "Poor";
                                    objAlloy.color = "#6b6b47";
                                    break;
                                case "no contact":
                                    objAlloy.label = "No Contact";
                                    objAlloy.color = "#ff1a1a";
                                    break;
                                case "right party contact":
                                    objAlloy.label = "Right Party Contact";
                                    objAlloy.color = "#e600ac";
                                    break;
                                default:
                                    objAlloy.label = "Unkown";
                                    objAlloy.color = null;
                                    break;
                            }
                        }
                        else
                        {
                            objAlloy.label = "Unkown";
                        }
                        lstAlloy.Add(objAlloy);

                    }


                    lstLocationAgeGroup = new List<LocationDistributorAgeGroup>();

                    foreach (var location in dtBatchOutput.AsEnumerable().Select(t => t.Field<string>("HOME_ADDRESS_PROVINCE")).Distinct().ToList())
                    {

                        objLocationAgeGroup = new LocationDistributorAgeGroup();
                        objLocationAgeGroup.state = location;
                        objLocationAgeGroup.male18_34 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 18 && t.Field<int>("PROFILE_AGE_GROUP") <= 34).Count();
                        objLocationAgeGroup.male35_40 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 35 && t.Field<int>("PROFILE_AGE_GROUP") <= 40).Count();
                        objLocationAgeGroup.male41_60 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 41 && t.Field<int>("PROFILE_AGE_GROUP") <= 60).Count();
                        objLocationAgeGroup.maleabove61 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 61).Count();
                        objLocationAgeGroup.female18_34 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 18 && t.Field<int>("PROFILE_AGE_GROUP") <= 34).Count();
                        objLocationAgeGroup.female35_40 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 35 && t.Field<int>("PROFILE_AGE_GROUP") <= 40).Count();
                        objLocationAgeGroup.female41_60 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 41 && t.Field<int>("PROFILE_AGE_GROUP") <= 60).Count();
                        objLocationAgeGroup.femaleabove61 = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("HOME_ADDRESS_PROVINCE") == location && t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 61).Count();

                        lstLocationAgeGroup.Add(objLocationAgeGroup);
                    }

                    AgeGroup = new AgeGrouGenders();
                    AgeGroup.male = new int[4];
                    AgeGroup.feMale = new int[4];

                    AgeGroup.male[0] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 61).Count();
                    AgeGroup.male[1] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 41 && t.Field<int>("PROFILE_AGE_GROUP") <= 60).Count();
                    AgeGroup.male[2] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 35 && t.Field<int>("PROFILE_AGE_GROUP") <= 40).Count();
                    AgeGroup.male[3] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<int>("PROFILE_AGE_GROUP") >= 18 && t.Field<int>("PROFILE_AGE_GROUP") <= 34).Count();

                    AgeGroup.feMale[0] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 61).Count();
                    AgeGroup.feMale[1] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 41 && t.Field<int>("PROFILE_AGE_GROUP") <= 60).Count();
                    AgeGroup.feMale[2] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 35 && t.Field<int>("PROFILE_AGE_GROUP") <= 40).Count();
                    AgeGroup.feMale[3] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<int>("PROFILE_AGE_GROUP") >= 18 && t.Field<int>("PROFILE_AGE_GROUP") <= 34).Count();

                    incomeBrackets = new IncomeBrackets();
                    incomeBrackets.incomeBracketColumns = new string[12];
                    incomeBrackets.incomeBracketFeMale = new int[12];
                    incomeBrackets.incomeBracketMale = new int[12];

                    incomeBrackets.incomeBracketColumns[0] = "Unknown";
                    incomeBrackets.incomeBracketColumns[1] = " > 55,000";
                    incomeBrackets.incomeBracketColumns[2] = "45,001 to 55,000";
                    incomeBrackets.incomeBracketColumns[3] = "35,001 to 45,000";
                    incomeBrackets.incomeBracketColumns[4] = "25,001 to 35,000";
                    incomeBrackets.incomeBracketColumns[5] = "20,001 to 25,000";
                    incomeBrackets.incomeBracketColumns[6] = "15,001 to 20,000";
                    incomeBrackets.incomeBracketColumns[7] = "10,001 to 15,000";
                    incomeBrackets.incomeBracketColumns[8] = "5,001 to 10,000";
                    incomeBrackets.incomeBracketColumns[9] = "2,501 to 5,000";
                    incomeBrackets.incomeBracketColumns[10] = "1 to 2,500";
                    incomeBrackets.incomeBracketColumns[11] = "0";

                    incomeBrackets.incomeBracketFeMale[0] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower() == "Unknown".ToLower()).Count();
                    incomeBrackets.incomeBracketFeMale[1] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == " > 55,000".ToLower()).Count();
                    incomeBrackets.incomeBracketFeMale[2] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "45,001 to 55,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[3] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "35,001 to 45,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[4] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "25,001 to 35,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[5] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "20,001 to 25,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[6] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "15,001 to 20,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[7] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "10,001 to 15,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[8] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "5,001 to 10,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[9] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "2,501 to 5,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[10] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "1 to 2,500".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketFeMale[11] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "female" && t.Field<string>("INCOME").ToLower().Trim() == "0".ToLower().Trim()).Count();

                    incomeBrackets.incomeBracketMale[0] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower() == "Unknown".ToLower()).Count();
                    incomeBrackets.incomeBracketMale[1] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == " > 55,000".ToLower()).Count();
                    incomeBrackets.incomeBracketMale[2] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "45,001 to 55,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[3] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "35,001 to 45,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[4] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "25,001 to 35,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[5] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "20,001 to 25,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[6] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "15,001 to 20,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[7] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "10,001 to 15,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[8] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "5,001 to 10,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[9] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "2,501 to 5,000".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[10] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "1 to 2,500".ToLower().Trim()).Count();
                    incomeBrackets.incomeBracketMale[11] = dtBatchOutput.AsEnumerable().Where(t => t.Field<string>("PROFILE_GENDER").ToLower() == "male" && t.Field<string>("INCOME").ToLower().Trim() == "0".ToLower().Trim()).Count();

                    totalRecordsAvailable = new TotalRecordsAvailable();
                    totalRecordsAvailable.recordsvalue = "";
                    totalRecordsAvailable.maritalindicator = dtBatchOutput.AsEnumerable().Where(t => !string.IsNullOrEmpty(t.Field<string>("PROFILE_MARITAL_STATUS")) && t.Field<string>("PROFILE_MARITAL_STATUS").ToLower() == "married".ToLower()).Count();
                    totalRecordsAvailable.deceasedindicator = dtBatchOutput.AsEnumerable().Where(t => !string.IsNullOrEmpty(t.Field<string>("DECEASED_IND")) && t.Field<string>("DECEASED_IND").ToLower() == "Deceased".ToLower()).Count();
                    totalRecordsAvailable.emailaddress = dtBatchOutput.AsEnumerable().Where(t => !string.IsNullOrEmpty(t.Field<string>("X_EMAIL"))).Count();
                    totalRecordsAvailable.deedsindicator = dtBatchOutput.AsEnumerable().Where(t => !string.IsNullOrEmpty(t.Field<string>("PROFILE_HOMEOWNERSHIP")) && t.Field<string>("PROFILE_HOMEOWNERSHIP").ToUpper() == "Y").Count();
                    totalRecordsAvailable.directorshipindicator = dtBatchOutput.AsEnumerable().Where(t => !string.IsNullOrEmpty(t.Field<string>("PROFILE_DIRECTORSHIP")) && t.Field<string>("PROFILE_DIRECTORSHIP").ToUpper() == "Y").Count();
                    totalRecordsAvailable.adverseindicator = dtBatchOutput.AsEnumerable().Where(t => !string.IsNullOrEmpty(t.Field<string>("JUDGE_INDICATOR")) && t.Field<string>("JUDGE_INDICATOR").ToUpper() == "Y").Count();
                    lstTotalRecords = new List<TotalRecordsAvailable>();
                    lstTotalRecords.Add(totalRecordsAvailable);

                    objBatchExcelCommand = new SqlCommand();
                    objBatchExcelCommand.Connection = IDASDBConnection;
                    objBatchExcelCommand.CommandText = "update LeadsGenaration set ProfileReport = 1,MaritalStaus = '" + JsonConvert.SerializeObject(lstMaritalStatus) + "', RiskCategories = '" + JsonConvert.SerializeObject(lstMorrisRisks) + "', AlloyBreakDowns = '" + JsonConvert.SerializeObject(lstAlloy) + "', LocationServices = '" + JsonConvert.SerializeObject(lstLocationAgeGroup) + "', AgeGroupGenders = '" + JsonConvert.SerializeObject(AgeGroup) + "', ProfileGender = '" + JsonConvert.SerializeObject(lstGender) + "', IncomeBrackets = '" + JsonConvert.SerializeObject(incomeBrackets) + "', TotalRecordsAvailable = '" + JsonConvert.SerializeObject(lstTotalRecords) + "' where ID = '" + dr["LeadId"].ToString() + "';update LeadFileGeneration set Executed = 1, ExecutedDate = GETDATE() where ID = '" + dr["ID"].ToString() + "'";
                    objBatchExcelCommand.CommandType = CommandType.Text;
                    objBatchExcelCommand.CommandTimeout = 0;
                    if (IDASDBConnection.State != ConnectionState.Open)
                        IDASDBConnection.Open();
                    objBatchExcelCommand.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while processing : " + Temp);
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        public static string GetIdnoAsync(string InputDetail, string OutputDetail, ESService _eSService)
        {
            LeadGenerationResponse res = new LeadGenerationResponse();
            LeadsRequest request = new LeadsRequest();
            string IDNumbers = string.Empty;
            try
            {
                
                res = JsonConvert.DeserializeObject<LeadGenerationResponse>(OutputDetail);
                request = JsonConvert.DeserializeObject<LeadsRequest>(InputDetail);
                EsLeadInNoRequest req = new EsLeadInNoRequest();
                foreach (var alloy in request.alloylst)
                {
                    AlloyLeads alloylead = new AlloyLeads();
                    alloylead.AlloyName = alloy.AlloyName;
                    req.alloylst.Add(alloylead);
                }
                req.DoBRange1 = request.DisplayDate1;
                req.DoBRange2 = request.DisplayDate2;
                foreach (var inc in request.inclst)
                {
                    IncomeCategoryLeads inclead = new IncomeCategoryLeads();
                    inclead.IncomeCategoryName = inc.IncomeCategoryName;
                    req.inclst.Add(inclead);
                }
                req.IsAdversed = request.IsAdversed;
                req.IsCellNumber = request.IsCellNumber;
                req.IsDeceased = request.IsDeceased;
                req.IsDirector = request.IsDirector;
                req.IsEmail = request.IsEmail;
                req.IsEmployed = request.IsEmployed;
                req.IsHomeOwner = request.IsHomeOwner;
                req.LeadsRequested = request.RequiredLeads;
                foreach (var lsm in request.lsmlst)
                {
                    LSMLeads lsmlead = new LSMLeads();
                    lsmlead.LsmName = lsm.LsmName;
                    req.lsmlst.Add(lsmlead);
                }
                foreach (var risk in request.risklst)
                {
                    RiskCategoryLeads risklead = new RiskCategoryLeads();
                    risklead.RiskName = risk.RiskName;
                    req.risklst.Add(risklead);
                }
                foreach (var table in res.tableresponse)
                {
                    LeadsResponseIdno tablelead = new LeadsResponseIdno();
                    tablelead.AvailableCount = table.AvailableCount;
                    tablelead.Gender = table.Gender;
                    tablelead.Marital = table.Marital;
                    tablelead.ProvinceName = table.ProvinceName;
                    tablelead.RequiredCount = table.RequiredCount;
                    tablelead.TotalSum = table.TotalSum;
                    req.tablelst.Add(tablelead);
                }

                IDNumbers = _eSService.GetLeadIdNumber(req);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Occured while fetching the IDs for Lead process");
            }
            return IDNumbers;
        }

        public class Province
        {
            public string ProvinceName { get; set; }
            public int Percentage { get; set; }
        }
        public class Gender
        {
            public string GenderName { get; set; }
            public int Percentage { get; set; }
        }
        public class MaritalStatus
        {

            public string StatusName { get; set; }
            public int percentage { get; set; }
        }
        public class LSM
        {
            public string LsmName { get; set; }
            public bool Ischecked { get; set; }
        }
        public class RiskCategory
        {
            public string RiskName { get; set; }
            public bool Ischecked { get; set; }
        }
        public class Alloy
        {
            public string AlloyName { get; set; }
            public bool Ischecked { get; set; }
        }
        public class IncomeCategory
        {
            public string IncomeCategoryName { get; set; }
            public bool Ischecked { get; set; }
        }
        public class LeadsRequest
        {
            public Guid CustomerId { get; set; }
            public Guid CustomerUserId { get; set; }
            public int RequiredLeads { get; set; }
            public int DateRange1 { get; set; }
            public int DateRange2 { get; set; }
            public bool IsEmployed { get; set; }
            public bool IsDirector { get; set; }
            public bool IsHomeOwner { get; set; }
            public bool IsAdversed { get; set; }
            public bool IsCellNumber { get; set; }
            public bool IsEmail { get; set; }
            public string EmploymentLevel { get; set; }
            public bool IsDeceased { get; set; }
            public DateTime DisplayDate1 { get; set; }
            public DateTime DisplayDate2 { get; set; }
            public List<Gender> genderlst { get; set; } = new List<Gender>();
            public List<Province> provincelst { get; set; } = new List<Province>();
            public List<MaritalStatus> statuslst { get; set; } = new List<MaritalStatus>();
            public List<Alloy> alloylst { get; set; } = new List<Alloy>();
            public List<LSM> lsmlst { get; set; } = new List<LSM>();
            public List<RiskCategory> risklst { get; set; } = new List<RiskCategory>();
            public List<IncomeCategory> inclst { get; set; } = new List<IncomeCategory>();
        }

        public class LeadGenerationResponse
        {
            public Guid LeadId { get; set; }
            public string ProfileGender { get; set; }
            public string ProfileMarital { get; set; }
            public string ProfileRiskCategory { get; set; }
            public string ProfileAlloyBreakdown { get; set; }
            public string ProfileLocationDistributorAge { get; set; }
            public string ProfileAgeGroup { get; set; }
            public string ProfileIncomeBrackets { get; set; }
            public string ProfileTotalRecords { get; set; }
            public List<LeadsResponse> tableresponse { get; set; } = new List<LeadsResponse>();
            public List<MorrisGender> morrisGenders { get; set; } = new List<MorrisGender>();
            public List<MorrisMaritalStaus> morrisMaritalStaus { get; set; } = new List<MorrisMaritalStaus>();
            public List<MorrisRiskCategories> morrisRiskCategories { get; set; } = new List<MorrisRiskCategories>();
            public List<MorrisAlloyBreakDown> morrisAlloyBreakDowns { get; set; } = new List<MorrisAlloyBreakDown>();
            public List<LocationDistributorAgeGroup> locationDistributorAgeGroups { get; set; } = new List<LocationDistributorAgeGroup>();
            public AgeGrouGenders ageGrouGenders { get; set; } = new AgeGrouGenders();
            public IncomeBrackets IncomeBrackets { get; set; } = new IncomeBrackets();
            public List<TotalRecordsAvailable> totalRecordsAvailables { get; set; } = new List<TotalRecordsAvailable>();
            public string Leadtableresponse { get; set; }
            public string LeadInput { get; set; }
        }

        public class LeadsResponse
        {
            public string ProvinceName { get; set; }
            public string Gender { get; set; }
            public string Marital { get; set; }
            public int RequiredCount { get; set; }
            public int AvailableCount { get; set; }
            public int TotalSum { get; set; }
        }

        public class MorrisGender
        {
            public string label { get; set; }
            public int value { get; set; }
            public string color { get; set; }
        }

        public class MorrisMaritalStaus
        {
            public string label { get; set; }
            public int value { get; set; }
            public string color { get; set; }
        }
        public class MorrisRiskCategories
        {
            public string label { get; set; }
            public int value { get; set; }
            public string color { get; set; }
        }
        public class MorrisAlloyBreakDown
        {
            public string label { get; set; }
            public int value { get; set; }
            public string color { get; set; }
        }
        public class LocationDistributorAgeGroup
        {
            public string state { get; set; }
            public int male18_34 { get; set; }
            public int male35_40 { get; set; }
            public int male41_60 { get; set; }
            public int maleabove61 { get; set; }
            public int female18_34 { get; set; }
            public int female35_40 { get; set; }
            public int female41_60 { get; set; }
            public int femaleabove61 { get; set; }
        }

        public class AgeGrouGenders
        {
            public int[] male { get; set; }
            public int[] feMale { get; set; }
        }
        public class IncomeBrackets
        {
            public string[] incomeBracketColumns { get; set; }
            public int[] incomeBracketMale { get; set; }
            public int[] incomeBracketFeMale { get; set; }
        }
        public class TotalRecordsAvailable
        {
            public string recordsvalue { get; set; }
            public int maritalindicator { get; set; }
            public int emailaddress { get; set; }
            public int directorshipindicator { get; set; }
            public int deceasedindicator { get; set; }
            public int deedsindicator { get; set; }
            public int adverseindicator { get; set; }
        }
    }
}
