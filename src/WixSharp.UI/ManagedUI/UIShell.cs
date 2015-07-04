using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Deployment.Samples.EmbeddedUI;
using Microsoft.Deployment.WindowsInstaller;
using forms = System.Windows.Forms;

#pragma warning disable 1591

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

    public partial class UIShell : IUIShell, IManagedDialogContainer
    {
        public object RuntimeContext { get { return MsiRuntime; } }
        public MsiRuntime MsiRuntime { get; set; }
        public IManagedUI UI { get; set; }
        public void StartExecute()
        {
            //msiRuntime.Session["INSTALLDIR"] = @"C:\Program Files (x86)\AAA";
            MsiRuntime.StartExecute();
        }

        InstallProgressCounter progressCounter = new InstallProgressCounter(0.5);
        bool started = false;
        bool canceled = false;

        public ManagedDialogs Dialogs { get; set; }

        IManagedDialog currentDialog;
        Form shellView;

        int currentViewIndex = -1;
        public int CurrentDialogIndex
        {
            get { return currentViewIndex; }

            set
            {
                currentViewIndex = value;

                try
                {
                    shellView.ClearChildren();

                    if (currentViewIndex >= 0 && currentViewIndex < Dialogs.Count)
                    {
                        Type viewType = Dialogs[currentViewIndex];

                        var view = (Form)Activator.CreateInstance(viewType);
                        view.LocalizeFrom(MsiRuntime.Localize);
                        view.FormBorderStyle = forms.FormBorderStyle.None;
                        view.TopLevel = false;
                        view.Dock = DockStyle.Fill;

                        currentDialog = (IManagedDialog)view;
                        currentDialog.Shell = this;

                        view.Parent = shellView;
                        view.Visible = true;
                        shellView.Text = view.Text;
                    }
                }
                catch { }
            }
        }

        public void ShowModal(MsiRuntime msiRuntime, IManagedUI ui)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            shellView = new ShellView();

            UI = ui;
            MsiRuntime = msiRuntime;

            if (MsiRuntime.Session.IsInstalling())
            {
                Dialogs = ui.InstallDialogs;
            }
            else if (MsiRuntime.Session.IsRepairing())
            {
                Dialogs = ui.RepairDialogs;
            }

            GoNext();

            shellView.ShowDialog();
        }

        public void GoNext()
        {
            CurrentDialogIndex++;
        }

        public void GoPrev()
        {
            CurrentDialogIndex--;
        }

        public void GoTo(int index)
        {
            CurrentDialogIndex = index;
        }

        public void Exit()
        {
            shellView.Close();
        }

        public void Cancel()
        {
            canceled = true;
            if (!started)
                Exit();
        }

        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            try
            {
                this.progressCounter.ProcessMessage(messageType, messageRecord);

                currentDialog.OnProgress((int)Math.Round(100 * this.progressCounter.Progress));

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

            var result = currentDialog.ProcessMessage(messageType, messageRecord, buttons, icon, defaultButton);
            Application.DoEvents();
            return result;
        }

        StringBuilder log = new StringBuilder();
        public string Log { get { return log.ToString(); } }

        void LogMessage(string message, params object[] args)
        {
            log.AppendLine(message.FormatInline(args));
        }

        public void OnExecuteStarted()
        {
            currentDialog.OnExecuteStarted();
        }

        public void OnExecuteComplete()
        {
            currentDialog.OnExecuteComplete();
        }

        public void InUIThread(System.Action action)
        {
            shellView.Invoke(action);
        }
    }
}
