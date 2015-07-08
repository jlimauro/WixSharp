namespace WixSharp.UI.Forms
{
    partial class ExitDialog
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
            this.description = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.viewLog = new System.Windows.Forms.LinkLabel();
            this.back = new System.Windows.Forms.Button();
            this.finish = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.image = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            this.SuspendLayout();
            // 
            // description
            // 
            this.description.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.description.Location = new System.Drawing.Point(177, 84);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(305, 208);
            this.description.TabIndex = 7;
            this.description.Text = "[ExitDialogDescription]";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(176, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 61);
            this.label1.TabIndex = 6;
            this.label1.Text = "[ExitDialogTitle]";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.viewLog);
            this.panel1.Controls.Add(this.back);
            this.panel1.Controls.Add(this.finish);
            this.panel1.Controls.Add(this.cancel);
            this.panel1.Location = new System.Drawing.Point(-4, 308);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(503, 57);
            this.panel1.TabIndex = 5;
            // 
            // viewLog
            // 
            this.viewLog.AutoSize = true;
            this.viewLog.Location = new System.Drawing.Point(16, 21);
            this.viewLog.Name = "viewLog";
            this.viewLog.Size = new System.Drawing.Size(54, 13);
            this.viewLog.TabIndex = 1;
            this.viewLog.TabStop = true;
            this.viewLog.Text = "[ViewLog]";
            this.viewLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.viewLog_LinkClicked);
            // 
            // back
            // 
            this.back.Enabled = false;
            this.back.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back.Location = new System.Drawing.Point(227, 12);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(75, 23);
            this.back.TabIndex = 0;
            this.back.Text = "[WixUIBack]";
            this.back.UseVisualStyleBackColor = true;
            // 
            // finish
            // 
            this.finish.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finish.Location = new System.Drawing.Point(308, 12);
            this.finish.Name = "finish";
            this.finish.Size = new System.Drawing.Size(75, 23);
            this.finish.TabIndex = 0;
            this.finish.Text = "[WixUIFinish]";
            this.finish.UseVisualStyleBackColor = true;
            this.finish.Click += new System.EventHandler(this.finish_Click);
            // 
            // cancel
            // 
            this.cancel.Enabled = false;
            this.cancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancel.Location = new System.Drawing.Point(404, 12);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "[WixUICancel]";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // image
            // 
            this.image.Location = new System.Drawing.Point(0, -4);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(156, 312);
            this.image.TabIndex = 4;
            this.image.TabStop = false;
            // 
            // ExitDialog
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.Controls.Add(this.description);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.image);
            this.Name = "ExitDialog";
            this.Text = "[ExitDialog_Title]";
            this.Load += new System.EventHandler(this.ExitDialog_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label description;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Button finish;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.PictureBox image;
        private System.Windows.Forms.LinkLabel viewLog;
    }
}