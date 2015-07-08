using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

#pragma warning disable 1591

namespace WixSharp.UI.Forms
{
    public partial class ProgressDialog : ManagedForm
    {
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
                            var message = messageRecord[messageRecord.FieldCount - 1].ToString();

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