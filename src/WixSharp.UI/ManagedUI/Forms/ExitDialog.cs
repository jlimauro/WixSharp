using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    public partial class ExitDialog : ManagedForm
    {
        public ExitDialog()
        {
            InitializeComponent();
        }

        void ExitDialog_Load(object sender, System.EventArgs e)
        {
            image.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Dialog");
            if (Shell.UserInterrupted || Shell.ErrorDetected)
            {
                description.Text = "[UserExitDescription1]";
                this.Localize();
            }
        }

        void finish_Click(object sender, System.EventArgs e)
        {
            Shell.Exit();
        }

        void viewLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string wixSharpDir = Path.Combine(Path.GetTempPath(), @"WixSharp");
                if (!Directory.Exists(wixSharpDir))
                    Directory.CreateDirectory(wixSharpDir);
                
                string logFile = Path.Combine(wixSharpDir, MsiRuntime.ProductName + ".log");
                System.IO.File.WriteAllText(logFile, Shell.Log);
                Process.Start(logFile);
            }
            catch { }
        }
    }
}
