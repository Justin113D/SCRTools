using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SCR.Tools.WPF.Utility
{
    public static class ScreenBounds
    {
        private const int MONITOR_DEFAULTTONULL = 0x00000000;
        private const int MONITOR_DEFAULTTOPRIMARY = 0x00000001;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const int MONITORINFOF_PRIMARY = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        private class tagMONITORINFO
        {
            public int cbSize = 72;
            public RECT rcMonitor = default;
            public RECT rcWork = default;
            public int dwFlags = 0;

        }

        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int flags);

        [DllImport("User32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out] tagMONITORINFO flags);

        public static bool GetScreenWorkWidthHeight(Window window, out double width, out double height)
        {
            width = 0;
            height = 0;

            IntPtr handle = new WindowInteropHelper(window).Handle;
            IntPtr monitorHandle = MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);
            if (monitorHandle == IntPtr.Zero)
            {
                return false;
            }

            tagMONITORINFO monitorInfo = new();
            if (!GetMonitorInfo(monitorHandle, monitorInfo))
            {
                return false;
            }

            RECT workArea = monitorInfo.rcWork;

            width = workArea.right - workArea.left;
            height = workArea.bottom - workArea.top;

            return true;
        }
    }
}
