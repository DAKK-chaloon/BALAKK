using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WpfApp1;

namespace BALAKK
{
    internal class KeyHandler
    {
        private static LowLevelMouseProc _proc = new LowLevelMouseProc(HookCallback);
        private static IntPtr _hookID = IntPtr.Zero;
        private const int WH_MOUSE_LL = 14;

        public static event EventHandler MouseAction = (_param1, _param2) => { };

        public static void Start() => _hookID = SetHook(_proc);

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule mainModule = currentProcess.MainModule)
                    return SetWindowsHookEx(14, proc, GetModuleHandle(mainModule.ModuleName), 0U);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && 523 == (int)wParam)
            {
                MSLLHOOKSTRUCT structure = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseAction((object)null, new EventArgs());
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
          int idHook,
          LowLevelMouseProc lpfn,
          IntPtr hMod,
          uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
          IntPtr hhk,
          int nCode,
          IntPtr wParam,
          IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private enum MouseMessages
        {
            WM_MOUSEWHEELUP = 64, // 0x00000040
            WM_MOUSEMOVE = 512, // 0x00000200
            WM_LBUTTONDOWN = 513, // 0x00000201
            WM_LBUTTONUP = 514, // 0x00000202
            WM_RBUTTONDOWN = 516, // 0x00000204
            WM_RBUTTONUP = 517, // 0x00000205
            WM_MOUSEWHEELDOWN = 519, // 0x00000207
            WM_MOUSEWHEEL = 522, // 0x0000020A
        }

        private struct MSLLHOOKSTRUCT
        {
            public MainWindow.POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}
