using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard InstallDir dialog
    /// </summary>
    public partial class InstallDirDialog : ManagedForm, IManagedDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallDirDialog"/> class.
        /// </summary>
        public InstallDirDialog()
        {
            InitializeComponent();
        }

        string installDirProperty;

        void InstallDirDialog_Load(object sender, EventArgs e)
        {
            installDirProperty = MsiRuntime.Session.Property("WixSharp_UI_INSTALLDIR");
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