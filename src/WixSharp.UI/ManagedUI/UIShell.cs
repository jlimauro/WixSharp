using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Deployment.Samples.EmbeddedUI;
using Microsoft.Deployment.WindowsInstaller;

namespace WixSharp
{
    public interface IUIShell
    {
        void ShowModal(MsiRuntime msiRuntime, IManagedUI ui);
        void OnExecuteComplete();
        void OnExecuteStarted();
        void InUIThread(System.Action action);
        MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);
    }

    public partial class UIShell : Form, IUIShell
    {
        public MsiRuntime msiRuntime;
        public IManagedUI ui;
        InstallProgressCounter progressCounter = new InstallProgressCounter(0.5);
        bool started = false;
        bool canceled = false;

        public UIShell()
        {
            InitializeComponent();
            currentStep.Text = null;
        }

        void next_Click(object sender, EventArgs e)
        {
        }

        void cancel_Click(object sender, EventArgs e)
        {
            canceled = true;
            if(!started)
                Close();
        }

        void Install_Click(object sender, EventArgs e)
        {
            msiRuntime.StartExecute();
        }

        void exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void ShowModal(MsiRuntime msiRuntime, IManagedUI ui)
        {
            listBox1.Items.Clear();

            this.ui = ui;
            this.msiRuntime = msiRuntime;

            if (this.msiRuntime.Session.IsInstalling())
            {
                listBox1.Items.AddRange(ui.InstallDialogs.Select(x => x.ToString()).ToArray());
                status.Text = "Installing";
            }
            else if (this.msiRuntime.Session.IsRepairing())
            {
                listBox1.Items.AddRange(ui.RepairDialogs.Select(x => x.ToString()).ToArray());
                status.Text = "Repairing";
            }
            this.ShowDialog();
        }

        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            try
            {
                this.progressCounter.ProcessMessage(messageType, messageRecord);

                this.currentStep.Text = "" + (int)Math.Round(100 * this.progressCounter.Progress) + "%";
                this.progressBar.Value = (int)(progressBar.Minimum + this.progressCounter.Progress * (this.progressBar.Maximum - this.progressBar.Minimum));

                switch (messageType)
                {
                    case InstallMessage.Error:
                    case InstallMessage.Warning:
                    case InstallMessage.Info:
                        this.LogMessage("{0}: {1}", messageType, messageRecord);
                        break;
                }

                if (this.canceled)
                {
                    return MessageResult.Cancel;
                }
            }
            catch (Exception ex)
            {
                this.LogMessage(ex.ToString());
                this.LogMessage(ex.StackTrace);
            }
            Application.DoEvents();
            return MessageResult.OK;
        }

        private void LogMessage(string message, params object[] args)
        {
            textBox1.AppendText(message.FormatInline(args) + Environment.NewLine);
        }

        public void OnExecuteComplete()
        {
            started = true;
            cancel.Enabled = 
            exit.Enabled = true;
        }

        public void OnExecuteStarted()
        {
            next.Enabled =
            install.Enabled =
            cancel.Enabled =
            exit.Enabled = false;
        }

        public void InUIThread(System.Action action)
        {
            this.Invoke(action);
        }
    }
}
