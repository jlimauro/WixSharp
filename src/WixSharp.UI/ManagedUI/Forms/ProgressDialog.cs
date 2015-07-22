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
            InitializeComponent();
        }

        void ProgressDialog_Load(object sender, EventArgs e)
        {
            banner.Image = MsiRuntime.Session.GetEmbeddedBitmap("WixUI_Bmp_Banner");
            Shell.StartExecute();
        }

        /// <summary>
        /// Called when Shell is changed. It is a good place to initialize the dialog to reflect the MSI session 
        /// (e.g. localize the view).
        /// </summary>
        protected override void OnShellChanged()
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

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Called when MSI execution progress is changed.
        /// </summary>
        /// <param name="progressPercentage">The progress percentage.</param>
        public override void OnProgress(int progressPercentage)
        {
            progress.Value = progressPercentage;
        }

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
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