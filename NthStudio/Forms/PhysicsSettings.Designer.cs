namespace NthStudio.Forms
{
    partial class PhysicsSettingsForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numAllowedPenetration = new System.Windows.Forms.NumericUpDown();
            this.numBiasFactor = new System.Windows.Forms.NumericUpDown();
            this.numMaxBias = new System.Windows.Forms.NumericUpDown();
            this.numBreakThreshold = new System.Windows.Forms.NumericUpDown();
            this.numMinVelocity = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAllowedPenetration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBiasFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBias)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBreakThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinVelocity)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numMinVelocity);
            this.groupBox1.Controls.Add(this.numBreakThreshold);
            this.groupBox1.Controls.Add(this.numMaxBias);
            this.groupBox1.Controls.Add(this.numBiasFactor);
            this.groupBox1.Controls.Add(this.numAllowedPenetration);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 192);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // numAllowedPenetration
            // 
            this.numAllowedPenetration.DecimalPlaces = 3;
            this.numAllowedPenetration.Location = new System.Drawing.Point(120, 36);
            this.numAllowedPenetration.Name = "numAllowedPenetration";
            this.numAllowedPenetration.Size = new System.Drawing.Size(93, 20);
            this.numAllowedPenetration.TabIndex = 0;
            // 
            // numBiasFactor
            // 
            this.numBiasFactor.DecimalPlaces = 3;
            this.numBiasFactor.Location = new System.Drawing.Point(120, 62);
            this.numBiasFactor.Name = "numBiasFactor";
            this.numBiasFactor.Size = new System.Drawing.Size(93, 20);
            this.numBiasFactor.TabIndex = 1;
            // 
            // numMaxBias
            // 
            this.numMaxBias.DecimalPlaces = 3;
            this.numMaxBias.Location = new System.Drawing.Point(120, 88);
            this.numMaxBias.Name = "numMaxBias";
            this.numMaxBias.Size = new System.Drawing.Size(93, 20);
            this.numMaxBias.TabIndex = 2;
            // 
            // numBreakThreshold
            // 
            this.numBreakThreshold.DecimalPlaces = 3;
            this.numBreakThreshold.Location = new System.Drawing.Point(120, 114);
            this.numBreakThreshold.Name = "numBreakThreshold";
            this.numBreakThreshold.Size = new System.Drawing.Size(93, 20);
            this.numBreakThreshold.TabIndex = 3;
            // 
            // numMinVelocity
            // 
            this.numMinVelocity.DecimalPlaces = 3;
            this.numMinVelocity.Location = new System.Drawing.Point(120, 140);
            this.numMinVelocity.Name = "numMinVelocity";
            this.numMinVelocity.Size = new System.Drawing.Size(93, 20);
            this.numMinVelocity.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Allowed penetration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Bias factor";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Maximum bias";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Break threshold";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Minimum Velocity";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(151, 198);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(66, 26);
            this.btnApply.TabIndex = 1;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(79, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(66, 26);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // PhysicsSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 226);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PhysicsSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Physics Engine";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAllowedPenetration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBiasFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxBias)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBreakThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinVelocity)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numMinVelocity;
        private System.Windows.Forms.NumericUpDown numBreakThreshold;
        private System.Windows.Forms.NumericUpDown numMaxBias;
        private System.Windows.Forms.NumericUpDown numBiasFactor;
        private System.Windows.Forms.NumericUpDown numAllowedPenetration;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}