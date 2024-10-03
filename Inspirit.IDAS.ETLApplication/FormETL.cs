using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Serilog;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;

namespace Inspirit.IDAS.ETLApplication
{
    public partial class FormETL : Form
    {
        public FormETL()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Table"));
            dt.Columns.Add(new DataColumn("Start Time", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("End Time", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Time Taken"));

            Log_DataGridView.DataSource = dt;
            cmbxTable.SelectedIndex = 0;
        }

        private async void Process_Button_Click(object sender, EventArgs e)
        {
            if (chkbxProcessSelected.Checked && cmbxTable.SelectedIndex == 0)
            {
                MessageBox.Show("Select Table Name","IDAS-ETL");
                cmbxTable.Focus();
            }
            else
            {
                cmbxTable.Enabled = false;
                chkbxProcessSelected.Enabled = false;
                Process_Button.Enabled = false;
                btnExportResult.Enabled = false;
                //DbHelper.RawConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["RawDBConnection"].ConnectionString;

                string LogFileName = AppDomain.CurrentDomain.BaseDirectory + @"Logs\" + @"Log " + DateTime.Now.ToString(@"dd-MM-yyyy hh.mm.ss tt") + @".txt";

                var log = new LoggerConfiguration()
                    .WriteTo.File(LogFileName)
                    .CreateLogger();

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    ETL etl = new ETL()
                    {
                        log = log,
                        TaskProgress_TextBox = this.TaskProgress_TextBox,
                        TaskProgress_ProgressBar = this.TaskProgress_ProgressBar,
                        Log_DataGridView = this.Log_DataGridView,
                        cmbTable = cmbxTable,
                        chkbxProcessSelectedTable = chkbxProcessSelected
                    };

                    await etl.Process();
                    if (!chkbxProcessSelected.Checked)
                        TaskProgress_TextBox.Text = "ETL Process Completed";
                    else
                        TaskProgress_TextBox.Text = cmbxTable.SelectedItem.ToString() + " Loaded Successfully.";
                    cmbxTable.Enabled = true;
                    chkbxProcessSelected.Enabled = true;
                    Process_Button.Enabled = true;
                    btnExportResult.Enabled = true;
                    this.Cursor = Cursors.Default;

                }
                catch (Exception ex)
                {
                    cmbxTable.Enabled = true;
                    chkbxProcessSelected.Enabled = true;
                    Process_Button.Enabled = true;
                    btnExportResult.Enabled = true;
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }

                Log.CloseAndFlush();

                log.Dispose();
                log = null;
            }
        }

        private void Browse_Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {   InitialDirectory = "c:\\",
                Filter = "mdf files (*.mdf)|*.mdf",
                FilterIndex = 1,
                Title= "Select a file to process"                
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //DatabaseFileName_TextBox.Text = openFileDialog.FileName;                
            }
        }

        private void btnExportResult_Click(object sender, EventArgs e)
        {
            try
            {
                if (((DataTable)Log_DataGridView.DataSource).Rows.Count > 0)
                {

                    if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(folderBrowserDialog1.SelectedPath + "\\IDAS-ETL-Results.xlsx"))
                        {
                            File.Delete(folderBrowserDialog1.SelectedPath + "\\IDAS-ETL-Results.xlsx");
                        }
                        using (SpreadsheetDocument document = SpreadsheetDocument.Create(folderBrowserDialog1.SelectedPath + "\\IDAS-ETL-Results.xlsx", SpreadsheetDocumentType.Workbook))
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
                            DataTable table = (DataTable)Log_DataGridView.DataSource;
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
                            MessageBox.Show("Results Exported.", "IDAS-ETL");
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error Occured While Exporting Results. /n " + ex.Message, "IDAS-ETL");
            }
        }

        private void TaskProgress_TextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void TaskProgress_ProgressBar_Click(object sender, EventArgs e)
        {

        }
    }
}
