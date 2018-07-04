using System;
using System.Runtime.InteropServices;

namespace ZombiesAutosplitter
{
    public static class User32Helper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
        UInt32 dwDesiredAccess,
        Int32 bInheritHandle,
        UInt32 dwProcessId
        );

        [DllImport("kernel32.dll")]
        public static extern Int32 ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [In, Out] byte[] buffer,
            UInt32 size,
            out IntPtr lpNumberOfBytesRead
            );
    }
}
