using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Deployment.Samples.EmbeddedUI;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.Forms;
using forms = System.Windows.Forms;
using System.Threading;

#pragma warning disable 1591

namespace WixSharp
{
    interface IUIContainer
    {
        void ShowModal(MsiRuntime msiRuntime, IManagedUI ui);
        void OnExecuteComplete();
        void OnExecuteStarted();
        MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);
    }


    public partial class UIShell : IUIContainer, IManagedUIShell
    {
        public object RuntimeContext { get { return MsiRuntime; } }
        public MsiRuntime MsiRuntime { get; set; }
        public IManagedUI UI { get; set; }

        public bool UserInterrupted { get; private set; }
        public bool ErrorDetected { get; private set; }

        public void StartExecute()
        {
            started = true;
            MsiRuntime.StartExecute();
        }

        InstallProgressCounter progressCounter = new InstallProgressCounter(0.5);
        bool started = false;
        bool canceled = false;
        bool finished = false;

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
            //Debugger.Launch();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UI = ui;
            MsiRuntime = msiRuntime;

            if (MsiRuntime.Session.IsInstalling())
            {
                Dialogs = ui.InstallDialogs;
            }
            else if (MsiRuntime.Session.IsRepairing())
            {
                Dialogs = ui.ModifyDialogs;
            }

            if (Dialogs.Any())
            {
                shellView = new ShellView();
                GoNext();
                shellView.ShowDialog();
            }
            else
            {
                this.StartExecute();
                while (!finished)
                    Thread.Sleep(1000);
            }
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

                if (currentDialog != null)
                    InUIThread(() => currentDialog.OnProgress((int)Math.Round(100 * this.progressCounter.Progress)));

                switch (messageType)
                {
                    case InstallMessage.Progress: break;
                    case InstallMessage.Error:
                    case InstallMessage.Warning:
                    case InstallMessage.Info:
                    default:
                        {
                            if (messageType == InstallMessage.Info)
                            {
                                if (messageRecord.ToString().Contains("User cancelled installation")) //there is no other way
                                    UserInterrupted = true;
                            }

                            if (messageType == InstallMessage.Error)
                                ErrorDetected = true;

                            if (messageType == InstallMessage.InstallEnd)
                            {
                                try
                                {
                                    string lastValue = messageRecord[messageRecord.FieldCount].ToString(); //MSI record is actually 1-based
                                    ErrorDetected = (lastValue == "3");
                                    UserInterrupted = (lastValue == "2");
                                }
                                catch { }//nothing we can do really
                                finished = true;
                            }

                            this.LogMessage("{0}: {1}", messageType, messageRecord);
                            break;
                        }
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

            var result = MessageResult.OK;
            InUIThread(() =>
            {
                if (currentDialog != null)
                    result = currentDialog.ProcessMessage(messageType, messageRecord, buttons, icon, defaultButton);
            });
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
            if (currentDialog != null)
                InUIThread(currentDialog.OnExecuteStarted);
        }

        public void OnExecuteComplete()
        {
            if (currentDialog != null)
                InUIThread(currentDialog.OnExecuteComplete);
        }

        public void InUIThread(System.Action action)
        {
            if (shellView != null)
                shellView.Invoke(action);
            else
                action();
        }
    }
}
