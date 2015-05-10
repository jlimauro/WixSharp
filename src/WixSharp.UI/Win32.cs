using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;

namespace WixSharp
{
#pragma warning disable 1591
    /// <summary>
    /// Set of Win32 API wrappers
    /// </summary>
    public class Win32
    {
        const int SW_HIDE = 0;
        const int SW_SHOW = 1;
        internal const int SW_RESTORE = 9;

        
        [DllImport("user32", EntryPoint = "SendMessage")]
        public extern static int SendMessage(IntPtr hwnd, uint msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static string GetWindowText(IntPtr wnd)
        {
            int length = GetWindowTextLength(wnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(wnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public static bool ShowWindow(IntPtr hWnd, bool show)
        {
            return ShowWindow(hWnd, show ? SW_SHOW : SW_HIDE);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        public static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
#pragma warning restore 1591
}
