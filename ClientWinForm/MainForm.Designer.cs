namespace ClientWinForm
{
    partial class MainForm
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
            this.dgwFilesList = new System.Windows.Forms.DataGridView();
            this.btnLoadUserFiles = new System.Windows.Forms.Button();
            this.btnUploadNewFile = new System.Windows.Forms.Button();
            this.btnDownloadAndOpen = new System.Windows.Forms.Button();
            this.cmbSelectEncryption = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgwFilesList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgwFilesList
            // 
            this.dgwFilesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwFilesList.Location = new System.Drawing.Point(56, 84);
            this.dgwFilesList.Margin = new System.Windows.Forms.Padding(4);
            this.dgwFilesList.Name = "dgwFilesList";
            this.dgwFilesList.RowHeadersWidth = 51;
            this.dgwFilesList.Size = new System.Drawing.Size(964, 265);
            this.dgwFilesList.TabIndex = 0;
            // 
            // btnLoadUserFiles
            // 
            this.btnLoadUserFiles.Location = new System.Drawing.Point(56, 48);
            this.btnLoadUserFiles.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadUserFiles.Name = "btnLoadUserFiles";
            this.btnLoadUserFiles.Size = new System.Drawing.Size(131, 28);
            this.btnLoadUserFiles.TabIndex = 1;
            this.btnLoadUserFiles.Text = "Get Files List";
            this.btnLoadUserFiles.UseVisualStyleBackColor = true;
            this.btnLoadUserFiles.Click += new System.EventHandler(this.btnLoadUserFiles_Click);
            // 
            // btnUploadNewFile
            // 
            this.btnUploadNewFile.Location = new System.Drawing.Point(56, 356);
            this.btnUploadNewFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnUploadNewFile.Name = "btnUploadNewFile";
            this.btnUploadNewFile.Size = new System.Drawing.Size(131, 28);
            this.btnUploadNewFile.TabIndex = 2;
            this.btnUploadNewFile.Text = "Upload New File";
            this.btnUploadNewFile.UseVisualStyleBackColor = true;
            this.btnUploadNewFile.Click += new System.EventHandler(this.btnUploadNewFile_Click);
            // 
            // btnDownloadAndOpen
            // 
            this.btnDownloadAndOpen.Location = new System.Drawing.Point(860, 356);
            this.btnDownloadAndOpen.Margin = new System.Windows.Forms.Padding(4);
            this.btnDownloadAndOpen.Name = "btnDownloadAndOpen";
            this.btnDownloadAndOpen.Size = new System.Drawing.Size(160, 28);
            this.btnDownloadAndOpen.TabIndex = 3;
            this.btnDownloadAndOpen.Text = "Download And Open";
            this.btnDownloadAndOpen.UseVisualStyleBackColor = true;
            this.btnDownloadAndOpen.Click += new System.EventHandler(this.btnDownloadAndOpen_Click);
            // 
            // cmbSelectEncryption
            // 
            this.cmbSelectEncryption.FormattingEnabled = true;
            this.cmbSelectEncryption.Location = new System.Drawing.Point(225, 356);
            this.cmbSelectEncryption.Name = "cmbSelectEncryption";
            this.cmbSelectEncryption.Size = new System.Drawing.Size(121, 24);
            this.cmbSelectEncryption.TabIndex = 4;
            this.cmbSelectEncryption.SelectedIndexChanged += new System.EventHandler(this.cmbSelectEncryption_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1101, 586);
            this.Controls.Add(this.cmbSelectEncryption);
            this.Controls.Add(this.dgwFilesList);
            this.Controls.Add(this.btnDownloadAndOpen);
            this.Controls.Add(this.btnUploadNewFile);
            this.Controls.Add(this.btnLoadUserFiles);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "CryptoClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgwFilesList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgwFilesList;
        private System.Windows.Forms.Button btnLoadUserFiles;
        private System.Windows.Forms.Button btnUploadNewFile;
        private System.Windows.Forms.Button btnDownloadAndOpen;
        private System.Windows.Forms.ComboBox cmbSelectEncryption;
    }
}

