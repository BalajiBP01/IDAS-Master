namespace Inspirit.IDAS.ETLApplication
{
    partial class FormETL
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Browse_Button = new System.Windows.Forms.Button();
            this.Process_Button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Log_DataGridView = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExportResult = new System.Windows.Forms.Button();
            this.chkbxProcessSelected = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbxTable = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TaskProgress_TextBox = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TaskProgress_ProgressBar = new System.Windows.Forms.ProgressBar();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Log_DataGridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Browse_Button
            // 
            this.Browse_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Browse_Button.Location = new System.Drawing.Point(797, 3);
            this.Browse_Button.Name = "Browse_Button";
            this.Browse_Button.Size = new System.Drawing.Size(38, 27);
            this.Browse_Button.TabIndex = 2;
            this.Browse_Button.Text = "&Browse...";
            this.Browse_Button.UseVisualStyleBackColor = true;
            this.Browse_Button.Visible = false;
            this.Browse_Button.Click += new System.EventHandler(this.Browse_Button_Click);
            // 
            // Process_Button
            // 
            this.Process_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Process_Button.Location = new System.Drawing.Point(589, 3);
            this.Process_Button.Name = "Process_Button";
            this.Process_Button.Size = new System.Drawing.Size(214, 27);
            this.Process_Button.TabIndex = 3;
            this.Process_Button.Text = "&Process";
            this.Process_Button.UseVisualStyleBackColor = true;
            this.Process_Button.Click += new System.EventHandler(this.Process_Button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.Log_DataGridView);
            this.groupBox1.Location = new System.Drawing.Point(12, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1043, 364);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // Log_DataGridView
            // 
            this.Log_DataGridView.AllowUserToAddRows = false;
            this.Log_DataGridView.AllowUserToDeleteRows = false;
            this.Log_DataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Log_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Log_DataGridView.Location = new System.Drawing.Point(6, 24);
            this.Log_DataGridView.Name = "Log_DataGridView";
            this.Log_DataGridView.ReadOnly = true;
            this.Log_DataGridView.RowHeadersVisible = false;
            this.Log_DataGridView.Size = new System.Drawing.Size(1031, 357);
            this.Log_DataGridView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnExportResult);
            this.panel1.Controls.Add(this.chkbxProcessSelected);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.cmbxTable);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.Process_Button);
            this.panel1.Controls.Add(this.Browse_Button);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1043, 33);
            this.panel1.TabIndex = 0;
            // 
            // btnExportResult
            // 
            this.btnExportResult.Location = new System.Drawing.Point(841, 3);
            this.btnExportResult.Name = "btnExportResult";
            this.btnExportResult.Size = new System.Drawing.Size(184, 27);
            this.btnExportResult.TabIndex = 7;
            this.btnExportResult.Text = "Export Result";
            this.btnExportResult.UseVisualStyleBackColor = true;
            this.btnExportResult.Click += new System.EventHandler(this.btnExportResult_Click);
            // 
            // chkbxProcessSelected
            // 
            this.chkbxProcessSelected.AutoSize = true;
            this.chkbxProcessSelected.Location = new System.Drawing.Point(556, 11);
            this.chkbxProcessSelected.Name = "chkbxProcessSelected";
            this.chkbxProcessSelected.Size = new System.Drawing.Size(15, 14);
            this.chkbxProcessSelected.TabIndex = 6;
            this.chkbxProcessSelected.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(391, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Process Selected Table";
            // 
            // cmbxTable
            // 
            this.cmbxTable.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.cmbxTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbxTable.ForeColor = System.Drawing.SystemColors.InfoText;
            this.cmbxTable.FormattingEnabled = true;
            this.cmbxTable.Items.AddRange(new object[] {
            "Select",
            "TelephoneTable",
            "ConsumerTable",
            "ConsumerHomeAffairsTable",
            "ConsumerEmploymentOccupationTable",
            "ConsumerEmploymentTable",
            "ConsumerAddressTable",
            "ConsumerEmailTable",
            "ConsumerTelephoneTable",
            "ConsumerDebitReviewTable",
            "ConsumerJudementTable",
            "CommercialTable",
            "CommercialTelephoneTable",
            "CommercialAddressTable",
            "CommercialAuditorTable",
            "CommercialJudgementTable",
            "DirectorTable",
            "DirectorTelephoneTable",
            "DirectorAddressTable",
            "CommercialDirectorTable",
            "PropertyDeedTable",
            "PropertyBuyerTable",
            "PropertySellerTable",
            "EndorsementTable",
            "AuditorAddressTable",
            "AddressParsing",
            "AlloyCalculation",
            "LSMCalculation",
            "ES-ConsumerMigration",
            "ES-ConsumerAddressMigration",
            "ES-ConsumerTelephoneMigration",
            "ES-ConsumerEmailMigration",
            "ES-ConsumerHomeAffairsMigration",
            "ES-ConsumerLSMMigration",
            "ES-ConsumerPropertyDeedsMigration",
            "ES-ConsumerDirectorsMigration",
            "ES-ConsumerAdverseMigration",
            "ES-CommercialsMigration",
            "ES-CommercialsAddressMigration",
            "ES-CommercialsPhoneMigration",
            "ES-AddressMigrationForAddressSearch",
            "ES-ConsumerEmploymentMigration"});
            this.cmbxTable.Location = new System.Drawing.Point(115, 5);
            this.cmbxTable.Name = "cmbxTable";
            this.cmbxTable.Size = new System.Drawing.Size(270, 24);
            this.cmbxTable.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Table Name : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Task Progress";
            // 
            // TaskProgress_TextBox
            // 
            this.TaskProgress_TextBox.Location = new System.Drawing.Point(115, 9);
            this.TaskProgress_TextBox.Name = "TaskProgress_TextBox";
            this.TaskProgress_TextBox.ReadOnly = true;
            this.TaskProgress_TextBox.Size = new System.Drawing.Size(922, 23);
            this.TaskProgress_TextBox.TabIndex = 1;
            this.TaskProgress_TextBox.TextChanged += new System.EventHandler(this.TaskProgress_TextBox_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.TaskProgress_TextBox);
            this.panel2.Controls.Add(this.TaskProgress_ProgressBar);
            this.panel2.Location = new System.Drawing.Point(12, 51);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1043, 72);
            this.panel2.TabIndex = 1;
            // 
            // TaskProgress_ProgressBar
            // 
            this.TaskProgress_ProgressBar.Location = new System.Drawing.Point(6, 39);
            this.TaskProgress_ProgressBar.Name = "TaskProgress_ProgressBar";
            this.TaskProgress_ProgressBar.Size = new System.Drawing.Size(1031, 23);
            this.TaskProgress_ProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.TaskProgress_ProgressBar.TabIndex = 2;
            this.TaskProgress_ProgressBar.Click += new System.EventHandler(this.TaskProgress_ProgressBar_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1067, 515);
            this.panel3.TabIndex = 3;
            // 
            // FormETL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 515);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormETL";
            this.Text = "ETL";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Log_DataGridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Browse_Button;
        private System.Windows.Forms.Button Process_Button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox TaskProgress_TextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar TaskProgress_ProgressBar;
        private System.Windows.Forms.DataGridView Log_DataGridView;
        private System.Windows.Forms.ComboBox cmbxTable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkbxProcessSelected;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnExportResult;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Panel panel3;
    }
}

