using System;
using System.Diagnostics;
using System.Linq;

namespace WixSharp
{
    //TODO:
    //".NET presence" launch condition
    //ensure UI event handlers are fired the last
    public class ManagedUI : IManagedUI
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

        public UIShell Shell;
        /// <summary>
        /// The predefined ManagedUI. It contains the dialog sequence similar to WixUI_Mondo. 
        /// </summary>
        static public ManagedUI Default = new ManagedUI();

        virtual public void BindTo(ManagedProject project)
        {
            project.UI = WUI.WixUI_ProgressOnly;
            project.Load += OnLoad;
            project.BeforeInstall += OnBeforeInstall;
            project.AfterInstall += OnAfterInstall;
        }

        public void UnbindFrom(ManagedProject project)
        {
            project.Load -= OnLoad;
            project.BeforeInstall -= OnBeforeInstall;
            project.AfterInstall -= OnAfterInstall;
        }

        void OnLoad(SetupEventArgs e)
        {
            //Debugger.Launch();
            if (!e.IsUISupressed && e.HasBeforeSetupClrDialogs)
            {
                e.MsiWindow.Hide();

                using (var form = new UIShell(e))
                {
                    form.Load += (s, x) => form.MoveToMiddleOf(e.MsiWindow);
                    form.ShowDialogSequence(e.BeforeSetupClrDialogs);

                    e.MsiWindow.MoveToMiddleOf(form);
                }
            }
        }

        void OnBeforeInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && e.HasBeforeSetupClrDialogs)
                e.MsiWindow.Show();
        }

        void OnAfterInstall(SetupEventArgs e)
        {
            //Debugger.Launch();
            if (!e.IsUISupressed && e.HasAfterSetupClrDialogs)
            {
                e.MsiWindow.Hide();

                using (var form = new UIShell(e))
                {
                    form.Load += (s, x) => form.MoveToMiddleOf(e.MsiWindow);
                    form.ShowDialogSequence(e.AfterInstallClrDialogs);
                }
            }
        }
    }
}