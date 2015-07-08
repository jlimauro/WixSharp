using System;
using System.Linq;
using System.Threading;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp.CommonTasks;
using WixSharp.UI.ManagedUI;
using System.Diagnostics;

#pragma warning disable 1591

namespace WixSharp
{
    public class ManagedUI : IManagedUI, IEmbeddedUI
    {
        static public ManagedUI Default = new ManagedUI
            {
                //http://wixtoolset.org/documentation/manual/v3/wixui/dialog_reference/wixui_featuretree.html
            };

        public ManagedUI()
        {
            InstallDialogs = new ManagedDialogs();
            ModifyDialogs = new ManagedDialogs();
        }

        public string DialogBitmap;
        public string DialogBanner;

        public void EmbeddResourcesInto(ManagedProject project)
        {
            project.AddBinary(new Binary(new Id("WixSharp_UIText"), LocalizationFileFor(project)));
            project.AddBinary(new Binary(new Id("WixSharp_LicenceFile"), LicenceFileFor(project)));
            project.AddBinary(new Binary(new Id("WixUI_Bmp_Dialog"), DialogBitmapFileFor(project)));
            project.AddBinary(new Binary(new Id("WixUI_Bmp_Banner"), DialogBannerFileFor(project)));
        }

        string LocalizationFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(project.LocalizationFile, project.OutDir, project.Name + ".wxl", Resources.WixUI_en_us);
        }

        string LicenceFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(project.LicenceFile, project.OutDir, project.Name + ".licence.rtf", Resources.WixSharp_LicenceFile);
        }

        string DialogBitmapFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(DialogBitmap, project.OutDir, project.Name + ".dialog_bmp.png", Resources.WixUI_Bmp_Dialog);
        }

        string DialogBannerFileFor(Project project)
        {
            return UIExtensions.UserOrDefaultContentOf(DialogBanner, project.OutDir, project.Name + ".dialog_banner.png", Resources.WixUI_Bmp_Banner);
        }

        public ManagedDialogs InstallDialogs { get; set; }
        public ManagedDialogs ModifyDialogs { get; set; }

        ManualResetEvent uiExitEvent = new ManualResetEvent(false);
        IUIContainer shell;

        void ReadDialogs(Session session)
        {
            InstallDialogs.Clear()
                          .AddRange(ManagedProject.ReadDialogs(session.Property("WixSharp_InstallDialogs")));

            ModifyDialogs.Clear()
                          .AddRange(ManagedProject.ReadDialogs(session.Property("WixSharp_ModifyDialogs")));

        }

        public bool Initialize(Session session, string resourcePath, ref InstallUIOptions uiLevel)
        {
            if (session != null && (session.IsUninstalling() || uiLevel.IsBasic()))
                return false; //use built-in MSI basic UI

            ReadDialogs(session);

            var startEvent = new ManualResetEvent(false);

            var uiThread = new Thread(() =>
            {

                shell = new UIShell(); //important to create the instance in the same thread that call ShowModal
                shell.ShowModal(new MsiRuntime(session) { StartExecute = () => startEvent.Set() }, this);
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
                shell.OnExecuteStarted();
                return true;
            }
        }

        public MessageResult ProcessMessage(InstallMessage messageType, Record messageRecord, MessageButtons buttons, MessageIcon icon, MessageDefaultButton defaultButton)
        {
            return shell.ProcessMessage(messageType, messageRecord, buttons, icon, defaultButton);
        }

        public void Shutdown()
        {
            shell.OnExecuteComplete();
            uiExitEvent.WaitOne();
        }
    }
}
