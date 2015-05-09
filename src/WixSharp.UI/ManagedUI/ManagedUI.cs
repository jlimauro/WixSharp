using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WixSharp
{
    //TODO:
    //on-screen positioning
    //".NET presence" launch condition
    //ensure UI event handlers are fired the last
    public class ManagedUI
    {
        public static void AttachTo(ManagedProject project)
        {
            project.Load += OnLoad;
            project.BeforeInstall += OnBeforeInstall;
            project.AfterInstall += OnAfterInstall;
        }

        static void OnLoad(SetupEventArgs e)
        {
            HideWindow(e.MsiWindow);

            new Form()
            {
                Text = "Managed Setup - " + e.Type,
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            }
            .ShowDialog();
        }

        static void OnBeforeInstall(SetupEventArgs e)
        {
            ShowWindow(e.MsiWindow);
        }

        static void OnAfterInstall(SetupEventArgs e)
        {
            HideWindow(e.MsiWindow);
            new Form()
            {
                Text = "Managed Setup - " + e.Type + " Exit",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterScreen
            }
            .ShowDialog();
        }

        static void HideWindow(IntPtr wnd)
        {
            ShowWindow(wnd, SW_HIDE);
        }

        static void ShowWindow(IntPtr wnd)
        {
            ShowWindow(wnd, SW_SHOW);
        }

        const int SW_HIDE = 0;
        const int SW_SHOW = 1;

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}