namespace DataCleansing
{
    partial class DataCleanerDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkListInputColumns = new System.Windows.Forms.CheckedListBox();
            this.cboConnectionManager = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grpColumnProperties = new System.Windows.Forms.GroupBox();
            this.chkOperations = new System.Windows.Forms.CheckedListBox();
            this.txtValidValues = new System.Windows.Forms.TextBox();
            this.txtMaxValue = new System.Windows.Forms.TextBox();
            this.txtMinValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFormatString = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpColumnProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(372, 263);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(453, 263);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkListInputColumns
            // 
            this.chkListInputColumns.FormattingEnabled = true;
            this.chkListInputColumns.Location = new System.Drawing.Point(12, 12);
            this.chkListInputColumns.Name = "chkListInputColumns";
            this.chkListInputColumns.Size = new System.Drawing.Size(140, 199);
            this.chkListInputColumns.TabIndex = 4;
            this.chkListInputColumns.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkListInputColumns_ItemCheck);
            this.chkListInputColumns.SelectedIndexChanged += new System.EventHandler(this.chkListInputColumns_SelectedIndexChanged);
            // 
            // cboConnectionManager
            // 
            this.cboConnectionManager.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConnectionManager.FormattingEnabled = true;
            this.cboConnectionManager.Location = new System.Drawing.Point(161, 236);
            this.cboConnectionManager.Name = "cboConnectionManager";
            this.cboConnectionManager.Size = new System.Drawing.Size(367, 21);
            this.cboConnectionManager.TabIndex = 17;
            this.cboConnectionManager.SelectedIndexChanged += new System.EventHandler(this.cboConnectionManager_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 239);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Connection Manager:";
            // 
            // grpColumnProperties
            // 
            this.grpColumnProperties.Controls.Add(this.chkOperations);
            this.grpColumnProperties.Controls.Add(this.txtValidValues);
            this.grpColumnProperties.Controls.Add(this.txtMaxValue);
            this.grpColumnProperties.Controls.Add(this.txtMinValue);
            this.grpColumnProperties.Controls.Add(this.label5);
            this.grpColumnProperties.Controls.Add(this.txtFormatString);
            this.grpColumnProperties.Controls.Add(this.label4);
            this.grpColumnProperties.Controls.Add(this.label3);
            this.grpColumnProperties.Controls.Add(this.label2);
            this.grpColumnProperties.Controls.Add(this.label1);
            this.grpColumnProperties.Location = new System.Drawing.Point(161, 12);
            this.grpColumnProperties.Name = "grpColumnProperties";
            this.grpColumnProperties.Size = new System.Drawing.Size(367, 218);
            this.grpColumnProperties.TabIndex = 18;
            this.grpColumnProperties.TabStop = false;
            this.grpColumnProperties.Text = "Column Properties";
            // 
            // chkOperations
            // 
            this.chkOperations.FormattingEnabled = true;
            this.chkOperations.Items.AddRange(new object[] {
            "SetNullDefault",
            "TrimString",
            "FormatValue",
            "ValidateRange",
            "ValidateKnownGood"});
            this.chkOperations.Location = new System.Drawing.Point(87, 17);
            this.chkOperations.Name = "chkOperations";
            this.chkOperations.Size = new System.Drawing.Size(261, 79);
            this.chkOperations.TabIndex = 25;
            // 
            // txtValidValues
            // 
            this.txtValidValues.Location = new System.Drawing.Point(87, 195);
            this.txtValidValues.Name = "txtValidValues";
            this.txtValidValues.Size = new System.Drawing.Size(261, 20);
            this.txtValidValues.TabIndex = 24;
            // 
            // txtMaxValue
            // 
            this.txtMaxValue.Location = new System.Drawing.Point(87, 169);
            this.txtMaxValue.Name = "txtMaxValue";
            this.txtMaxValue.Size = new System.Drawing.Size(261, 20);
            this.txtMaxValue.TabIndex = 23;
            // 
            // txtMinValue
            // 
            this.txtMinValue.Location = new System.Drawing.Point(87, 143);
            this.txtMinValue.Name = "txtMinValue";
            this.txtMinValue.Size = new System.Drawing.Size(261, 20);
            this.txtMinValue.TabIndex = 22;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Valid Values";
            // 
            // txtFormatString
            // 
            this.txtFormatString.Location = new System.Drawing.Point(87, 117);
            this.txtFormatString.Name = "txtFormatString";
            this.txtFormatString.Size = new System.Drawing.Size(261, 20);
            this.txtFormatString.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Max Value";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Min Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Format String";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Operation";
            // 
            // DataCleanerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 298);
            this.Controls.Add(this.grpColumnProperties);
            this.Controls.Add(this.cboConnectionManager);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.chkListInputColumns);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Name = "DataCleanerDialog";
            this.Text = "Data Cleaner Editor";
            this.Load += new System.EventHandler(this.DataCleanerDialog_Load);
            this.grpColumnProperties.ResumeLayout(false);
            this.grpColumnProperties.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckedListBox chkListInputColumns;
        private System.Windows.Forms.ComboBox cboConnectionManager;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpColumnProperties;
        private System.Windows.Forms.CheckedListBox chkOperations;
        private System.Windows.Forms.TextBox txtValidValues;
        private System.Windows.Forms.TextBox txtMaxValue;
        private System.Windows.Forms.TextBox txtMinValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtFormatString;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}