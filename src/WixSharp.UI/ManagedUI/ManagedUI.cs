using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System;

namespace WixSharp
{

    public class ManagedUI : IManagedUI, IEmbeddedUI
    {
        static public ManagedUI Default = new ManagedUI
            {
            };

        public ManagedUI()
        {
            InstallDialogs = new ManagedDialogs();
            RepairDialogs = new ManagedDialogs();
        }

        public ManagedDialogs InstallDialogs { get; set; }
        public ManagedDialogs RepairDialogs { get; set; }

        ManualResetEvent uiExitEvent = new ManualResetEvent(false);
        IUIShell shell;

        void ReadDialogs(Session session)
        {
            InstallDialogs.Clear()
                          .AddRange(ManagedProject.ReadDialogs(session.Property("WixSharp_InstallDialogs")));

            RepairDialogs.Clear()
                          .AddRange(ManagedProject.ReadDialogs(session.Property("WixSharp_RepairDialogs")));

        }

        public bool Initialize(Session session, string resourcePath, ref InstallUIOptions uiLevel)
        {
            //Debugger.Launch();
            if (session != null && (session.IsUninstalling() || uiLevel.IsBasic()))
                return false; //use built-in MSI basic UI

            ReadDialogs(session);

            var startEvent = new ManualResetEvent(false);

            var uiThread = new Thread(() =>
            {
                shell = new UIShell(); //important to create the instance in the same thread that call ShowModal
                shell.ShowModal(new MsiRuntime(session){ StartExecute = () => startEvent.Set() }, this);
                uiExitEvent.Set();
            });

            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();

            int waitResult = WaitHandle.WaitAny(new[] { startEvent, uiExitEvent });
            if (waitResult == 1)
            {
                //UI exited without starting the install. Cancel the installation.
                throw new InstallCanceledException();
            }
            else
            {
                // Start the installation with a silenced internal UI.
                // This "embedded external UI" will handle message types except for source resolution.
                uiLevel = InstallUIOptions.NoChange | InstallUIOptions.SourceResolutionOnly;
                shell.InUIThread(shell.OnExecuteStarted);
                return true;
            }
        }

        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            MessageResult result = MessageResult.OK;
            shell.InUIThread(() =>
                {
                    result = shell.ProcessMessage(messageType, messageRecord, buttons, icon, defaultButton);
                });
            return result;
        }

        public void Shutdown()
        {
            shell.OnExecuteComplete();
            uiExitEvent.WaitOne();
        }
    }
}
