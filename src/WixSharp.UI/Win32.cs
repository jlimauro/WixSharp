using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;

namespace WixSharp
{
    internal class Win32
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        [DllImport("user32", EntryPoint = "SendMessage")]
        public extern static int SendMessage(IntPtr hwnd, uint msg, uint wParam, uint lParam);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

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

        [DllImport("user32")]
        public static extern int SendMessage(int hwnd, int msg, int wparam, int lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

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
}
