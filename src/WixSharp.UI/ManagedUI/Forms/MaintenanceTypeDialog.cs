using System;
using System.Linq;


namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Maintenance Type dialog
    /// </summary>
    public partial class MaintenanceTypeDialog : ManagedForm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaintenanceTypeDialog"/> class.
        /// </summary>
        public MaintenanceTypeDialog()
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

        void change_Click(object sender, System.EventArgs e)
        {
            Shell.GoNext();
        }

        void repair_Click(object sender, System.EventArgs e)
        {
            int index = Shell.Dialogs.IndexOf(ProgressDialog);
            if (index != -1)
                Shell.GoTo(index);
            else
                Shell.GoNext();
        }

        void remove_Click(object sender, System.EventArgs e)
        {
            string[] names = MsiRuntime.Session.Features.Select(x => x.Name).ToArray();
            MsiRuntime.Session["REMOVE"] = names.Join(",");

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

        void MaintenanceTypeDialog_Load(object sender, System.EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            this.Localize();
        }
    }
}