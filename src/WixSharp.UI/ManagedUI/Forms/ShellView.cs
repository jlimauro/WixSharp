using System.Windows.Forms;

namespace WixSharp
{
    class ShellView : Form
    {
        public ShellView()
        {
            InitializeComponent();
        }

        void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 361);
            this.MaximizeBox = false;
            this.Name = "UIShell";
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += (sender, e) => TopMost = false; //To ensure initial 'on top'
            this.Icon = ".msi".GetAssiciatedIcon();
            this.ResumeLayout(false);
        }
    }
}