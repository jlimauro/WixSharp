using System;
using System.Linq;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class SetupTypeDialog : ManagedForm
    {
        public SetupTypeDialog()
        {
            InitializeComponent();
        }

        Type ProgressDialog
        {
            get
            {
                return Shell.Dialogs
                            .Where(d => d.GetInterfaces().Contains(typeof(IProgressDialog)))
                            .FirstOrDefault();
            }
        }

        void typical_Click(object sender, System.EventArgs e)
        {
            int index = Shell.Dialogs.IndexOf(ProgressDialog);
            if (index != -1)
                Shell.GoTo(index);
            else
                Shell.GoNext();
        }

        void custom_Click(object sender, System.EventArgs e)
        {
            Shell.GoNext();
        }

        void complete_Click(object sender, System.EventArgs e)
        {
            string[] names = MsiRuntime.Session.Features.Select(x => x.Name).ToArray();
            MsiRuntime.Session["ADDLOCAL"] = names.Join(",");

            int index = Shell.Dialogs.IndexOf(ProgressDialog);
            if (index != -1)
                Shell.GoTo(index);
            else
                Shell.GoNext();
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

        void SetupTypeDialog_Load(object sender, System.EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            this.Localize();
        }
    }
}