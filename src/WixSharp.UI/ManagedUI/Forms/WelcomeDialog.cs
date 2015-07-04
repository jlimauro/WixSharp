using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace WixSharp.UI.Forms
{
    public partial class WelcomeDialog : ManagedForm, IManagedDialog
    {
        public WelcomeDialog()
        {
            InitializeComponent();
        }

        void WelcomeDialog_Load(object sender, EventArgs e)
        {
            back.Enabled = false;
            image.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Dialog");
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }

        void next_Click(object sender, EventArgs e)
        {
            Shell.GoNext();
        }

        void back_Click(object sender, EventArgs e)
        {
            Shell.GoPrev();
        }
    }
}
