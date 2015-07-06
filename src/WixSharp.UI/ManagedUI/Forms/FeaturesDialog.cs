using System.Windows.Forms;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class FeaturesDialog : ManagedForm 
    {
        public FeaturesDialog()
        {
            InitializeComponent();
        }

        void FeaturesDialog_Load(object sender, System.EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
        }
        
        void back_Click(object sender, System.EventArgs e)
        {
            Shell.GoPrev();
        }

        void next_Click(object sender, System.EventArgs e)
        {
            Shell.GoNext();
        }

        void cancel_Click(object sender, System.EventArgs e)
        {
            Shell.Cancel();
        }
    }
}