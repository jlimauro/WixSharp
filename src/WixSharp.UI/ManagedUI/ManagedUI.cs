using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WixSharp
{
    //TODO:
    //on-screen positioning
    //".NET presence" launch condition
    //ensure UI event handlers are fired the last
    public class ManagedUI : IManagedUI
    {
        ManagedDialogs beforeInstall = new ManagedDialogs();
        public ManagedDialogs BeforeInstall { get { return beforeInstall; } }

        ManagedDialogs afterInstall = new ManagedDialogs();
        public ManagedDialogs AfterInstall { get { return afterInstall; } }

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
            e.MsiWindow.Hide();

            using (var form = new UIShell(e))
            {
                form.Load += (s, x) => form.MoveToMiddleOf(e.MsiWindow);

                form.ShowBeforeInstallSequence();
                
                e.MsiWindow.MoveToMiddleOf(form);
            }
        }

        void OnBeforeInstall(SetupEventArgs e)
        {
            e.MsiWindow.Show();
        }

        void OnAfterInstall(SetupEventArgs e)
        {
            e.MsiWindow.Hide();

            using (var form = new UIShell(e))
            {
                form.Load += (s, x) => form.MoveToMiddleOf(e.MsiWindow);

                form.ShowAfterInstallSequence(); 
            }
        }
    }
}