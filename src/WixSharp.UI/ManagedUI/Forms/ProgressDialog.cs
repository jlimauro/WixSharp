using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;


namespace WixSharp.UI.Forms
{
    /// <summary>
    /// The standard Installation Progress dialog
    /// </summary>
    public partial class ProgressDialog : ManagedForm, IProgressDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        public ProgressDialog()
        {
            //Debugger.Launch();
            InitializeComponent();
        }

        void ProgressDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            Shell.StartExecute();
        }

        public override void OnShellChanged()
        {
            if (MsiRuntime.Session.IsUninstalling())
            {
                Text = "[ProgressDlgTitleRemoving]";
                description.Text = "[ProgressDlgTextRemoving]";
            }
            else if (MsiRuntime.Session.IsRepairing())
            {
                Text = "[ProgressDlgTextRepairing]";
                description.Text = "[ProgressDlgTitleRepairing]"; 
            }
            else if (MsiRuntime.Session.IsInstalling())
            {
                Text = "[ProgressDlgTitleInstalling]";
                description.Text = "[ProgressDlgTextInstalling]";
            }

            this.Localize();
        }

        public override MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            switch (messageType)
            {
                case InstallMessage.ActionStart:
                    {

                        try
                        {
                            //messageRecord[0] - is reserved for FormatString value

                            string message = null;
 
                            bool simple = true;
                            if (simple)
                            {
                                for (int i = messageRecord.FieldCount - 1; i > 0; i--)
                                {
                                    message = messageRecord[i].ToString();
                                }
                            }
                            else
                            {
                                message = messageRecord.FormatString;
                                if (message.IsNotEmpty())
                                {
                                    for (int i = 1; i < messageRecord.FieldCount; i++) 
                                    {
                                        message = message.Replace("[" + i + "]", messageRecord[i].ToString());
                                    }
                                }
                                else
                                {
                                    message = messageRecord[messageRecord.FieldCount - 1].ToString();
                                }
                            }


                            if (message.IsNotEmpty())
                                currentAction.Text = message;
                        }
                        catch { }
                    }
                    break;
            }
            return MessageResult.OK;
        }

        public override void OnProgress(int progressPercentage)
        {
            progress.Value = progressPercentage;
        }

        public override void OnExecuteComplete()
        {
            currentAction.Text = null;
            Shell.GoNext();
        }

        void cancel_Click(object sender, EventArgs e)
        {
            Shell.Cancel();
        }
    }
}