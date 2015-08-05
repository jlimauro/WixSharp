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

namespace WixSharp
{
    /// <summary>
    /// Interface of the main window implementation of the MSI external/embedded UI. This interface is designed to be 
    /// used by Wix#/MSI runtime (e.g. ManagedUI). It is the interface that is directly bound to the 
    /// <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI"/> (e.g. <see cref="T:WixSharp.ManagedUI"/>). 
    /// </summary>
    interface IUIContainer
    {
        /// <summary>
        /// Shows the modal window of the MSI UI. This method is called by the <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI"/>
        /// when it is initialized at runtime.
        /// </summary>
        /// <param name="msiRuntime">The MSI runtime.</param>
        /// <param name="ui">The MSI external/embedded UI.</param>
        void ShowModal(MsiRuntime msiRuntime, IManagedUI ui);
        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        void OnExecuteComplete();
        /// <summary>
        /// Called when MSI execute started.
        /// </summary>
        void OnExecuteStarted();

        /// <summary>
        ///  Processes information and progress messages sent to the user interface.
        /// <para> This method directly mapped to the 
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI.ProcessMessage"/>.</para>
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
        MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton);
    }


    /// <summary>
    /// The main window WinForms implementation of the MSI external/embedded UI. 
    /// </summary>
    public partial class UIShell : IUIContainer, IManagedUIShell
    {
        /// <summary>
        /// Gets the runtime context object. Typically this object is of the <see cref="T:WixSharp.MsiRuntime" /> type.
        /// </summary>
        /// <value>
        /// The runtime context.
        /// </value>
        public object RuntimeContext { get { return MsiRuntime; } }

        internal MsiRuntime MsiRuntime { get; set; }
        internal IManagedUI UI { get; set; }

        /// <summary>
        /// Gets a value indicating whether the MSI session was interrupted (canceled) by user.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it was user interrupted; otherwise, <c>false</c>.
        /// </value>
        public bool UserInterrupted { get; private set; }
        /// <summary>
        /// Gets a value indicating whether MSI session ended with error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if error was detected; otherwise, <c>false</c>.
        /// </value>
        public bool ErrorDetected { get; private set; }

        /// <summary>
        /// Starts the execution of the MSI installation.
        /// </summary>
        public void StartExecute()
        {
            started = true;
            MsiRuntime.StartExecute();
        }

        InstallProgressCounter progressCounter = new InstallProgressCounter(0.5);
        bool started = false;
        bool canceled = false;
        bool finished = false;

        /// <summary>
        /// Gets the sequence of the UI dialogs specific for the current setup type (e.g. install vs. modify).
        /// </summary>
        /// <value>
        /// The dialogs.
        /// </value>
        public ManagedDialogs Dialogs { get; set; }

        IManagedDialog currentDialog;
        Form shellView;

        int currentViewIndex = -1;
        internal int CurrentDialogIndex
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
                        view.LocalizeWith(MsiRuntime.Localize);
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

        /// <summary>
        /// Shows the modal window of the MSI UI. This method is called by the <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI" />
        /// when it is initialized at runtime.
        /// </summary>
        /// <param name="msiRuntime">The MSI runtime.</param>
        /// <param name="ui">The MSI external/embedded UI.</param>
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

        /// <summary>
        /// Proceeds to the next UI dialog.
        /// </summary>
        public void GoNext()
        {
            CurrentDialogIndex++;
        }

        /// <summary>
        /// Moves to the previous UI Dialog.
        /// </summary>
        public void GoPrev()
        {
            CurrentDialogIndex--;
        }

        /// <summary>
        /// Moves to the UI Dialog by the specified index in the <see cref="T:WixSharp.IManagedUIShell.Dialogs" /> sequence.
        /// </summary>
        /// <param name="index">The index.</param>
        public void GoTo(int index)
        {
            CurrentDialogIndex = index;
        }

        /// <summary>
        /// Exits this MSI UI application.
        /// </summary>
        public void Exit()
        {
            shellView.Close();
        }

        /// <summary>
        /// Cancels the MSI installation.
        /// </summary>
        public void Cancel()
        {
            canceled = true;
            if (!started)
                Exit();
        }

        /// <summary>
        /// Processes information and progress messages sent to the user interface.
        /// <para> This method directly mapped to the
        /// <see cref="T:Microsoft.Deployment.WindowsInstaller.IEmbeddedUI.ProcessMessage" />.</para>
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="messageRecord">The message record.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultButton">The default button.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Gets the MSI log text.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public string Log { get { return log.ToString(); } }

        void LogMessage(string message, params object[] args)
        {
            log.AppendLine(message.FormatInline(args));
        }

        /// <summary>
        /// Called when MSI execute started.
        /// </summary>
        public void OnExecuteStarted()
        {
            //Debugger.Break();
            MsiRuntime.FetchInstallDir(); //user may have updated it

            if (currentDialog != null)
                InUIThread(currentDialog.OnExecuteStarted);
        }

        /// <summary>
        /// Called when MSI execution is complete.
        /// </summary>
        public void OnExecuteComplete()
        {
            if (currentDialog != null)
                InUIThread(currentDialog.OnExecuteComplete);
        }

        internal void InUIThread(System.Action action)
        {
            if (shellView != null)
                shellView.Invoke(action);
            else
                action();
        }
    }
}
