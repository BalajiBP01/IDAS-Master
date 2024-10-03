using Inspirit.IDAS.Data.Production;
using Inspirit.IDAS.Data.Raw;
using Inspirit.IDAS.ESData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.Text;
using Elasticsearch.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace Inspirit.IDAS.ETLApplication
{
    public class ETL
    {

        public int Take = 1000;
        int progress = 0;
        public Serilog.Core.Logger log;
        public System.Windows.Forms.TextBox TaskProgress_TextBox;
        public System.Windows.Forms.ProgressBar TaskProgress_ProgressBar;
        public System.Windows.Forms.DataGridView Log_DataGridView;
        public System.Windows.Forms.ComboBox cmbTable;
        public System.Windows.Forms.CheckBox chkbxProcessSelectedTable;
       
        List<Log> prodlogList = new List<Log>();
       
        DateTime LastDateProcessed;
        string processTable;
        public async Task Process()
        {

            DateTime ProcessStartDate = DateTime.Now;

            try
            {
                processTable = string.Empty;
                string strElasticIPAddress = System.Configuration.ConfigurationManager.AppSettings["ElasticDBIP"].ToString();
                if (cmbTable.SelectedIndex > 0)
                {
                    processTable = cmbTable.SelectedItem.ToString();
                }

                await ExecuteSPs(strElasticIPAddress);

                ShowTaskProgress(0, "ETL Process Completed.");

            }
            catch (Exception ex)
            {
                ShowTaskProgress(0, ex.Message);
                throw ex;
            }
        }

        public async Task ExecuteSPs(string elasticIP)
        {
            SqlConnection objConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ProdDBConnection"].ConnectionString);

            SqlCommand cmd;

            SqlDataAdapter sqlDataAdapter;
            SqlDataReader rdrDataReader;

            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Table"));
                dt.Columns.Add(new DataColumn("Start Time", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("End Time", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("Time Taken"));

                DateTime dtStartTime = DateTime.Now;
                DateTime dtEndTime = DateTime.Now;
                TimeSpan TimeTake = dtEndTime - dtStartTime;
                DataRow dr;

                int InsertESData = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TakeCount"]); ;
                ESService es = new ESService(elasticIP);
                int e = 0, p = 0, a = 0, temp = 0, i = 0;

                object strTemp1fordtreader;
                object strTemp2fordtreader;
                object strTemp3fordtreader;
                object strTemp4fordtreader;
                object strTemp5fordtreader;
                object strTemp6fordtreader;
                object strTemp7fordtreader;
                object strTemp8fordtreader;
                object strTemp9fordtreader;
                object strTemp10fordtreader;

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                string startId = string.Empty;
                string endId = string.Empty;
                StringResponse stringResponse;

                //Variables for Time tracking.

                DateTime consumerStart = DateTime.Now;
                DateTime consumerEnd = DateTime.Now;
                TimeSpan ConsumerTime = consumerEnd - consumerStart;
                DateTime addressStart = DateTime.Now;
                DateTime addressEnd = DateTime.Now;
                TimeSpan addressTime = addressEnd - addressStart;
                DateTime phoneStart = DateTime.Now;
                DateTime phoneEnd = DateTime.Now;
                TimeSpan phoneTime = phoneEnd - phoneStart;
                DateTime emailStart = DateTime.Now;
                DateTime emailEnd = DateTime.Now;
                TimeSpan emailTime = emailEnd - emailStart;
                DateTime homeStart = DateTime.Now;
                DateTime homeEnd = DateTime.Now;
                TimeSpan homeTime = emailEnd - emailStart;
                DateTime lsmstart = DateTime.Now;
                DateTime lsmEnd = DateTime.Now;
                TimeSpan lsmTime = lsmEnd - lsmstart;

                DateTime commercialStart = DateTime.Now;
                DateTime commercialEnd = DateTime.Now;
                TimeSpan CommercialTime = commercialEnd - commercialStart;
                commercialStart = DateTime.Now;

                DateTime addressSearchstart = DateTime.Now;
                DateTime addressSearchEnd = DateTime.Now;
                TimeSpan addressSearchTime = lsmEnd - lsmstart;

                //Telephone Master

                if (processTable == "TelephoneTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "TelephoneMaster";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    cmd.CommandText = "qspSSISToProcessTelephoneMaster";

                    ShowTaskProgress(50, "Processing 'TelephoneMaster'");
                    Log_DataGridView.DataSource = dt;
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;

                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "TelephoneMaster";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;

                    ShowTaskProgress(100, "'TelephoneMaster' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Consumer Master
                if (processTable == "ConsumerTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerMaster";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;

                    ShowTaskProgress(50, "Processing 'ConsumerMaster'");

                    cmd.CommandText = "qspSSISToProcessConsumerMaster";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerMaster";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerMaster' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;

                }
                //Home Affairs
                if (processTable == "ConsumerHomeAffairsTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerHomeAffairs";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'ConsumerHomeAffairs'");
                    cmd.CommandText = "qspSSISToProcessConsumerHomeAffairs";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerHomeAffairs";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "ConsumerHomeAffairs Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Consumer Employment Occupations
                if (processTable == "ConsumerEmploymentOccupationTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerEmploymentOccupation";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;

                    ShowTaskProgress(50, "Processing 'ConsumerEmploymentOccupation'");
                    cmd.CommandText = "qspSSISToProcessConsumerOccupations";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerEmploymentOccupation";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerEmploymentOccupation' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Consumer Employments
                if (processTable == "ConsumerEmploymentTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerEmployments";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;

                    ShowTaskProgress(50, "Processing 'ConsumerEmploymentOccupation'");
                    cmd.CommandText = "qspSSISToProcessConsumerEmployemnts";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerEmployments";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerEmploymentOccupation' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Consumer Address
                if (processTable == "ConsumerAddressTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerAddress";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'ConsumerAddress'");
                    cmd.CommandText = "qspSSISToProcessConsumerAddress";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerAddress";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerAddress' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Consumer Emails
                if (processTable == "ConsumerEmailTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerEmails";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'ConsumerEmails'");
                    cmd.CommandText = "qspSSISToProcessConsumerEmails";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerEmails";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerEmails' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Consumer Telephone
                if (processTable == "ConsumerTelephoneTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerTelephone";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'ConsumerTelephone'");
                    cmd.CommandText = "qspSSISToProcessConsumerTelephone";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerTelephone";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerTelephone' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Debit Reviews
                if (processTable == "ConsumerDebitReviewTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerDebtReview";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'ConsumerDebtReview'");
                    cmd.CommandText = "qspSSISToProcessConsumerDebtReview";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerDebtReview";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerDebtReview' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Consumer Judgements
                if (processTable == "ConsumerJudementTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ConsumerJudgement";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'ConsumerJudgement'");
                    cmd.CommandText = "qspSSISToProcessConsumerJudgement";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ConsumerJudgement";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ConsumerJudgement' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Commercials
                if (processTable == "CommercialTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "CommercialsMaster";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'CommercialsMaster'");
                    cmd.CommandText = "qspSSISToProcessCommercialsMaster";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "CommercialsMaster";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'CommercialsMaster' Completedd");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Commercial Telephone
                if (processTable == "CommercialTelephoneTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "CommercialTelephone";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'CommercialTelephone'");
                    cmd.CommandText = "qspSSISToProcessCommercialTelephone";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "CommercialTelephone";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'CommercialTelephone' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Commercial Address
                if (processTable == "CommercialAddressTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "CommercialAddress";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'CommercialAddress'");
                    cmd.CommandText = "qspSSISToProcessCommercialAddress";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "CommercialAddress";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'CommercialAddress' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Commercial Auditor
                if (processTable == "CommercialAuditorTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "CommercialAuditor";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'CommercialAuditor'");
                    cmd.CommandText = "qspSSISToProcessCommercialAuditor";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "CommercialAuditor";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'CommercialAuditor' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Commercial Judgements
                if (processTable == "CommercialJudgementTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "CommercialJudgment";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'CommercialJudgment'");
                    cmd.CommandText = "qspSSISToProcessCommercialJudgment";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "CommercialJudgment";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'CommercialJudgment' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Directors
                if (processTable == "DirectorTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "DirectorMaster";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'DirectorMaster'");
                    cmd.CommandText = "qspSSISToProcessDirectorMaster";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "DirectorMaster";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'DirectorMaster' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Director Telephones
                if (processTable == "DirectorTelephoneTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "DirectorTelephone";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'DirectorTelephone'");
                    cmd.CommandText = "qspSSISToProcessDirectorTelephone";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "DirectorTelephone";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'DirectorTelephone' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Director Address
                if (processTable == "DirectorAddressTable" || processTable == String.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "DirectorAddress";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'DirectorAddress'");
                    cmd.CommandText = "qspSSISToProcessDirectorAddress";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "DirectorAddress";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'DirectorAddress' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Commercial Director
                if (processTable == "CommercialDirectorTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "CommercialDirector";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'CommercialDirector'");
                    cmd.CommandText = "qspSSISToProcessCommercialDirector";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "CommercialDirector";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'CommercialDirector' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Property Deeds
                if (processTable == "PropertyDeedTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "PropertyDeed";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'PropertyDeed'");
                    cmd.CommandText = "qspSSISToProcessPropertyDeed";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "PropertyDeed";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'PropertyDeed' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Buyers
                if (processTable == "PropertyBuyerTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "PropertyDeedBuyers";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'PropertyDeedBuyers'");
                    cmd.CommandText = "qspSSISToProcessPropertyDeedBuyers";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "PropertyDeedBuyers";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'PropertyDeedBuyers' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                // Sellers
                if (processTable == "PropertySellerTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "PropertyDeedSeller";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'PropertyDeedSeller'");
                    cmd.CommandText = "qspSSISToProcessPropertyDeedSeller";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "PropertyDeedSeller";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'PropertyDeedSeller' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }


                //endorsement

                if (processTable == "EndorsementTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "Endorsements";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'Endorsements'");
                    cmd.CommandText = "qspSSISToProcessEndorsement";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "Endorsements";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'Endorsements' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }


                //Auditor Address
                if (processTable == "AuditorAddressTable" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dtStartTime = DateTime.Now;
                    dr = dt.NewRow();

                    dr["Table"] = "ProcessAuditorAddress";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'ProcessAuditorAddress'");
                    cmd.CommandText = "qspSSISToProcessAuditorAddress";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ProcessAuditorAddress";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'ProcessAuditorAddress' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Address Parsing
                if (processTable == "AddressParsing" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "Address Parsing";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'Address Parsing'");
                    cmd.CommandText = "QSPCONSUMERADDRESSPROVINCEDETAILS";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "Address Parsing";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'Address Parsing' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //Alloy Calculation
                if (processTable == "AlloyCalculation" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "Alloy Calculation";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'Alloy Calculation'");
                    cmd.CommandText = "QSPAlloyClaculation";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "Alloy Calculation";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'Alloy Calculation' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                //LSM
                if (processTable == "LSMCalculation" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "LSM Calculation";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(50, "Processing 'LSM Calculation'");
                    cmd.CommandText = "QSPLSMCalculation";
                    await cmd.ExecuteNonQueryAsync();
                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "LSM Calculation";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(100, "'LSM Calculation' Completed");
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                //Elastic Search - Consumers
                consumerStart = DateTime.Now;

                if (processTable == "ES-ConsumerMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(0, "Pushing Consumer Information to Elastic");
                    cmd.CommandText = "select ConsumerID,PassportNo,FirstName,Surname,IDNO,GenderInd,BirthDate,Alloy,LSM from Consumers where RecordStatusInd = 1 and (IsESSynced = 0 or IsESSynced is null) and (DATEDIFF(Year,BirthDate,getdate()) >=18 or BirthDate is null)";

                    rdrDataReader = cmd.ExecuteReader();

                    dtStartTime = DateTime.Now;

                    if (rdrDataReader.HasRows)
                    {
                        temp = 0;
                        startId = string.Empty; endId = string.Empty;


                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Consumer - " + startId + " - " + endId + ".txt");
                                }

                                consumerEnd = DateTime.Now;
                                ConsumerTime = consumerEnd - consumerStart;
                                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            if (rdrDataReader["ConsumerID"] != DBNull.Value)
                                strTemp1fordtreader = Convert.ToString(rdrDataReader["ConsumerID"]);
                            else
                                strTemp1fordtreader = null;
                            if (rdrDataReader["PassportNo"] != DBNull.Value)
                                strTemp2fordtreader = Convert.ToString(rdrDataReader["PassportNo"]);
                            else
                                strTemp2fordtreader = null;
                            if (rdrDataReader["FirstName"] != DBNull.Value)
                                strTemp3fordtreader = Convert.ToString(rdrDataReader["FirstName"]);
                            else
                                strTemp3fordtreader = null;
                            if (rdrDataReader["Surname"] != DBNull.Value)
                                strTemp4fordtreader = Convert.ToString(rdrDataReader["Surname"]);
                            else
                                strTemp4fordtreader = null;
                            if (rdrDataReader["IDNO"] != DBNull.Value)
                                strTemp5fordtreader = Convert.ToString(rdrDataReader["IDNO"]);
                            else
                                strTemp5fordtreader = null;
                            if (rdrDataReader["GenderInd"] != DBNull.Value)
                                strTemp6fordtreader = Convert.ToString(rdrDataReader["GenderInd"]);
                            else
                                strTemp6fordtreader = null;
                            if (rdrDataReader["BirthDate"] != DBNull.Value)
                                strTemp7fordtreader = Convert.ToDateTime(rdrDataReader["BirthDate"]);
                            else
                                strTemp7fordtreader = null;
                            if (rdrDataReader["Alloy"] != DBNull.Value)
                                strTemp8fordtreader = Convert.ToString(rdrDataReader["Alloy"]);
                            else
                                strTemp8fordtreader = null;
                            if (rdrDataReader["LSM"] != DBNull.Value)
                                strTemp9fordtreader = Convert.ToString(rdrDataReader["LSM"]);
                            else
                                strTemp9fordtreader = null;

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            
                            sb.Append("{\"index\" : { \"_index\" : \"consumerindex\", \"_type\" : \"consumerindex\", \"_id\" : \"");
                            sb.Append(Convert.ToString(strTemp1fordtreader));
                            sb.Append("\" } } \n");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("consumerid");
                                writer.WriteValue(Convert.ToString(strTemp1fordtreader));
                                writer.WritePropertyName("passportno");
                                writer.WriteValue(Convert.ToString(strTemp2fordtreader));
                                writer.WritePropertyName("firstname");
                                writer.WriteValue(Convert.ToString(strTemp3fordtreader));
                                writer.WritePropertyName("surname");
                                writer.WriteValue(Convert.ToString(strTemp4fordtreader));
                                writer.WritePropertyName("idnumber");
                                writer.WriteValue(Convert.ToString(strTemp5fordtreader));
                                writer.WritePropertyName("gender");
                                writer.WriteValue(Convert.ToString(strTemp6fordtreader));
                                writer.WritePropertyName("dateofbirth");
                                writer.WriteValue(Convert.ToDateTime(strTemp7fordtreader));
                                writer.WritePropertyName("alloy");
                                writer.WriteValue(Convert.ToString(strTemp8fordtreader));
                                writer.WritePropertyName("lsm");
                                writer.WriteValue(Convert.ToString(strTemp9fordtreader));
                                writer.WritePropertyName("emailindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("deedsindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("adverseindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("directorindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("maritalstatus");
                                writer.WriteValue("Unknown");
                                writer.WritePropertyName("incomercategoty");
                                writer.WriteValue("Unknown");
                                writer.WritePropertyName("riskcategory");
                                writer.WriteValue("Unknown");
                                writer.WritePropertyName("province");
                                writer.WriteValue("Unknown");
                                writer.WritePropertyName("cellindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("employmentindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("iscontactexists");
                                writer.WriteValue("0");
                                writer.WritePropertyName("isleadeligible");
                                writer.WriteValue(Convert.ToString(strTemp5fordtreader).Length >= 13 ? "1" : "0");
                                writer.WriteEndObject();
                            }
                            //}
                            temp++;

                            i++;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Consumer - " + startId + " - " + endId + ".txt");
                            }
                        }
                        rdrDataReader.Close();
                    }



                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;

                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                consumerEnd = DateTime.Now;
                ConsumerTime = consumerEnd - consumerStart;

                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString());

                if (sb.Length > 0)
                    sb.Clear();

                addressStart = DateTime.Now;
                List<string> documentLst = new List<string>();

                //Elastic Search - Consumers Address
                if (processTable == "ES-ConsumerAddressMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerAddressMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    ShowTaskProgress(0, "Pushing Consumer Information to Elastic");
                    cmd.CommandText = "select ConsumerAddressID, ConsumerAddresses.ConsumerID,OriginalAddress1,OriginalAddress2,OriginalAddress3,OriginalAddress4, OriginalPostalCode,ConsumerAddresses.Province,Town,Region,AddressTypeInd,ConsumerAddresses.CreatedOnDate from ConsumerAddresses inner join Consumers on ConsumerAddresses.ConsumerID = Consumers.ConsumerID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 and ConsumerAddresses.RecordStatusInd = 1 order by ConsumerID; ";

                    rdrDataReader = cmd.ExecuteReader();


                    dtStartTime = DateTime.Now;

                    if (rdrDataReader.HasRows)
                    {
                        string ConsumerAddressID = string.Empty;
                        string ConsumerId = string.Empty;
                        string CurrentConsumerId = string.Empty;
                        string Address = "Unknown";
                        string Town = "Unknown";
                        string Region = "Unknown";
                        DateTime CreatedOnDate = DateTime.MinValue;
                        DateTime tmpDate = DateTime.MinValue;
                        startId = string.Empty; endId = string.Empty;
                        temp = 0;
                        string Province = string.Empty;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "ConsumerAddress - " + startId + " - " + endId + ".txt");
                                }
                                addressEnd = DateTime.Now;
                                addressTime = addressEnd - addressStart;
                                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());
                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;
                            }

                            ConsumerAddressID = rdrDataReader["ConsumerAddressID"].ToString();
                            startId = string.IsNullOrEmpty(startId) ? ConsumerAddressID : startId;
                            endId = ConsumerAddressID;

                            CurrentConsumerId = rdrDataReader["ConsumerID"].ToString();

                            ConsumerId = string.IsNullOrEmpty(ConsumerId) ? CurrentConsumerId : ConsumerId;


                            strTemp1fordtreader = rdrDataReader["OriginalAddress1"];
                            strTemp2fordtreader = rdrDataReader["OriginalAddress2"];
                            strTemp3fordtreader = rdrDataReader["OriginalAddress3"];
                            strTemp4fordtreader = rdrDataReader["OriginalAddress4"];
                            strTemp5fordtreader = rdrDataReader["OriginalPostalCode"];
                            strTemp6fordtreader = rdrDataReader["Province"];
                            strTemp7fordtreader = rdrDataReader["Town"];
                            strTemp8fordtreader = rdrDataReader["Region"];
                            strTemp9fordtreader = rdrDataReader["AddressTypeInd"];
                            strTemp10fordtreader = rdrDataReader["CreatedOnDate"];
                            CreatedOnDate = strTemp10fordtreader == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(strTemp10fordtreader);


                            Address = ((!string.IsNullOrEmpty(strTemp1fordtreader.ToString()) ? strTemp1fordtreader.ToString().Trim() + " " : "")
                                                                             + (!string.IsNullOrEmpty(strTemp2fordtreader.ToString()) ? strTemp2fordtreader.ToString().Trim() + " " : "")
                                                                             + (!string.IsNullOrEmpty(strTemp3fordtreader.ToString()) ? strTemp3fordtreader.ToString().Trim() + " " : "")
                                                                             + (!string.IsNullOrEmpty(strTemp4fordtreader.ToString()) ? strTemp4fordtreader.ToString().Trim() + " " : "")
                                                                             + (!string.IsNullOrEmpty(strTemp5fordtreader.ToString()) ? strTemp5fordtreader.ToString().Trim() + " " : "")
                                                                             + (!string.IsNullOrEmpty(strTemp6fordtreader.ToString()) ? strTemp6fordtreader.ToString().Trim() + " " : "")
                                                                             + (!string.IsNullOrEmpty(strTemp7fordtreader.ToString()) ? strTemp7fordtreader.ToString().Trim() + " " : "")
                                                                             + (!string.IsNullOrEmpty(strTemp8fordtreader.ToString()) ? strTemp8fordtreader.ToString().Trim() + " " : ""));

                            if (ConsumerId == CurrentConsumerId)
                            {
                                documentLst.Add(Address);
                                if (strTemp9fordtreader.ToString() == "1" && CreatedOnDate >= tmpDate)
                                {
                                    if (strTemp6fordtreader != DBNull.Value)
                                    {
                                        Province = strTemp6fordtreader == DBNull.Value ? "Unknown" : strTemp6fordtreader.ToString().ToLower();
                                        Town = strTemp7fordtreader == DBNull.Value ? "Unknown" : strTemp7fordtreader.ToString().ToLower();
                                        Region = strTemp8fordtreader == DBNull.Value ? "Unknown" : strTemp8fordtreader.ToString().ToLower();
                                        tmpDate = CreatedOnDate;
                                    }
                                }

                            }
                            else if (ConsumerId != CurrentConsumerId)
                            {



                                sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(ConsumerId);
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");

                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {

                                    writer.WriteStartObject();
                                    writer.WritePropertyName("province");
                                    writer.WriteValue(Province);
                                    writer.WritePropertyName("town");
                                    writer.WriteValue(Town);
                                    writer.WritePropertyName("region");
                                    writer.WriteValue(Region);
                                    writer.WritePropertyName("address");
                                    writer.WriteStartArray();
                                    foreach (string str in documentLst)
                                        writer.WriteValue(str.Trim());
                                    writer.WriteEnd();
                                    writer.WriteEndObject();
                                }


                                sb.Append("}");
                                documentLst.Clear();
                                ConsumerId = CurrentConsumerId;
                                documentLst.Add(Address);
                                Address = "Unknown";
                                Town = "Unknown";
                                Region = "Unknown";
                                tmpDate = DateTime.MinValue;
                                if (strTemp9fordtreader.ToString() == "1" && CreatedOnDate >= tmpDate)
                                {

                                    Province = strTemp6fordtreader == DBNull.Value ? "Unknown" : strTemp6fordtreader.ToString().ToLower();
                                    Town = strTemp7fordtreader == DBNull.Value ? "Unknown" : strTemp7fordtreader.ToString().ToLower();
                                    Region = strTemp8fordtreader == DBNull.Value ? "Unknown" : strTemp8fordtreader.ToString().ToLower();
                                    tmpDate = CreatedOnDate;
                                }

                            }

                            temp++;

                            a++;
                        }
                        rdrDataReader.Close();
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(ConsumerId);
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {

                                writer.WriteStartObject();
                                writer.WritePropertyName("province");
                                writer.WriteValue(Province);
                                writer.WritePropertyName("town");
                                writer.WriteValue(Town);
                                writer.WritePropertyName("region");
                                writer.WriteValue(Region);
                                writer.WritePropertyName("address");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                            Province = string.Empty;
                            Town = string.Empty;
                            Region = string.Empty;
                            tmpDate = DateTime.MinValue;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerAddress - " + startId + " - " + endId + ".txt");
                            }
                        }


                    }


                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerAddressMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                addressEnd = DateTime.Now;
                addressTime = addressEnd - addressStart;

                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());

                if (sb.Length > 0)
                    sb.Clear();

                if (documentLst.Count > 0)
                    documentLst.Clear();

                phoneStart = DateTime.Now;

                //Elastic Search - Consumers Telephone
                if (processTable == "ES-ConsumerTelephoneMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerTelephoneMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select ConsumerTelephoneID, ConsumerTelephones.ConsumerID,TelephoneNo,InternationalDialingCode,TelephoneTypeInd from ConsumerTelephones inner join Consumers on ConsumerTelephones.ConsumerID = Consumers.ConsumerID and IsESSynced = 0  and Consumers.RecordStatusInd = 1 and ConsumerTelephones.RecordStatusInd = 1 inner join Telephones on ConsumerTelephones.TelephoneID = Telephones.TelephoneID and Telephones.RecordStatusInd =1 order by ConsumerID; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        string PhoneNumber = string.Empty;
                        string ConsumerId = string.Empty;
                        string CurrentConsumerId = string.Empty;
                        string ConsumerTelephoneID = string.Empty;
                        string InternationalDialingCode = string.Empty;
                        string TelephoneTypeInd = string.Empty;
                        bool add = false;
                        string IsCell = "0";
                        temp = 0;
                        startId = string.Empty; endId = string.Empty;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "ConsumerPhones - " + startId + " - " + endId + ".txt");
                                }

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;

                                phoneEnd = DateTime.Now;
                                phoneTime = phoneEnd - phoneStart;
                                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());

                            }

                            PhoneNumber = string.Empty;
                            ConsumerTelephoneID = string.Empty;
                            InternationalDialingCode = string.Empty;
                            TelephoneTypeInd = string.Empty;
                            add = false;

                            ConsumerTelephoneID = rdrDataReader["ConsumerTelephoneID"].ToString();
                            CurrentConsumerId = rdrDataReader["ConsumerID"].ToString();
                            ConsumerId = string.IsNullOrEmpty(ConsumerId) ? CurrentConsumerId : ConsumerId;
                            PhoneNumber = rdrDataReader["TelephoneNo"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            InternationalDialingCode = rdrDataReader["InternationalDialingCode"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            TelephoneTypeInd = rdrDataReader["TelephoneTypeInd"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            if (InternationalDialingCode == "27")
                            {
                                PhoneNumber = "0" + PhoneNumber;
                                if (PhoneNumber.Length == 10)
                                    add = true;
                            }
                            else
                            {
                                PhoneNumber = InternationalDialingCode + PhoneNumber;
                                add = true;
                            }

                            if (add)
                            {

                                if (ConsumerId == CurrentConsumerId)
                                {
                                    documentLst.Add(PhoneNumber);
                                    if (TelephoneTypeInd == "3") IsCell = "1";
                                }
                                else if (ConsumerId != CurrentConsumerId)
                                {
                                    sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                    sb.Append("{ \"update\" : {\"_id\" : \"");
                                    sb.Append(ConsumerId);
                                    sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                    sb.Append("{ \"doc\" :  ");

                                    sw = new StringWriter(sb);
                                    using (JsonWriter writer = new JsonTextWriter(sw))
                                    {

                                        writer.WriteStartObject();
                                        writer.WritePropertyName("cellindicator");
                                        writer.WriteValue(IsCell);
                                        writer.WritePropertyName("iscontactexists");
                                        writer.WriteValue("1");
                                        writer.WritePropertyName("phonenumber");
                                        writer.WriteStartArray();
                                        foreach (string str in documentLst)
                                            writer.WriteValue(str.Trim());
                                        writer.WriteEnd();
                                        writer.WriteEndObject();
                                    }
                                    sb.Append("}");
                                    documentLst.Clear();
                                    ConsumerId = CurrentConsumerId;
                                    documentLst.Add(PhoneNumber);
                                    IsCell = TelephoneTypeInd == "3" ? "1" : "0";
                                }
                            }
                            temp++;

                            p++;
                        }
                        rdrDataReader.Close();
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(ConsumerId);
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("cellindicator");
                                writer.WriteValue(IsCell);
                                writer.WritePropertyName("iscontactexists");
                                writer.WriteValue("1");
                                writer.WritePropertyName("phonenumber");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerPhones - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }


                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerTelephoneMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                phoneEnd = DateTime.Now;
                phoneTime = phoneEnd - phoneStart;
                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                emailStart = DateTime.Now;

                //Elastic Search - Consumers Emails
                if (processTable == "ES-ConsumerEmailMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerEmailMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select ID, ConsumerEmails.ConsumerID,EmailID from ConsumerEmails inner join Consumers on ConsumerEmails.ConsumerID = Consumers.ConsumerID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 and ConsumerEmails.RecordStatusInd = 1 order by ConsumerID; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        temp = 0;
                        startId = string.Empty; endId = string.Empty;
                        string ConsumerId = string.Empty;
                        string CurrentConsumerId = string.Empty;
                        string email = string.Empty;
                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "ConsumerEmails - " + startId + " - " + endId + ".txt");
                                }
                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;

                                emailEnd = DateTime.Now;
                                emailTime = emailEnd - emailStart;
                                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString() + " Email Count: " + e.ToString() + " Time Taken: " + Math.Round(emailTime.TotalMinutes, 2).ToString());
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["ID"];
                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                            CurrentConsumerId = rdrDataReader["ConsumerID"].ToString();
                            ConsumerId = string.IsNullOrEmpty(ConsumerId) ? CurrentConsumerId : ConsumerId;

                            email = rdrDataReader["EmailID"].ToString();

                            if (ConsumerId == CurrentConsumerId)
                            {
                                documentLst.Add(email);
                            }
                            else if (ConsumerId != CurrentConsumerId)
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(ConsumerId);
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");

                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {

                                    writer.WriteStartObject();
                                    writer.WritePropertyName("emailindicator");
                                    writer.WriteValue("1");
                                    writer.WritePropertyName("email");
                                    writer.WriteStartArray();
                                    foreach (string str in documentLst)
                                        writer.WriteValue(str.Trim());
                                    writer.WriteEnd();
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                                documentLst.Clear();
                                ConsumerId = CurrentConsumerId;
                                documentLst.Add(email);
                            }
                            temp++;

                            e += 1;
                        }
                        rdrDataReader.Close();

                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(ConsumerId);
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("emailindicator");
                                writer.WriteValue("1");
                                writer.WritePropertyName("email");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerEmails - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }


                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerEmailMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                emailEnd = DateTime.Now;
                emailTime = emailEnd - emailStart;
                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString() + " Email Count: " + e.ToString() + " Time Taken: " + Math.Round(emailTime.TotalMinutes, 2).ToString());

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                homeStart = DateTime.Now;

                //Elastic Search - Consumers Home Affairs
                if (processTable == "ES-ConsumerHomeAffairsMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerHomeAffairsMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select HomeAffairsID, ConsumerHomeAffairs.ConsumerID ,MarriageDate,DivorceDate,ConsumerHomeAffairs.DeceasedStatus from ConsumerHomeAffairs inner join Consumers on ConsumerHomeAffairs.ConsumerID = Consumers.ConsumerID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 and ConsumerHomeAffairs.RecordStatusInd = 'A' order by ConsumerID; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int HomeCount = 0;
                        temp = 0;
                        homeStart = DateTime.Now;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "ConsumerHomeAffires - " + startId + " - " + endId + ".txt");
                                }

                                homeEnd = DateTime.Now;
                                homeTime = homeEnd - homeStart;
                                ShowTaskProgress(100, " Home Affairs Count: " + HomeCount.ToString() + " Time Taken: " + Math.Round(homeTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["HomeAffairsID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            //strInsertString.Append("{\"index\" : { \"_index\" : \"consumersinformation\", \"_type\" : \"consumersinformation\", \"_id\" : \"" + strTemp1fordtreader.ToString() + "\" } } \n");
                            strTemp2fordtreader = rdrDataReader.GetValue(1);
                            strTemp3fordtreader = rdrDataReader.GetValue(2);
                            strTemp4fordtreader = rdrDataReader.GetValue(3);
                            strTemp5fordtreader = rdrDataReader.GetValue(4);

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(strTemp2fordtreader.ToString());
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();

                                writer.WritePropertyName("maritalstatus");
                                writer.WriteValue(strTemp3fordtreader == DBNull.Value ? "Single" : strTemp4fordtreader == DBNull.Value ? "Married" : Convert.ToDateTime(strTemp4fordtreader) > Convert.ToDateTime(strTemp3fordtreader) ? "Divorced" : "Married");
                                writer.WritePropertyName("isdeceased");
                                writer.WriteValue(strTemp5fordtreader == DBNull.Value ? false : strTemp5fordtreader.ToString().ToUpper() == "Deceased".ToUpper() ? true : false);
                                writer.WriteEndObject();
                            }
                            sb.Append("}");

                            HomeCount++;
                            temp++;
                        }
                        rdrDataReader.Close();
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerHomeAffires - " + startId + " - " + endId + ".txt");
                            }
                            homeEnd = DateTime.Now;
                            homeTime = homeEnd - homeStart;
                            ShowTaskProgress(100, " Home Affairs Count: " + HomeCount.ToString() + " Time Taken: " + Math.Round(homeTime.TotalMinutes, 2).ToString());

                        }
                    }


                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerHomeAffairsMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                homeEnd = DateTime.Now;
                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;

                //Elastic Search - Consumers LSM
                if (processTable == "ES-ConsumerLSMMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerLSMMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select LSM.ConsumerID,RiskCategory,IncomeCategory from LSM inner join Consumers on Consumers.ConsumerID = LSM.CONSUMERID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 order by ConsumerID;  ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int lsmcount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "LSM - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " LSM: " + lsmcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["ConsumerID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                           
                            strTemp2fordtreader = rdrDataReader.GetValue(1);
                            strTemp3fordtreader = rdrDataReader.GetValue(2);

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(strTemp1fordtreader.ToString());
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();

                                writer.WritePropertyName("riskcategory");
                                writer.WriteValue(strTemp2fordtreader == DBNull.Value ? "Unknown" : strTemp2fordtreader.ToString());
                                writer.WritePropertyName("incomercategoty");
                                writer.WriteValue(strTemp3fordtreader == DBNull.Value ? "Unknown" : strTemp3fordtreader.ToString());
                                writer.WriteEndObject();
                            }
                            sb.Append("}");

                            lsmcount++;
                            temp++;
                        }
                        rdrDataReader.Close();
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "LSM - " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " LSM Count: " + lsmcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerLSMMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }
                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;

                //Elastic Search - Consumers PropertyDeeds
                if (processTable == "ES-ConsumerPropertyDeedsMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerPropertyDeedsMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select distinct Consumers.ConsumerID from PropertyDeedBuyers inner join PropertyDeeds on PropertyDeedBuyers.PropertyDeedId = PropertyDeeds.PropertyDeedID and IsCurrentOwner = 1 and PropertyDeedBuyers.RecordStatusInd = 'A' and PropertyDeeds.RecordStatusInd = 'A' inner join Consumers on PropertyDeedBuyers.BuyerIDNO = Consumers.IDNO and IsESSynced = 0 and Consumers.RecordStatusInd = 1; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int deedcount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Deed - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " Deeds: " + deedcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["ConsumerID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(strTemp1fordtreader.ToString());
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("deedsindicator");
                                writer.WriteValue("1");
                                writer.WriteEndObject();
                            }
                            sb.Append("}");

                            deedcount++;
                            temp++;
                        }
                        rdrDataReader.Close();
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Deed - " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " Deeds Count: " + deedcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerPropertyDeedsMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;

                //Elastic Search - Consumers Directors
                if (processTable == "ES-ConsumerDirectorsMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerDirectorsMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select distinct ConsumerID from Directors inner join CommercialDirectors on Directors.DirectorID = CommercialDirectors.DirectorID and CommercialDirectors.DirectorStatusCode = 'A' and Directors.RecordStatusInd = 'A' and CommercialDirectors.RecordStatusInd = 'A' inner join Consumers on Directors.IDNo = Consumers.IDNO and Consumers.RecordStatusInd = 1; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int dircount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Directors - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " Direcotors: " + dircount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["ConsumerID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(strTemp1fordtreader.ToString());
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("directorindicator");
                                writer.WriteValue("1");
                                writer.WriteEndObject();
                            }
                            sb.Append("}");

                            dircount++;
                            temp++;
                        }
                        rdrDataReader.Close();
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Director - " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " Directors Count: " + dircount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerDirectorsMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;

                //Elastic Search - Consumers Adverse
                if (processTable == "ES-ConsumerAdverseMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerAdverseMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select distinct Consumers.ConsumerId from ConsumerJudgements inner join Consumers on ConsumerJudgements.ConsumerID = Consumers.ConsumerID and Consumers.RecordStatusInd = 1 and Rescinded is null and ConsumerJudgements.RecordStatusInd = 1 union select distinct Consumers.ConsumerID from ConsumerDebtReviews inner join Consumers on ConsumerDebtReviews.ConsumerID = Consumers.ConsumerID and Consumers.RecordStatusInd = 1 and ConsumerDebtReviews.RecordStatusInd = 1; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int advcount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Adv - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " Adverse Indicators: " + advcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["ConsumerID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(strTemp1fordtreader.ToString());
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("adverseindicator");
                                writer.WriteValue("1");
                                writer.WriteEndObject();
                            }
                            sb.Append("}");

                            advcount++;
                            temp++;
                        }
                        rdrDataReader.Close();
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Adv- " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " Adverse Count: " + advcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerAdverseMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();

                commercialStart = DateTime.Now;

                //Elastic Search - Commercials
                if (processTable == "ES-CommercialsMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-CommercialsMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select CommercialID,RegistrationNo,CommercialName,BusinessStartDate,CommercialStatusCode,LastUpdatedDate from Commercials where IsESSynced = 0 and RecordStatusInd = 'A' order by CommercialID; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        temp = 0;
                        startId = string.Empty; endId = string.Empty;


                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Commercials - " + startId + " - " + endId + ".txt");
                                }

                                commercialEnd = DateTime.Now;
                                CommercialTime = commercialEnd - commercialStart;
                                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["CommercialID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "").Replace("\"", "") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "").Replace("\"", "");

                            sb.Append("{\"index\" : { \"_index\" : \"commercialdata\", \"_type\" : \"commercialdata\", \"_id\" : \"");
                            sb.Append(Convert.ToString(strTemp1fordtreader));
                            sb.Append("\" } } \n");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("CommercialID");
                                writer.WriteValue(Convert.ToInt32(strTemp1fordtreader));
                                var number = rdrDataReader.GetValue(1);
                                writer.WritePropertyName("companyregnumber");
                                writer.WriteValue(number.ToString());
                                writer.WritePropertyName("companyregno");
                                writer.WriteValue(number.ToString().Replace("/", ""));
                                writer.WritePropertyName("companyname");
                                writer.WriteValue(rdrDataReader.GetValue(2));
                                writer.WritePropertyName("businessstartdate");
                                writer.WriteValue(rdrDataReader.GetValue(3));
                                writer.WritePropertyName("commercialstatuscode");
                                writer.WriteValue(rdrDataReader.GetValue(4));
                                writer.WritePropertyName("lastupdateddate");
                                writer.WriteValue(rdrDataReader.GetValue(5));
                                writer.WriteEndObject();
                            }
                            temp++;


                            i++;
                        }
                        rdrDataReader.Close();

                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Commercials - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-CommercialsMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                commercialEnd = DateTime.Now;
                CommercialTime = commercialEnd - commercialStart;
                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString());

                addressStart = DateTime.Now;
                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0) documentLst.Clear();

                //Elastic Search - Commercials Address
                if (processTable == "ES-CommercialsAddressMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-CommercialsAddressMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select  CommercialAddressID, CommercialAddresses.CommercialID,OriginalAddress1,OriginalAddress2,OriginalAddress3,OriginalAddress4,OriginalPostalCode from CommercialAddresses inner join Commercials on CommercialAddresses.CommercialID = Commercials.CommercialID and IsESSynced = 0  and Commercials.RecordStatusInd = 'A' order by CommercialID; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        string CommercialAddressID = string.Empty;
                        string Address = string.Empty;
                        startId = string.Empty; endId = string.Empty;
                        string CommercialId = string.Empty;
                        string CurrentCommercialId = string.Empty;
                        temp = 0;
                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "CommercialAddress - " + startId + " - " + endId + ".txt");
                                }
                                addressEnd = DateTime.Now;
                                addressTime = addressEnd - addressStart;
                                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());
                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;
                            }

                            CommercialAddressID = rdrDataReader["CommercialAddressID"].ToString();
                            startId = string.IsNullOrEmpty(startId) ? CommercialAddressID : startId;
                            endId = CommercialAddressID;

                            CurrentCommercialId = rdrDataReader["CommercialID"].ToString();
                            CommercialId = string.IsNullOrEmpty(CommercialId) ? CurrentCommercialId : CommercialId;

                            strTemp1fordtreader = rdrDataReader["OriginalAddress1"];
                            strTemp2fordtreader = rdrDataReader["OriginalAddress2"];
                            strTemp3fordtreader = rdrDataReader["OriginalAddress3"];
                            strTemp4fordtreader = rdrDataReader["OriginalAddress4"];
                            strTemp5fordtreader = rdrDataReader["OriginalPostalCode"];

                            Address = ((!string.IsNullOrEmpty(strTemp1fordtreader.ToString()) ? strTemp1fordtreader.ToString().Trim() + " " : "")
                                                                        + (!string.IsNullOrEmpty(strTemp2fordtreader.ToString()) ? strTemp2fordtreader.ToString().Trim() + " " : "")
                                                                        + (!string.IsNullOrEmpty(strTemp3fordtreader.ToString()) ? strTemp3fordtreader.ToString().Trim() + " " : "")
                                                                        + (!string.IsNullOrEmpty(strTemp4fordtreader.ToString()) ? strTemp4fordtreader.ToString().Trim() + " " : "")
                                                                        + (!string.IsNullOrEmpty(strTemp5fordtreader.ToString()) ? strTemp5fordtreader.ToString().Trim() + " " : ""));

                            if (CommercialId == CurrentCommercialId)
                                documentLst.Add(Address);
                            else
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(CommercialId);
                                sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                                sb.Append("{ \"doc\" :  ");

                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {

                                    writer.WriteStartObject();
                                    writer.WritePropertyName("commercialaddress");
                                    writer.WriteStartArray();
                                    foreach (string str in documentLst)
                                        writer.WriteValue(str.Trim());
                                    writer.WriteEnd();
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                                documentLst.Clear();
                                CommercialId = CurrentCommercialId;
                                documentLst.Add(Address);
                            }

                            temp++;


                            a++;
                        }
                        rdrDataReader.Close();
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(CommercialId);
                            sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {

                                writer.WriteStartObject();
                                writer.WritePropertyName("commercialaddress");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "CommercialAddress - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-CommercialsAddressMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                addressEnd = DateTime.Now;
                addressTime = addressEnd - addressStart;

                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Commercial Address Count: " + i.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());
                phoneStart = DateTime.Now;
                if (sb.Length > 0)
                    sb.Clear();

                if (documentLst.Count > 0)
                    documentLst.Clear();

                //Elastic Search - Commercials phone
                if (processTable == "ES-CommercialsPhoneMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-CommercialsPhoneMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select  CommercialTelephoneID, CommercialTelephones.CommercialID,TelephoneNo,TelephoneCode  from CommercialTelephones inner join Commercials on CommercialTelephones.CommercialID = Commercials.CommercialID and IsESSynced = 0 and Commercials.RecordStatusInd = 'A'  order by CommercialID;";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        string PhoneNumber = string.Empty;
                        string CommercialTelephoneID = string.Empty;
                        string TelephoneCode = string.Empty;
                        bool add = false;
                        temp = 0;
                        startId = string.Empty; endId = string.Empty;
                        string CommercialId = string.Empty;
                        string CurrentCommercialId = string.Empty;
                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "CommercialPhone - " + startId + " - " + endId + ".txt");
                                }

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;

                                phoneEnd = DateTime.Now;
                                phoneTime = phoneEnd - phoneStart;
                                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());

                            }

                            PhoneNumber = string.Empty;
                            CommercialTelephoneID = string.Empty;
                            add = false;

                            CommercialTelephoneID = rdrDataReader["CommercialTelephoneID"].ToString();
                            CurrentCommercialId = rdrDataReader["CommercialID"].ToString();
                            CommercialId = string.IsNullOrEmpty(CommercialId) ? CurrentCommercialId : CommercialId;
                            PhoneNumber = rdrDataReader["TelephoneNo"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            TelephoneCode = rdrDataReader["TelephoneCode"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            PhoneNumber = TelephoneCode + PhoneNumber;
                            if (PhoneNumber.Length == 10)
                                add = true;

                            if (add)
                            {
                                if (CommercialId == CurrentCommercialId)
                                {
                                    documentLst.Add(PhoneNumber);
                                }
                                else
                                {
                                    sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                    sb.Append("{ \"update\" : {\"_id\" : \"");
                                    sb.Append(CommercialId);
                                    sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                                    sb.Append("{ \"doc\" :  ");

                                    sw = new StringWriter(sb);
                                    using (JsonWriter writer = new JsonTextWriter(sw))
                                    {

                                        writer.WriteStartObject();
                                        writer.WritePropertyName("commercialtelephone");
                                        writer.WriteStartArray();
                                        foreach (string str in documentLst)
                                            writer.WriteValue(str.Trim());
                                        writer.WriteEnd();
                                        writer.WriteEndObject();
                                    }
                                    sb.Append("}");
                                    documentLst.Clear();
                                    CommercialId = CurrentCommercialId;
                                    documentLst.Add(PhoneNumber);
                                }
                            }
                            temp++;


                            p++;
                        }
                        rdrDataReader.Close();
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(CommercialId);
                            sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {

                                writer.WriteStartObject();
                                writer.WritePropertyName("commercialtelephone");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "CommercialPhone - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }


                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-CommercialsPhoneMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                phoneEnd = DateTime.Now;
                phoneTime = phoneEnd - phoneStart;
                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());

                addressSearchstart = DateTime.Now;
                if (sb.Length > 0)
                    sb.Clear();

                if (documentLst.Count > 0)
                    documentLst.Clear();

                //Elastic search - address for address search
                if (processTable == "ES-AddressMigrationForAddressSearch" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-AddressMigrationForAddressSearch";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select ConsumerAddressID,ConsumerID,OriginalAddress1,OriginalAddress2,OriginalAddress3,OriginalAddress4,OriginalPostalCode,Town,Region,Province from ConsumerAddresses where RecordStatusInd = 1;";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        string Address = string.Empty;
                        string ConsumerAddressID = string.Empty;
                        bool add = false;
                        temp = 0; i = 0;
                        startId = string.Empty; endId = string.Empty;
                        string ConsumerId = string.Empty;
                        string CurrentConsumerId = string.Empty;
                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "AddressForAddressSearch - " + startId + " - " + endId + ".txt");
                                }

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;

                                addressSearchEnd = DateTime.Now;
                                addressSearchTime = addressSearchEnd - addressSearchstart;
                                ShowTaskProgress(100, "Consumer Address Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());

                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            ConsumerAddressID = rdrDataReader["ConsumerAddressID"].ToString();
                            startId = string.IsNullOrEmpty(startId) ? ConsumerAddressID : startId;
                            endId = ConsumerAddressID;

                            sb.Append("{\"index\" : { \"_index\" : \"consumeraddressinformation\", \"_type\" : \"consumeraddressinformation\", \"_id\" : \"");
                            sb.Append(ConsumerAddressID);
                            sb.Append("\" } } \n");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("consumeraddressid");
                                writer.WriteValue(ConsumerAddressID);
                                writer.WritePropertyName("consumerid");
                                writer.WriteValue(rdrDataReader["ConsumerID"].ToString());
                                writer.WritePropertyName("address");
                                strTemp1fordtreader = rdrDataReader["OriginalAddress1"];
                                strTemp2fordtreader = rdrDataReader["OriginalAddress2"];
                                strTemp3fordtreader = rdrDataReader["OriginalAddress3"];
                                strTemp4fordtreader = rdrDataReader["OriginalAddress4"];
                                strTemp5fordtreader = rdrDataReader["OriginalPostalCode"];
                                strTemp6fordtreader = rdrDataReader["Province"];
                                strTemp7fordtreader = rdrDataReader["Town"];
                                strTemp8fordtreader = rdrDataReader["Region"];

                                Address = ((!string.IsNullOrEmpty(strTemp1fordtreader.ToString()) ? strTemp1fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp2fordtreader.ToString()) ? strTemp2fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp3fordtreader.ToString()) ? strTemp3fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp4fordtreader.ToString()) ? strTemp4fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp5fordtreader.ToString()) ? strTemp5fordtreader.ToString().Trim() + " " : "")
                                                                               + (!string.IsNullOrEmpty(strTemp6fordtreader.ToString()) ? strTemp6fordtreader.ToString().Trim() + " " : "")
                                                                                  + (!string.IsNullOrEmpty(strTemp7fordtreader.ToString()) ? strTemp7fordtreader.ToString().Trim() + " " : "")
                                                                                     + (!string.IsNullOrEmpty(strTemp8fordtreader.ToString()) ? strTemp8fordtreader.ToString().Trim() + " " : "")
                                                                            );


                                writer.WriteValue(Address);
                                writer.WriteEndObject();
                            }

                            temp++;
                            i++;
                        }


                        rdrDataReader.Close();
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"index\" : {\"_id\" : \"");
                            sb.Append(ConsumerId);
                            sb.Append("\", \"_type\" : \"consumeraddressinformation\", \"_index\" : \"consumeraddressinformation\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {

                                writer.WriteStartObject();
                                writer.WritePropertyName("consumeraddressid");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "AddressForAddressSearch - " + startId + " - " + endId + ".txt");
                            }
                        }

                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-AddressMigrationForAddressSearch";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

                addressSearchEnd = DateTime.Now;
                addressSearchTime = addressSearchEnd - addressSearchstart;
                TimeSpan TotalTime = addressSearchEnd - commercialStart;
                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString() + " Total Time: " + Math.Round(TotalTime.TotalMinutes, 2).ToString());

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;

                //Elastic Search - Consumers Employment
                if (processTable == "ES-ConsumerEmploymentMigration" || processTable == string.Empty)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = objConnection;
                    cmd.CommandType = CommandType.Text;
                    if (objConnection.State != ConnectionState.Open)
                        objConnection.Open();
                    cmd.CommandTimeout = 0;
                    dr = dt.NewRow();

                    dr["Table"] = "ES-ConsumerEmploymentMigration";
                    dr["Start Time"] = dtStartTime;
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    cmd.CommandText = "select distinct Consumers.ConsumerID from ConsumerEmployments inner join Consumers on ConsumerEmployments.ConsumerID = Consumers.ConsumerID and Consumers.RecordStatusInd = 1 and ConsumerEmployments.RecordStatusInd = 1 ; ";
                    dtStartTime = DateTime.Now;

                    rdrDataReader = cmd.ExecuteReader();

                    if (rdrDataReader.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int advcount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrDataReader.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Employment - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " Employment Indicators: " + advcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }

                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrDataReader["ConsumerID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(strTemp1fordtreader.ToString());
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("employmentindicator");
                                writer.WriteValue("1");
                                writer.WriteEndObject();
                            }
                            sb.Append("}");

                            advcount++;
                            temp++;
                        }
                        rdrDataReader.Close();
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Employment- " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " Employment Count: " + advcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }

                    dtEndTime = DateTime.Now;
                    TimeTake = dtEndTime - dtStartTime;
                    dt.Rows.RemoveAt(dt.Rows.Count - 1);

                    dr["Table"] = "ES-ConsumerEmploymentMigration";
                    dr["Start Time"] = dtStartTime;
                    dr["End Time"] = dtEndTime;
                    dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                    dt.Rows.Add(dr);
                    Log_DataGridView.DataSource = dt;
                    if (!chkbxProcessSelectedTable.Checked) processTable = string.Empty;
                }

            }
            catch (Exception ex)
            {
                ShowTaskProgress(0, ex.Message);
                throw ex;

            }
            finally
            {


            }
        }

        private async Task InsertCustomerinfromation(string elasticIP)
        {
            string Id = string.Empty;
            try
            {
                ESService es = new ESService(elasticIP);

                DataTable dt = (DataTable)Log_DataGridView.DataSource;
                DateTime dtStartTime = DateTime.Now;
                ShowTaskProgress(0, "Processing 'Consumer' Table to ElasticDB - 0%");
                int i = 0;
                int InsertESData = Convert.ToInt32(ConfigurationManager.AppSettings["TakeCount"]); ;

                SqlConnection objConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ProdDBConnection"].ConnectionString);
                SqlCommand objConsumerCommand = new SqlCommand();

                objConsumerCommand.CommandText = "select ConsumerID,PassportNo,FirstName,Surname,IDNO,GenderInd,BirthDate,Alloy,LSM from Consumers where RecordStatusInd = 1 and IsESSynced = 0 and (DATEDIFF(Year,BirthDate,getdate()) >=18 or BirthDate is null) order by consumerid; select ConsumerAddressID, ConsumerAddresses.ConsumerID,OriginalAddress1,OriginalAddress2,OriginalAddress3,OriginalAddress4, OriginalPostalCode,Province,Town,Region,AddressTypeInd,ConsumerAddresses.CreatedOnDate from ConsumerAddresses inner join Consumers on ConsumerAddresses.ConsumerID = Consumers.ConsumerID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 and ConsumerAddresses.RecordStatusInd = 1 order by ConsumerID; select ConsumerTelephoneID, ConsumerTelephones.ConsumerID,TelephoneNo,InternationalDialingCode from ConsumerTelephones inner join Consumers on ConsumerTelephones.ConsumerID = Consumers.ConsumerID and IsESSynced = 0  and Consumers.RecordStatusInd = 1 and ConsumerTelephones.RecordStatusInd = 1 inner join Telephones on ConsumerTelephones.TelephoneID = Telephones.TelephoneID and Telephones.RecordStatusInd =1 order by ConsumerID; select ID, ConsumerEmails.ConsumerID,EmailID from ConsumerEmails inner join Consumers on ConsumerEmails.ConsumerID = Consumers.ConsumerID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 and ConsumerEmails.RecordStatusInd = 1 order by ConsumerID; select HomeAffairsID, ConsumerHomeAffairs.ConsumerID ,MarriageDate,DivorceDate,ConsumerHomeAffairs.DeceasedStatus from ConsumerHomeAffairs inner join Consumers on ConsumerHomeAffairs.ConsumerID = Consumers.ConsumerID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 and ConsumerHomeAffairs.RecordStatusInd = 'A' order by ConsumerID; select LSM.ConsumerID,RiskCategory,IncomeCategory from LSM inner join Consumers on Consumers.ConsumerID = LSM.CONSUMERID and IsESSynced = 0 and Consumers.RecordStatusInd = 1 order by ConsumerID; select distinct Consumers.ConsumerID from PropertyDeedBuyers inner join PropertyDeeds on PropertyDeedBuyers.PropertyDeedId = PropertyDeeds.PropertyDeedID and IsCurrentOwner = 1 and PropertyDeedBuyers.RecordStatusInd = 'A' and PropertyDeeds.RecordStatusInd = 'A' inner join Consumers on PropertyDeedBuyers.BuyerIDNO = Consumers.IDNO and IsESSynced = 0 and Consumers.RecordStatusInd = 1; select distinct ConsumerID from Directors inner join CommercialDirectors on Directors.DirectorID = CommercialDirectors.DirectorID and CommercialDirectors.DirectorStatusCode = 'A' and Directors.RecordStatusInd = 'A' and CommercialDirectors.RecordStatusInd = 'A' inner join Consumers on Directors.IDNo = Consumers.IDNO and Consumers.RecordStatusInd = 1; select distinct Consumers.ConsumerId from ConsumerJudgements inner join Consumers on ConsumerJudgements.ConsumerID = Consumers.ConsumerID and Consumers.RecordStatusInd = 1 and Rescinded is null and ConsumerJudgements.RecordStatusInd = 1 union select distinct Consumers.ConsumerID from ConsumerDebtReviews inner join Consumers on ConsumerDebtReviews.ConsumerID = Consumers.ConsumerID and Consumers.RecordStatusInd = 1 and ConsumerDebtReviews.RecordStatusInd = 1; ";
                objConsumerCommand.CommandType = CommandType.Text;
                objConsumerCommand.Connection = objConnection;
                objConsumerCommand.CommandTimeout = 0;

                if (objConnection.State != ConnectionState.Open)
                    objConnection.Open();

                SqlDataReader rdrConsumer = objConsumerCommand.ExecuteReader(CommandBehavior.SequentialAccess);

                int e = 0, p = 0, a = 0, temp = 0;

                object strTemp1fordtreader;
                object strTemp2fordtreader;
                object strTemp3fordtreader;
                object strTemp4fordtreader;
                object strTemp5fordtreader;
                object strTemp6fordtreader;
                object strTemp7fordtreader;
                object strTemp8fordtreader;
                object strTemp9fordtreader;
                object strTemp10fordtreader;

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                string startId = string.Empty;
                string endId = string.Empty;
                StringResponse stringResponse;

                //Variables for Time tracking.

                DateTime consumerStart = DateTime.Now;
                DateTime consumerEnd = DateTime.Now;
                TimeSpan ConsumerTime = consumerEnd - consumerStart;
                DateTime addressStart = DateTime.Now;
                DateTime addressEnd = DateTime.Now;
                TimeSpan addressTime = addressEnd - addressStart;
                DateTime phoneStart = DateTime.Now;
                DateTime phoneEnd = DateTime.Now;
                TimeSpan phoneTime = phoneEnd - phoneStart;
                DateTime emailStart = DateTime.Now;
                DateTime emailEnd = DateTime.Now;
                TimeSpan emailTime = emailEnd - emailStart;
                DateTime homeStart = DateTime.Now;
                DateTime homeEnd = DateTime.Now;
                TimeSpan homeTime = emailEnd - emailStart;
                DateTime lsmstart = DateTime.Now;
                DateTime lsmEnd = DateTime.Now;
                TimeSpan lsmTime = lsmEnd - lsmstart;
                DataRow dr;

                consumerStart = DateTime.Now;
                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;
                //Consumers Information
                if (rdrConsumer.HasRows)
                {
                    temp = 0;
                    startId = string.Empty; endId = string.Empty;


                    while (rdrConsumer.Read())
                    {
                        if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                        {
                            sb.Append("\n");

                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Consumer - " + startId + " - " + endId + ".txt");
                            }

                            consumerEnd = DateTime.Now;
                            ConsumerTime = consumerEnd - consumerStart;
                            ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString());

                            sb.Clear();
                            temp = 0;
                            startId = string.Empty;
                        }
                        else
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrConsumer["ConsumerID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                            //strInsertString.Append("{\"index\" : { \"_index\" : \"consumersinformation\", \"_type\" : \"consumersinformation\", \"_id\" : \"" + strTemp1fordtreader.ToString() + "\" } } \n");
                            sb.Append("{\"index\" : { \"_index\" : \"consumerindex\", \"_type\" : \"consumerindex\", \"_id\" : \"");
                            sb.Append(Convert.ToString(strTemp1fordtreader));
                            sb.Append("\" } } \n");
                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("consumerid");
                                writer.WriteValue(Convert.ToString(strTemp1fordtreader));
                                writer.WritePropertyName("passportno");
                                writer.WriteValue(rdrConsumer.GetValue(1));
                                writer.WritePropertyName("firstname");
                                writer.WriteValue(rdrConsumer.GetValue(2));
                                writer.WritePropertyName("surname");
                                writer.WriteValue(rdrConsumer.GetValue(3));
                                writer.WritePropertyName("idnumber");
                                writer.WriteValue(rdrConsumer.GetValue(4));
                                writer.WritePropertyName("gender");
                                writer.WriteValue(rdrConsumer.GetValue(5));
                                writer.WritePropertyName("dateofbirth");
                                writer.WriteValue(rdrConsumer.GetValue(6));
                                writer.WritePropertyName("alloy");
                                writer.WriteValue(rdrConsumer.GetValue(7));
                                writer.WritePropertyName("lsm");
                                writer.WriteValue(rdrConsumer.GetValue(8));
                                writer.WritePropertyName("emailindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("deedsindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("adverseindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("directorindicator");
                                writer.WriteValue("0");
                                writer.WritePropertyName("maritalstatus");
                                writer.WriteValue("Unknown");
                                writer.WritePropertyName("incomercategoty");
                                writer.WriteValue("Unknown");
                                writer.WritePropertyName("riskcategory");
                                writer.WriteValue("Unknown");
                                writer.WritePropertyName("province");
                                writer.WriteValue("Unknown");
                                writer.WriteEndObject();
                            }
                            temp++;
                        }
                        i++;
                    }
                    if (sb.Length != 0)
                    {
                        sb.Append("\n");
                        stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                        if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                        {
                            string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                            WriteToFile(str, "Consumer - " + startId + " - " + endId + ".txt");
                        }
                    }
                }

                consumerEnd = DateTime.Now;
                ConsumerTime = consumerEnd - consumerStart;
                DateTime dtEndTime;
                TimeSpan TimeTake;
                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString());

                if (sb.Length > 0)
                    sb.Clear();

                addressStart = DateTime.Now;
                List<string> documentLst = new List<string>();
                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Address Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;
                //Address Information
                if (rdrConsumer.NextResult())
                {
                    if (rdrConsumer.HasRows)
                    {
                        string ConsumerAddressID = string.Empty;
                        string ConsumerId = string.Empty;
                        string CurrentConsumerId = string.Empty;
                        string Address = "Unknown";
                        string Town = "Unknown";
                        string Region = "Unknown";
                        DateTime CreatedOnDate = DateTime.MinValue;
                        DateTime tmpDate = DateTime.MinValue;
                        startId = string.Empty; endId = string.Empty;
                        temp = 0;
                        string Province = string.Empty;

                        while (rdrConsumer.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "ConsumerAddress - " + startId + " - " + endId + ".txt");
                                }
                                addressEnd = DateTime.Now;
                                addressTime = addressEnd - addressStart;
                                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());
                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;
                            }
                            else
                            {
                                ConsumerAddressID = rdrConsumer["ConsumerAddressID"].ToString();
                                startId = string.IsNullOrEmpty(startId) ? ConsumerAddressID : startId;
                                endId = ConsumerAddressID;

                                CurrentConsumerId = rdrConsumer["ConsumerID"].ToString();

                                ConsumerId = string.IsNullOrEmpty(ConsumerId) ? CurrentConsumerId : ConsumerId;


                                strTemp1fordtreader = rdrConsumer["OriginalAddress1"];
                                strTemp2fordtreader = rdrConsumer["OriginalAddress2"];
                                strTemp3fordtreader = rdrConsumer["OriginalAddress3"];
                                strTemp4fordtreader = rdrConsumer["OriginalAddress4"];
                                strTemp5fordtreader = rdrConsumer["OriginalPostalCode"];
                                strTemp6fordtreader = rdrConsumer["Province"];
                                strTemp7fordtreader = rdrConsumer["Town"];
                                strTemp8fordtreader = rdrConsumer["Region"];
                                strTemp9fordtreader = rdrConsumer["AddressTypeInd"];
                                strTemp10fordtreader = rdrConsumer["CreatedOnDate"];
                                CreatedOnDate = strTemp10fordtreader == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(strTemp10fordtreader);


                                Address = ((!string.IsNullOrEmpty(strTemp1fordtreader.ToString()) ? strTemp1fordtreader.ToString().Trim() + " " : "")
                                                                                 + (!string.IsNullOrEmpty(strTemp2fordtreader.ToString()) ? strTemp2fordtreader.ToString().Trim() + " " : "")
                                                                                 + (!string.IsNullOrEmpty(strTemp3fordtreader.ToString()) ? strTemp3fordtreader.ToString().Trim() + " " : "")
                                                                                 + (!string.IsNullOrEmpty(strTemp4fordtreader.ToString()) ? strTemp4fordtreader.ToString().Trim() + " " : "")
                                                                                 + (!string.IsNullOrEmpty(strTemp5fordtreader.ToString()) ? strTemp5fordtreader.ToString().Trim() + " " : "")
                                                                                 + (!string.IsNullOrEmpty(strTemp6fordtreader.ToString()) ? strTemp6fordtreader.ToString().Trim() + " " : ""));

                                if (ConsumerId == CurrentConsumerId)
                                {
                                    documentLst.Add(Address);
                                    if (strTemp9fordtreader.ToString() == "1" && CreatedOnDate >= tmpDate)
                                    {

                                        Province = strTemp6fordtreader == DBNull.Value ? "Unknown" : strTemp6fordtreader.ToString();
                                        Town = strTemp7fordtreader == DBNull.Value ? "Unknown" : strTemp7fordtreader.ToString();
                                        Region = strTemp8fordtreader == DBNull.Value ? "Unknown" : strTemp8fordtreader.ToString();
                                        tmpDate = CreatedOnDate;
                                    }

                                }
                                else if (ConsumerId != CurrentConsumerId)
                                {



                                    sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                    sb.Append("{ \"update\" : {\"_id\" : \"");
                                    sb.Append(ConsumerId);
                                    sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                    sb.Append("{ \"doc\" :  ");

                                    sw = new StringWriter(sb);
                                    using (JsonWriter writer = new JsonTextWriter(sw))
                                    {

                                        writer.WriteStartObject();
                                        writer.WritePropertyName("province");
                                        writer.WriteValue(Province);
                                        writer.WritePropertyName("town");
                                        writer.WriteValue(Town);
                                        writer.WritePropertyName("region");
                                        writer.WriteValue(Region);
                                        writer.WritePropertyName("address");
                                        writer.WriteStartArray();
                                        foreach (string str in documentLst)
                                            writer.WriteValue(str.Trim());
                                        writer.WriteEnd();
                                        writer.WriteEndObject();
                                    }


                                    sb.Append("}");
                                    documentLst.Clear();
                                    ConsumerId = CurrentConsumerId;
                                    documentLst.Add(Address);
                                    Address = "Unknown";
                                    Town = "Unknown";
                                    Region = "Unknown";
                                    tmpDate = DateTime.MinValue;
                                    if (strTemp9fordtreader.ToString() == "1" && CreatedOnDate >= tmpDate)
                                    {

                                        Province = strTemp6fordtreader == DBNull.Value ? "Unknown" : strTemp6fordtreader.ToString();
                                        Town = strTemp7fordtreader == DBNull.Value ? "Unknown" : strTemp7fordtreader.ToString();
                                        Region = strTemp8fordtreader == DBNull.Value ? "Unknown" : strTemp8fordtreader.ToString();
                                        tmpDate = CreatedOnDate;
                                    }

                                }

                                temp++;
                            }
                            a++;
                        }
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(ConsumerId);
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {

                                writer.WriteStartObject();
                                writer.WritePropertyName("province");
                                writer.WriteValue(Province);
                                writer.WritePropertyName("town");
                                writer.WriteValue(Town);
                                writer.WritePropertyName("region");
                                writer.WriteValue(Region);
                                writer.WritePropertyName("address");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                            Province = string.Empty;
                            Town = string.Empty;
                            Region = string.Empty;
                            tmpDate = DateTime.MinValue;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerAddress - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }
                }
                addressEnd = DateTime.Now;
                addressTime = addressEnd - addressStart;

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Address Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());

                if (sb.Length > 0)
                    sb.Clear();

                if (documentLst.Count > 0)
                    documentLst.Clear();

                phoneStart = DateTime.Now;

                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Phone Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                //Phones Information
                if (rdrConsumer.NextResult())
                {
                    if (rdrConsumer.HasRows)
                    {
                        string PhoneNumber = string.Empty;
                        string ConsumerId = string.Empty;
                        string CurrentConsumerId = string.Empty;
                        string ConsumerTelephoneID = string.Empty;
                        string InternationalDialingCode = string.Empty;
                        bool add = false;
                        temp = 0;
                        startId = string.Empty; endId = string.Empty;

                        while (rdrConsumer.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "ConsumerPhones - " + startId + " - " + endId + ".txt");
                                }

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;

                                phoneEnd = DateTime.Now;
                                phoneTime = phoneEnd - phoneStart;
                                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());

                            }
                            else
                            {
                                PhoneNumber = string.Empty;
                                ConsumerTelephoneID = string.Empty;
                                InternationalDialingCode = string.Empty;
                                add = false;

                                ConsumerTelephoneID = rdrConsumer["ConsumerTelephoneID"].ToString();
                                CurrentConsumerId = rdrConsumer["ConsumerID"].ToString();
                                ConsumerId = string.IsNullOrEmpty(ConsumerId) ? CurrentConsumerId : ConsumerId;
                                PhoneNumber = rdrConsumer["TelephoneNo"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                                InternationalDialingCode = rdrConsumer["InternationalDialingCode"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                                if (InternationalDialingCode == "27")
                                {
                                    PhoneNumber = "0" + PhoneNumber;
                                    if (PhoneNumber.Length == 10)
                                        add = true;
                                }
                                else
                                {
                                    PhoneNumber = InternationalDialingCode + PhoneNumber;
                                    add = true;
                                }

                                if (add)
                                {
                                    if (ConsumerId == CurrentConsumerId)
                                    {
                                        documentLst.Add(PhoneNumber);
                                    }
                                    else if (ConsumerId != CurrentConsumerId)
                                    {
                                        sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                        sb.Append("{ \"update\" : {\"_id\" : \"");
                                        sb.Append(ConsumerId);
                                        sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                        sb.Append("{ \"doc\" :  ");

                                        sw = new StringWriter(sb);
                                        using (JsonWriter writer = new JsonTextWriter(sw))
                                        {

                                            writer.WriteStartObject();
                                            writer.WritePropertyName("phonenumber");
                                            writer.WriteStartArray();
                                            foreach (string str in documentLst)
                                                writer.WriteValue(str.Trim());
                                            writer.WriteEnd();
                                            writer.WriteEndObject();
                                        }
                                        sb.Append("}");
                                        documentLst.Clear();
                                        ConsumerId = CurrentConsumerId;
                                        documentLst.Add(PhoneNumber);
                                    }
                                }
                                temp++;
                            }
                            p++;
                        }
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(ConsumerId);
                            sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("phonenumber");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerPhones - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }
                }

                phoneEnd = DateTime.Now;
                phoneTime = phoneEnd - phoneStart;
                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());
                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Phone Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;
                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                emailStart = DateTime.Now;

                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Emails Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                //EmailsInfromation
                if (rdrConsumer.NextResult())
                {
                    temp = 0;
                    startId = string.Empty; endId = string.Empty;
                    string ConsumerId = string.Empty;
                    string CurrentConsumerId = string.Empty;
                    string email = string.Empty;
                    while (rdrConsumer.Read())
                    {
                        if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerEmails - " + startId + " - " + endId + ".txt");
                            }
                            sb.Clear();
                            temp = 0;
                            startId = string.Empty; endId = string.Empty;

                            emailEnd = DateTime.Now;
                            emailTime = emailEnd - emailStart;
                            ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString() + " Email Count: " + e.ToString() + " Time Taken: " + Math.Round(emailTime.TotalMinutes, 2).ToString());
                        }
                        else
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrConsumer["ID"];
                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                            CurrentConsumerId = rdrConsumer["ConsumerID"].ToString();
                            ConsumerId = string.IsNullOrEmpty(ConsumerId) ? CurrentConsumerId : ConsumerId;

                            email = rdrConsumer["EmailID"].ToString();

                            if (ConsumerId == CurrentConsumerId)
                            {
                                documentLst.Add(email);
                            }
                            else if (ConsumerId != CurrentConsumerId)
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(ConsumerId);
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");

                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {

                                    writer.WriteStartObject();
                                    writer.WritePropertyName("emailindicator");
                                    writer.WriteValue("1");
                                    writer.WritePropertyName("email");
                                    writer.WriteStartArray();
                                    foreach (string str in documentLst)
                                        writer.WriteValue(str.Trim());
                                    writer.WriteEnd();
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                                documentLst.Clear();
                                ConsumerId = CurrentConsumerId;
                                documentLst.Add(email);
                            }
                            temp++;
                        }
                        e += 1;
                    }
                    if (documentLst.Count > 0)
                    {
                        sb = sb.Length != 0 ? sb.Append("\n") : sb;

                        sb.Append("{ \"update\" : {\"_id\" : \"");
                        sb.Append(ConsumerId);
                        sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                        sb.Append("{ \"doc\" :  ");

                        sw = new StringWriter(sb);
                        using (JsonWriter writer = new JsonTextWriter(sw))
                        {
                            writer.WriteStartObject();
                            writer.WritePropertyName("emailindicator");
                            writer.WriteValue("1");
                            writer.WritePropertyName("email");
                            writer.WriteStartArray();
                            foreach (string str in documentLst)
                                writer.WriteValue(str.Trim());
                            writer.WriteEnd();
                            writer.WriteEndObject();
                        }
                        sb.Append("}");
                        documentLst.Clear();
                    }
                    if (sb.Length != 0)
                    {
                        sb.Append("\n");
                        stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                        if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                        {
                            string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                            WriteToFile(str, "ConsumerEmails - " + startId + " - " + endId + ".txt");
                        }
                    }
                }

                emailEnd = DateTime.Now;
                emailTime = emailEnd - emailStart;
                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString() + " Email Count: " + e.ToString() + " Time Taken: " + Math.Round(emailTime.TotalMinutes, 2).ToString());

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Emails Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                homeStart = DateTime.Now;

                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Home Affairs Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                //HomeAffires
                if (rdrConsumer.NextResult())
                {

                    if (rdrConsumer.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int HomeCount = 0;
                        temp = 0;
                        homeStart = DateTime.Now;

                        while (rdrConsumer.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "ConsumerHomeAffires - " + startId + " - " + endId + ".txt");
                                }

                                homeEnd = DateTime.Now;
                                homeTime = homeEnd - homeStart;
                                ShowTaskProgress(100, " Home Affairs Count: " + HomeCount.ToString() + " Time Taken: " + Math.Round(homeTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }
                            else
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;
                                strTemp1fordtreader = rdrConsumer["HomeAffairsID"];

                                startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                                endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                                //strInsertString.Append("{\"index\" : { \"_index\" : \"consumersinformation\", \"_type\" : \"consumersinformation\", \"_id\" : \"" + strTemp1fordtreader.ToString() + "\" } } \n");
                                strTemp2fordtreader = rdrConsumer.GetValue(1);
                                strTemp3fordtreader = rdrConsumer.GetValue(2);
                                strTemp4fordtreader = rdrConsumer.GetValue(3);
                                strTemp5fordtreader = rdrConsumer.GetValue(4);

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(strTemp2fordtreader.ToString());
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");
                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("maritalstatus");
                                    writer.WriteValue(strTemp3fordtreader == DBNull.Value ? "Single" : strTemp4fordtreader == DBNull.Value ? "Married" : Convert.ToDateTime(strTemp4fordtreader) > Convert.ToDateTime(strTemp3fordtreader) ? "Divorced" : "Married");
                                    writer.WritePropertyName("isdeceased");
                                    writer.WriteValue(strTemp5fordtreader == DBNull.Value ? false : strTemp5fordtreader.ToString().ToUpper() == "Deceased".ToUpper() ? true : false);
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                            }
                            HomeCount++;
                            temp++;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "ConsumerHomeAffires - " + startId + " - " + endId + ".txt");
                            }
                            homeEnd = DateTime.Now;
                            homeTime = homeEnd - homeStart;
                            ShowTaskProgress(100, " Home Affairs Count: " + HomeCount.ToString() + " Time Taken: " + Math.Round(homeTime.TotalMinutes, 2).ToString());

                        }
                    }
                }

                homeEnd = DateTime.Now;
                TimeSpan TotalTime = homeEnd - consumerStart;
                ShowTaskProgress(100, "Consumers Count: " + i.ToString() + " Time Taken: " + Math.Round(ConsumerTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString() + " Email Count: " + e.ToString() + " Time Taken: " + Math.Round(emailTime.TotalMinutes, 2).ToString() + " Total Time : " + Math.Round(TotalTime.TotalMinutes, 2).ToString());

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Home Affairs Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;

                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Risk and Income Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                //LSM
                if (rdrConsumer.NextResult())
                {

                    if (rdrConsumer.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int lsmcount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrConsumer.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "LSM - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " LSM: " + lsmcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }
                            else
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;
                                strTemp1fordtreader = rdrConsumer["ConsumerID"];

                                startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                                endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                                //strInsertString.Append("{\"index\" : { \"_index\" : \"consumersinformation\", \"_type\" : \"consumersinformation\", \"_id\" : \"" + strTemp1fordtreader.ToString() + "\" } } \n");
                                strTemp2fordtreader = rdrConsumer.GetValue(1);
                                strTemp3fordtreader = rdrConsumer.GetValue(2);

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(strTemp1fordtreader.ToString());
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");
                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {
                                    writer.WriteStartObject();

                                    writer.WritePropertyName("riskcategory");
                                    writer.WriteValue(strTemp2fordtreader == DBNull.Value ? "Unknown" : strTemp2fordtreader.ToString());
                                    writer.WritePropertyName("incomercategoty");
                                    writer.WriteValue(strTemp3fordtreader == DBNull.Value ? "Unknown" : strTemp3fordtreader.ToString());
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                            }
                            lsmcount++;
                            temp++;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "LSM - " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " LSM Count: " + lsmcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }
                }

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Risk and Income Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;
                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Deeds Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;
                //Property Deeds
                if (rdrConsumer.NextResult())
                {

                    if (rdrConsumer.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int deedcount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrConsumer.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Deed - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " Deeds: " + deedcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }
                            else
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;
                                strTemp1fordtreader = rdrConsumer["ConsumerID"];

                                startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                                endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(strTemp1fordtreader.ToString());
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");
                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {
                                    writer.WriteStartObject();
                                    writer.WritePropertyName("deedsindicator");
                                    writer.WriteValue("1");
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                            }
                            deedcount++;
                            temp++;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Deed - " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " Deeds Count: " + deedcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }
                }

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Deeds Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;
                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Directors Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                //Directors
                if (rdrConsumer.NextResult())
                {

                    if (rdrConsumer.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int dircount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrConsumer.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Directors - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " Direcotors: " + dircount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }
                            else
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;
                                strTemp1fordtreader = rdrConsumer["ConsumerID"];

                                startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                                endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(strTemp1fordtreader.ToString());
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");
                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {
                                    writer.WriteStartObject();
                                    writer.WritePropertyName("directorindicator");
                                    writer.WriteValue("1");
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                            }
                            dircount++;
                            temp++;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Director - " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " Directors Count: " + dircount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }
                }

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Directors Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0)
                    documentLst.Clear();
                lsmstart = DateTime.Now;
                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Consumer Adverse Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                //Adverse
                if (rdrConsumer.NextResult())
                {

                    if (rdrConsumer.HasRows)
                    {
                        startId = string.Empty; endId = string.Empty;
                        int advcount = 0;
                        temp = 0;
                        lsmstart = DateTime.Now;

                        while (rdrConsumer.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");

                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "Adv - " + startId + " - " + endId + ".txt");
                                }

                                lsmEnd = DateTime.Now;
                                lsmTime = lsmEnd - lsmstart;
                                ShowTaskProgress(100, " Adverse Indicators: " + advcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty;
                            }
                            else
                            {
                                sb = sb.Length != 0 ? sb.Append("\n") : sb;
                                strTemp1fordtreader = rdrConsumer["ConsumerID"];

                                startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"") : startId;
                                endId = strTemp1fordtreader.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");

                                sb.Append("{ \"update\" : {\"_id\" : \"");
                                sb.Append(strTemp1fordtreader.ToString());
                                sb.Append("\", \"_type\" : \"consumerindex\", \"_index\" : \"consumerindex\"} } \n");
                                sb.Append("{ \"doc\" :  ");
                                sw = new StringWriter(sb);
                                using (JsonWriter writer = new JsonTextWriter(sw))
                                {
                                    writer.WriteStartObject();
                                    writer.WritePropertyName("adverseindicator");
                                    writer.WriteValue("1");
                                    writer.WriteEndObject();
                                }
                                sb.Append("}");
                            }
                            advcount++;
                            temp++;
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Adv- " + startId + " - " + endId + ".txt");
                            }
                            lsmEnd = DateTime.Now;
                            lsmTime = lsmEnd - lsmstart;
                            ShowTaskProgress(100, " Adverse Count: " + advcount.ToString() + " Time Taken: " + Math.Round(lsmTime.TotalMinutes, 2).ToString());

                        }
                    }
                }

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Consumer Directors Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

            }
            catch (Exception ex)
            {
                ShowTaskProgress(0, ex.Message);
                throw ex;
            }
        }

        public async Task InsertCommercialinformmation(string elasticIP)
        {
            string Id = string.Empty;
            try
            {
                ESService es = new ESService(elasticIP);

                DataTable dt = (DataTable)Log_DataGridView.DataSource;
                DateTime dtStartTime = DateTime.Now;
                DateTime dtEndTime;
                TimeSpan TimeTake;
                DataRow dr;
                ShowTaskProgress(0, "Processing 'Commercials' Table to ElasticDB - 0%");
                int i = 0;
                int InsertESData = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TakeCount"]); ;

                SqlConnection objConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ProdDBConnection"].ConnectionString);
                SqlCommand objCommercialCommand = new SqlCommand();

                objCommercialCommand.CommandText = "select CommercialID,RegistrationNo,CommercialName,BusinessStartDate,CommercialStatusCode,LastUpdatedDate from Commercials where IsESSynced = 0 order by CommercialID; select  CommercialAddressID, CommercialAddresses.CommercialID,OriginalAddress1,OriginalAddress2,OriginalAddress3,OriginalAddress4,OriginalPostalCode from CommercialAddresses inner join Commercials on CommercialAddresses.CommercialID = Commercials.CommercialID and IsESSynced = 0 order by CommercialID; select  CommercialTelephoneID, CommercialTelephones.CommercialID,TelephoneNo,TelephoneCode  from CommercialTelephones inner join Commercials on CommercialTelephones.CommercialID = Commercials.CommercialID and IsESSynced = 0 order by CommercialID;";
                objCommercialCommand.CommandType = CommandType.Text;
                objCommercialCommand.Connection = objConnection;
                objCommercialCommand.CommandTimeout = 0;

                if (objConnection.State != ConnectionState.Open)
                    objConnection.Open();

                SqlDataReader rdrCommercial = objCommercialCommand.ExecuteReader(CommandBehavior.SequentialAccess);

                int e = 0, p = 0, a = 0, temp = 0;

                object strTemp1fordtreader;
                object strTemp2fordtreader;
                object strTemp3fordtreader;
                object strTemp4fordtreader;
                object strTemp5fordtreader;

                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                string startId = string.Empty;
                string endId = string.Empty;
                StringResponse stringResponse;

                //Variables for Time tracking.

                DateTime commercialStart = DateTime.Now;
                DateTime commercialEnd = DateTime.Now;
                TimeSpan CommercialTime = commercialEnd - commercialStart;
                DateTime addressStart = DateTime.Now;
                DateTime addressEnd = DateTime.Now;
                TimeSpan addressTime = addressEnd - addressStart;
                DateTime phoneStart = DateTime.Now;
                DateTime phoneEnd = DateTime.Now;
                TimeSpan phoneTime = phoneEnd - phoneStart;
                commercialStart = DateTime.Now;

                List<string> documentLst = new List<string>();

                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Commercials Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;
                ShowTaskProgress(0, "Processing 'Address Parsing'");

                //commercial information
                if (rdrCommercial.HasRows)
                {
                    temp = 0;
                    startId = string.Empty; endId = string.Empty;


                    while (rdrCommercial.Read())
                    {
                        if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                        {
                            sb.Append("\n");

                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "Commercials - " + startId + " - " + endId + ".txt");
                            }

                            commercialEnd = DateTime.Now;
                            CommercialTime = commercialEnd - commercialStart;
                            ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString());

                            sb.Clear();
                            temp = 0;
                            startId = string.Empty;
                        }
                        else
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;
                            strTemp1fordtreader = rdrCommercial["CommercialID"];

                            startId = string.IsNullOrEmpty(startId) ? strTemp1fordtreader.ToString().Replace("\\", "").Replace("\"", "") : startId;
                            endId = strTemp1fordtreader.ToString().Replace("\\", "").Replace("\"", "");

                            sb.Append("{\"index\" : { \"_index\" : \"commercialdata\", \"_type\" : \"commercialdata\", \"_id\" : \"");
                            sb.Append(Convert.ToString(strTemp1fordtreader));
                            sb.Append("\" } } \n");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                writer.WriteStartObject();
                                writer.WritePropertyName("CommercialID");
                                writer.WriteValue(Convert.ToInt32(strTemp1fordtreader));
                                var number = rdrCommercial.GetValue(1);
                                writer.WritePropertyName("companyregnumber");
                                writer.WriteValue(number.ToString());
                                writer.WritePropertyName("companyregno");
                                writer.WriteValue(number.ToString().Replace("/", ""));
                                writer.WritePropertyName("companyname");
                                writer.WriteValue(rdrCommercial.GetValue(2));
                                writer.WritePropertyName("businessstartdate");
                                writer.WriteValue(rdrCommercial.GetValue(3));
                                writer.WritePropertyName("commercialstatuscode");
                                writer.WriteValue(rdrCommercial.GetValue(4));
                                writer.WritePropertyName("lastupdateddate");
                                writer.WriteValue(rdrCommercial.GetValue(5));
                                writer.WriteEndObject();
                            }
                            temp++;

                        }
                        i++;
                    }
                    if (sb.Length != 0)
                    {
                        sb.Append("\n");
                        stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                        if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                        {
                            string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                            WriteToFile(str, "Commercials - " + startId + " - " + endId + ".txt");
                        }
                    }
                }
                commercialEnd = DateTime.Now;
                CommercialTime = commercialEnd - commercialStart;
                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString());

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Commercials Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;


                addressStart = DateTime.Now;
                if (sb.Length > 0)
                    sb.Clear();
                if (documentLst.Count > 0) documentLst.Clear();
                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Commercials Address Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;
                //commercialaddressinformation
                if (rdrCommercial.NextResult())
                {
                    if (rdrCommercial.HasRows)
                    {
                        string CommercialAddressID = string.Empty;
                        string Address = string.Empty;
                        startId = string.Empty; endId = string.Empty;
                        string CommercialId = string.Empty;
                        string CurrentCommercialId = string.Empty;
                        temp = 0;
                        while (rdrCommercial.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "CommercialAddress - " + startId + " - " + endId + ".txt");
                                }
                                addressEnd = DateTime.Now;
                                addressTime = addressEnd - addressStart;
                                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());
                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;
                            }
                            else
                            {
                                CommercialAddressID = rdrCommercial["CommercialAddressID"].ToString();
                                startId = string.IsNullOrEmpty(startId) ? CommercialAddressID : startId;
                                endId = CommercialAddressID;

                                CurrentCommercialId = rdrCommercial["CommercialID"].ToString();
                                CommercialId = string.IsNullOrEmpty(CommercialId) ? CurrentCommercialId : CommercialId;

                                strTemp1fordtreader = rdrCommercial["OriginalAddress1"];
                                strTemp2fordtreader = rdrCommercial["OriginalAddress2"];
                                strTemp3fordtreader = rdrCommercial["OriginalAddress3"];
                                strTemp4fordtreader = rdrCommercial["OriginalAddress4"];
                                strTemp5fordtreader = rdrCommercial["OriginalPostalCode"];

                                Address = ((!string.IsNullOrEmpty(strTemp1fordtreader.ToString()) ? strTemp1fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp2fordtreader.ToString()) ? strTemp2fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp3fordtreader.ToString()) ? strTemp3fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp4fordtreader.ToString()) ? strTemp4fordtreader.ToString().Trim() + " " : "")
                                                                            + (!string.IsNullOrEmpty(strTemp5fordtreader.ToString()) ? strTemp5fordtreader.ToString().Trim() + " " : ""));

                                if (CommercialId == CurrentCommercialId)
                                    documentLst.Add(Address);
                                else
                                {
                                    sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                    sb.Append("{ \"update\" : {\"_id\" : \"");
                                    sb.Append(CommercialId);
                                    sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                                    sb.Append("{ \"doc\" :  ");

                                    sw = new StringWriter(sb);
                                    using (JsonWriter writer = new JsonTextWriter(sw))
                                    {

                                        writer.WriteStartObject();
                                        writer.WritePropertyName("commercialaddress");
                                        writer.WriteStartArray();
                                        foreach (string str in documentLst)
                                            writer.WriteValue(str.Trim());
                                        writer.WriteEnd();
                                        writer.WriteEndObject();
                                    }
                                    sb.Append("}");
                                    documentLst.Clear();
                                    CommercialId = CurrentCommercialId;
                                    documentLst.Add(Address);
                                }

                                temp++;

                            }
                            a++;
                        }
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(CommercialId);
                            sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {

                                writer.WriteStartObject();
                                writer.WritePropertyName("commercialaddress");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "CommercialAddress - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }
                }
                addressEnd = DateTime.Now;
                addressTime = addressEnd - addressStart;

                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Commercial Address Count: " + i.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString());


                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Commercials Address Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                phoneStart = DateTime.Now;
                if (sb.Length > 0)
                    sb.Clear();

                if (documentLst.Count > 0)
                    documentLst.Clear();

                dtStartTime = DateTime.Now;
                dr = dt.NewRow();

                dr["Table"] = "Pushing Commercials Phone Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                if (rdrCommercial.NextResult())
                {
                    if (rdrCommercial.HasRows)
                    {
                        string PhoneNumber = string.Empty;
                        string CommercialTelephoneID = string.Empty;
                        string TelephoneCode = string.Empty;
                        bool add = false;
                        temp = 0;
                        startId = string.Empty; endId = string.Empty;
                        string CommercialId = string.Empty;
                        string CurrentCommercialId = string.Empty;
                        while (rdrCommercial.Read())
                        {
                            if (temp == InsertESData || sb.Length + 1000 == sb.MaxCapacity)
                            {
                                sb.Append("\n");
                                stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                                if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                                {
                                    string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                    WriteToFile(str, "CommercialPhone - " + startId + " - " + endId + ".txt");
                                }

                                sb.Clear();
                                temp = 0;
                                startId = string.Empty; endId = string.Empty;

                                phoneEnd = DateTime.Now;
                                phoneTime = phoneEnd - phoneStart;
                                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString());

                            }
                            else
                            {
                                PhoneNumber = string.Empty;
                                CommercialTelephoneID = string.Empty;
                                add = false;

                                CommercialTelephoneID = rdrCommercial["CommercialTelephoneID"].ToString();
                                CurrentCommercialId = rdrCommercial["CommercialID"].ToString();
                                CommercialId = string.IsNullOrEmpty(CommercialId) ? CurrentCommercialId : CommercialId;
                                PhoneNumber = rdrCommercial["TelephoneNo"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                                TelephoneCode = rdrCommercial["TelephoneCode"].ToString().Replace("\\", "\\\\").Replace("\"", "\\\"");
                                PhoneNumber = TelephoneCode + PhoneNumber;
                                if (PhoneNumber.Length == 10)
                                    add = true;

                                if (add)
                                {
                                    if (CommercialId == CurrentCommercialId)
                                    {
                                        documentLst.Add(PhoneNumber);
                                    }
                                    else
                                    {
                                        sb = sb.Length != 0 ? sb.Append("\n") : sb;

                                        sb.Append("{ \"update\" : {\"_id\" : \"");
                                        sb.Append(CommercialId);
                                        sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                                        sb.Append("{ \"doc\" :  ");

                                        sw = new StringWriter(sb);
                                        using (JsonWriter writer = new JsonTextWriter(sw))
                                        {

                                            writer.WriteStartObject();
                                            writer.WritePropertyName("commercialtelephone");
                                            writer.WriteStartArray();
                                            foreach (string str in documentLst)
                                                writer.WriteValue(str.Trim());
                                            writer.WriteEnd();
                                            writer.WriteEndObject();
                                        }
                                        sb.Append("}");
                                        documentLst.Clear();
                                        CommercialId = CurrentCommercialId;
                                        documentLst.Add(PhoneNumber);
                                    }
                                }
                                temp++;

                            }
                            p++;
                        }
                        if (documentLst.Count > 0)
                        {
                            sb = sb.Length != 0 ? sb.Append("\n") : sb;

                            sb.Append("{ \"update\" : {\"_id\" : \"");
                            sb.Append(CommercialId);
                            sb.Append("\", \"_type\" : \"commercialdata\", \"_index\" : \"commercialdata\"} } \n");
                            sb.Append("{ \"doc\" :  ");

                            sw = new StringWriter(sb);
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {

                                writer.WriteStartObject();
                                writer.WritePropertyName("commercialtelephone");
                                writer.WriteStartArray();
                                foreach (string str in documentLst)
                                    writer.WriteValue(str.Trim());
                                writer.WriteEnd();
                                writer.WriteEndObject();
                            }
                            sb.Append("}");
                            documentLst.Clear();
                        }
                        if (sb.Length != 0)
                        {
                            sb.Append("\n");
                            stringResponse = await es.UpdsertinformationLowlevel(sb.ToString(), elasticIP);

                            if (stringResponse.Body.Contains("\"errors\":true") || string.IsNullOrEmpty(stringResponse.Body))
                            {
                                string[] str = new string[] { string.IsNullOrEmpty(stringResponse.Body) ? stringResponse.DebugInformation : stringResponse.Body };
                                WriteToFile(str, "CommercialPhone - " + startId + " - " + endId + ".txt");
                            }
                        }
                    }

                }
                phoneEnd = DateTime.Now;
                phoneTime = phoneEnd - phoneStart;
                TimeSpan TotalTime = phoneEnd - commercialStart;
                ShowTaskProgress(100, "Commercial Count: " + i.ToString() + " Time Taken: " + Math.Round(CommercialTime.TotalMinutes, 2).ToString() + " Addresses Count:" + a.ToString() + " Time Taken: " + Math.Round(addressTime.TotalMinutes, 2).ToString() + " Phone Count: " + p.ToString() + " Time Taken: " + Math.Round(phoneTime.TotalMinutes, 2).ToString() + " Total Time: " + Math.Round(TotalTime.TotalMinutes, 2).ToString());

                dtEndTime = DateTime.Now;
                TimeTake = dtEndTime - dtStartTime;
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                dr["Table"] = "Pushing Commercials Phone Information to Elastic";
                dr["Start Time"] = dtStartTime;
                dr["End Time"] = dtEndTime;
                dr["Time Taken"] = TimeTake.Hours.ToString() + " Hrs " + TimeTake.Minutes.ToString() + " Mins " + TimeTake.Seconds.ToString() + " Secs";
                dt.Rows.Add(dr);
                Log_DataGridView.DataSource = dt;

                ShowTaskProgress(100, "ETL Process Completed");


            }
            catch (Exception ex)
            {
                ShowTaskProgress(100, ex.Message.ToString());
                throw ex;

            }
        }
        public void ShowTaskProgress(int progess, string message)
        {
            TaskProgress_TextBox.Text = message;
            TaskProgress_TextBox.Invalidate();
            TaskProgress_TextBox.Update();
            TaskProgress_TextBox.Refresh();

            TaskProgress_ProgressBar.Value = progess;
            TaskProgress_ProgressBar.Invalidate();
            TaskProgress_ProgressBar.Update();
            TaskProgress_ProgressBar.Refresh();

            System.Windows.Forms.Application.DoEvents();


        }

        private Log WriteLog(Util.LogTypeEnum logType, string logDescription, string table, string errortype = "")
        {
            Log logfile = new Log();
            logfile.LogType = logType.ToString();
            logfile.LogDescription = logDescription;
            logfile.TableName = table;
            logfile.LogTime = DateTime.Now;

            return logfile;
        }

        public void WriteToFile(string[] lines, string strFileName)
        {
            if (string.IsNullOrEmpty(strFileName)) strFileName = ConfigurationManager.AppSettings["LogPath"] + "\\WriteLines.txt";
            else strFileName = ConfigurationManager.AppSettings["LogPath"] + "\\" + strFileName;

            if (System.IO.File.Exists(strFileName))
                System.IO.File.Delete(strFileName);
            System.IO.File.WriteAllLines(strFileName, lines);
        }

        public static string Remove2(string str)
        {
            return Regex.Replace(str, "[,./?`~!@#$%^&*()-_+=]", "");
        }
    }
}
