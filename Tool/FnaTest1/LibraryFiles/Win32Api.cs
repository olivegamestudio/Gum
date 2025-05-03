//© 2021-2025 Bit Kid, Inc.
//Use at your own risk. No warranty expressed or implied!

using System;
using System.Runtime.InteropServices;

namespace WPFNA
{
    internal static class Win32Api
    {
        public struct POINT
        {
            public uint X;
            public uint Y;
        }

        public const int GWL_STYLE = (-16);

        [Flags]
        public enum WindowStyles : uint
        {
            WS_CHILD = 0x40000000,
            WS_TABSTOP = 0x00010000,
        };

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr child, IntPtr newParent);
        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
    }
}
