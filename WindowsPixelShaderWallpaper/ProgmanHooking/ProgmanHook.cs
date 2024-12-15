using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsWallpaper
{
    class ProgmanHook
    {
        private IntPtr Handle;
        private List<IntPtr> HookedHandles;

        public ProgmanHook()
        {
            HookedHandles = new List<IntPtr>();

            IntPtr Progman = U32.FindWindow("Progman", null);

            IntPtr MessageResult = IntPtr.Zero;
            U32.SendMessageTimeout(Progman,
                       0x052C,
                       new IntPtr(0),
                       IntPtr.Zero,
                       U32.SendMessageTimeoutFlags.SMTO_NORMAL,
                       1000,
                       out MessageResult);

            U32.EnumWindows(new U32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = U32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    Handle = U32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);
        }

        public void HookHandle(IntPtr childHandle)
        {
            var Result = U32.SetParent(childHandle, Handle);
            Console.WriteLine($"WorkerW hWnd: {Handle.ToString()}");
            HookedHandles.Add(childHandle);
        }
    }
}
