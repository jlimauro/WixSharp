using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    public partial class InstallDirDialog : ManagedForm, IManagedDialog
    {
        public InstallDirDialog()
        {
            InitializeComponent();
        }

        string installDirProperty = "INSTALLDIR";

        void InstallDirDialog_Load(object sender, EventArgs e)
        {
            installDirProperty = MsiRuntime.Session.GetInstallDirectoryName(); //user may overwrite it

            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            installDir.Text = MsiRuntime.Session.GetDirectoryPath(installDirProperty);
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            MsiRuntime.Session[installDirProperty] = installDir.Text;
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void change_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog { SelectedPath = installDir.Text })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    installDir.Text = dialog.SelectedPath;
                }
            }
        }
    }
}