using System;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows;
using System.ComponentModel;

namespace iCopy.Services
{
    public class KeyboardService
    {
        private static KeyboardService _instance;
        public static KeyboardService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new KeyboardService();
                }
                return _instance;
            }
        }

        public event Action<Key> KeyPressed;

        // Windows API 常量
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        // 委托
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private KeyboardService()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    Key key = KeyInterop.KeyFromVirtualKey(vkCode);

                    // 安全地调用事件
                    var app = Application.Current;
                    if (app != null)
                    {
                        app.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                KeyPressed?.Invoke(key);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error in KeyPressed event handler: {ex.Message}");
                            }
                        });
                    }
                    else
                    {
                        // 如果Application.Current为null，直接调用事件
                        try
                        {
                            KeyPressed?.Invoke(key);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error in KeyPressed event handler: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in HookCallback: {ex.Message}");
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void Cleanup()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }
    }
}
