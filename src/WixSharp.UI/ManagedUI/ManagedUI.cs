using System;
using System.Linq;
using System.Threading;
using Microsoft.Deployment.WindowsInstaller;

namespace WixSharp
{
    public class EmbeddedUI : IManagedUI, IEmbeddedUI
    {
        ManagedDialogs beforeInstall = new ManagedDialogs();
        public ManagedDialogs BeforeInstall { get { return beforeInstall; } }

        ManagedDialogs afterInstall = new ManagedDialogs();
        public ManagedDialogs AfterInstall { get { return afterInstall; } }

        ManagedDialogs beforeUninstall = new ManagedDialogs();
        public ManagedDialogs BeforeUninstall { get { return beforeUninstall; } }

        ManagedDialogs afterUninstall = new ManagedDialogs();
        public ManagedDialogs AfterUninstall { get { return afterUninstall; } }

        ManagedDialogs beforeRepair = new ManagedDialogs();
        public ManagedDialogs BeforeRepair { get { return beforeRepair; } }

        ManagedDialogs afterRepair = new ManagedDialogs();
        public ManagedDialogs AfterRepair { get { return afterRepair; } }

        public EmbeddedUIShell Shell;
        /// <summary>
        /// The predefined ManagedUI. It contains the dialog sequence similar to WixUI_Mondo. 
        /// </summary>
        static public EmbeddedUI Default = new EmbeddedUI();

        public void BindTo(ManagedProject project)
        {
        }

        public void UnbindFrom(ManagedProject project)
        {
        }

        ManualResetEvent installStartEvent = new ManualResetEvent(false);
        ManualResetEvent installExitEvent = new ManualResetEvent(false);
        Thread uiThread;

        public bool Initialize(Session session, string resourcePath, ref InstallUIOptions uiLevel)
        {
            if (session != null && (session.IsUninstall() || uiLevel.IsBasic()))
                return false;

            uiThread = new Thread(ShowUI);
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();

            // Wait for the setup wizard to either kickoff the install or prematurely exit.
            int waitResult = WaitHandle.WaitAny(new[] { installStartEvent, installExitEvent });
            if (waitResult == 1)
            {
                // The setup wizard set the exit event instead of the start event. Cancel the installation.
                throw new InstallCanceledException();
            }
            else
            {
                // Start the installation with a silenced internal UI.
                // This "embedded external UI" will handle message types except for source resolution.
                uiLevel = InstallUIOptions.NoChange | InstallUIOptions.SourceResolutionOnly;
                return true;
            }
        }

        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord,
           MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            // Synchronously send the message to the setup wizard window on its thread.
            //object result = this.setupWizard.Dispatcher.Invoke(DispatcherPriority.Send,
            //    new Func<MessageResult>(delegate()
            //    {
            //        return this.setupWizard.ProcessMessage(messageType, messageRecord, buttons, icon, defaultButton);
            //    }));
            //return (MessageResult)result;
            return MessageResult.OK;
        }

        public void Shutdown()
        {
            // Wait for the user to exit the setup wizard.
            //this.setupWizard.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //    new Action(delegate()
            //    {
            //        this.setupWizard.EnableExit();
            //    }));
            uiThread.Join();
        }

        void ShowUI()
        {

            installExitEvent.Set();
        }

    }

}