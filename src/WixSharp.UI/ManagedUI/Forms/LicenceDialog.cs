using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using io = System.IO;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class LicenceDialog : ManagedForm
    {
        public LicenceDialog()
        {
            InitializeComponent();
        }

        void LicenceDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            agreement.Rtf = MsiRuntime.Session.GetEmbeddedString("WixSharp_LicenceFile");
            accepted.Checked = MsiRuntime.Session["LastLicenceAcceptedChecked"] == "True";
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, EventArgs e)
        {
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void accepted_CheckedChanged(object sender, EventArgs e)
        {
            next.Enabled = accepted.Checked;
            MsiRuntime.Session["LastLicenceAcceptedChecked"] = accepted.Checked.ToString();
        }

        void print_Click(object sender, EventArgs e)
        {
            try
            {
                var file = Path.Combine(Path.GetTempPath(), MsiRuntime.Session.Property("ProductName") + ".licence.rtf");
                io.File.WriteAllText(file, agreement.Rtf);
                Process.Start(file);
            }
            catch { }
        }

        void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var data = new DataObject();

                if (agreement.SelectedText.Length > 0)
                {
                    data.SetData(DataFormats.UnicodeText, agreement.SelectedText);
                    data.SetData(DataFormats.Rtf, agreement.SelectedRtf);
                }
                else
                {
                    data.SetData(DataFormats.Rtf, agreement.Rtf);
                    data.SetData(DataFormats.Text, agreement.Text);
                }

                Clipboard.SetDataObject(data);
            }
            catch { }
        }
    }
}
