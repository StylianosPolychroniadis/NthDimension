namespace NthDimension
{
    partial class ExceptionForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.ckbLogError = new System.Windows.Forms.CheckBox();
            this.lbErrorInfo = new System.Windows.Forms.Label();
            this.rtbErrorDetails = new System.Windows.Forms.RichTextBox();
            this.ckbErrorDetails = new System.Windows.Forms.CheckBox();
            this.lbErrorDetails = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(310, 124);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ckbLogError
            // 
            this.ckbLogError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckbLogError.AutoSize = true;
            this.ckbLogError.Checked = true;
            this.ckbLogError.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbLogError.Location = new System.Drawing.Point(12, 130);
            this.ckbLogError.Name = "ckbLogError";
            this.ckbLogError.Size = new System.Drawing.Size(68, 17);
            this.ckbLogError.TabIndex = 3;
            this.ckbLogError.Text = "Log error";
            this.ckbLogError.UseVisualStyleBackColor = true;
            // 
            // lbErrorInfo
            // 
            this.lbErrorInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbErrorInfo.Location = new System.Drawing.Point(12, 9);
            this.lbErrorInfo.Name = "lbErrorInfo";
            this.lbErrorInfo.Size = new System.Drawing.Size(370, 99);
            this.lbErrorInfo.TabIndex = 2;
            this.lbErrorInfo.Text = resources.GetString("lbErrorInfo.Text");
            // 
            // rtbErrorDetails
            // 
            this.rtbErrorDetails.BackColor = System.Drawing.SystemColors.Control;
            this.rtbErrorDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbErrorDetails.DetectUrls = false;
            this.rtbErrorDetails.Location = new System.Drawing.Point(12, 137);
            this.rtbErrorDetails.Margin = new System.Windows.Forms.Padding(3, 13, 3, 13);
            this.rtbErrorDetails.Name = "rtbErrorDetails";
            this.rtbErrorDetails.ReadOnly = true;
            this.rtbErrorDetails.Size = new System.Drawing.Size(370, 258);
            this.rtbErrorDetails.TabIndex = 4;
            this.rtbErrorDetails.Text = "";
            this.rtbErrorDetails.Visible = false;
            // 
            // ckbErrorDetails
            // 
            this.ckbErrorDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ckbErrorDetails.AutoSize = true;
            this.ckbErrorDetails.Location = new System.Drawing.Point(12, 111);
            this.ckbErrorDetails.Name = "ckbErrorDetails";
            this.ckbErrorDetails.Size = new System.Drawing.Size(86, 17);
            this.ckbErrorDetails.TabIndex = 2;
            this.ckbErrorDetails.Text = "Show details";
            this.ckbErrorDetails.UseVisualStyleBackColor = true;
            this.ckbErrorDetails.CheckedChanged += new System.EventHandler(this.ckbErrorDetails_CheckedChanged);
            // 
            // lbErrorDetails
            // 
            this.lbErrorDetails.AutoSize = true;
            this.lbErrorDetails.Location = new System.Drawing.Point(11, 121);
            this.lbErrorDetails.Name = "lbErrorDetails";
            this.lbErrorDetails.Size = new System.Drawing.Size(65, 13);
            this.lbErrorDetails.TabIndex = 6;
            this.lbErrorDetails.Text = "Error details:";
            this.lbErrorDetails.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.button1.Location = new System.Drawing.Point(183, 124);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Ignore and Continue";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ExceptionForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 159);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ckbErrorDetails);
            this.Controls.Add(this.lbErrorInfo);
            this.Controls.Add(this.ckbLogError);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rtbErrorDetails);
            this.Controls.Add(this.lbErrorDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Unexpected error";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox ckbLogError;
        private System.Windows.Forms.Label lbErrorInfo;
        private System.Windows.Forms.RichTextBox rtbErrorDetails;
        private System.Windows.Forms.CheckBox ckbErrorDetails;
        private System.Windows.Forms.Label lbErrorDetails;
        private System.Windows.Forms.Button button1;
    }
}