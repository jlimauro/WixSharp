using System;
using System.Windows.Forms;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class LicenceDialog : ManagedForm
    {
        public LicenceDialog()
        {
            InitializeComponent();
            banner.Image = ManagedUI.Resources.WixUI_Bmp_Banner;
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

        private void accepted_CheckedChanged(object sender, EventArgs e)
        {
            next.Enabled = accepted.Checked;
        }
    }
    
   
}