using System;
using System.Windows.Forms;
using WixSharp.UI.Forms;

namespace WixSharp.Forms
{
    public class Dialogs
    {
        static public Type Welcome = typeof(WelcomeDialog);
        static public Type Licence = typeof(LicenceDialog);
        static public Type Features = typeof(FeaturesDialog);
        static public Type InstallDir = typeof(InstallDirDialog);
        static public Type Progress = typeof(ProgressDialog);
        static public Type SetupType = typeof(SetupTypeDialog);
        static public Type Exit = typeof(ExitDialog);
    }

    class ShellView : Form
    {
        public ShellView()
        {
            InitializeComponent();
            this.Load += (sender, e) => TopMost = false; //To ensure initial 'on top'
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
            this.Icon = ".msi".GetAssiciatedIcon();
            this.ResumeLayout(false);
        }
    }
}