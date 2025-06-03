using System.Runtime.InteropServices;

namespace RecallShield.Services;

public class ScreenshotBlockerService
{
    private bool _isEnabled = true;
    private IntPtr _hookHandle = IntPtr.Zero;
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int VK_SNAPSHOT = 0x2C;
    private const int VK_LWIN = 0x5B;
    private const int VK_RWIN = 0x5C;
    private const int VK_SHIFT = 0x10;
    private const int VK_S = 0x53;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                if (value)
                    EnableScreenshotBlock();
                else
                    DisableScreenshotBlock();
            }
        }
    }

    public ScreenshotBlockerService()
    {
        EnableScreenshotBlock();
    }

    private void EnableScreenshotBlock()
    {
        if (_hookHandle == IntPtr.Zero)
        {
            _hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, HookCallback, GetModuleHandle(null), 0);
        }
    }

    private void DisableScreenshotBlock()
    {
        if (_hookHandle != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookHandle);
            _hookHandle = IntPtr.Zero;
        }
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            var keyInfo = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            
            // Block Print Screen key
            if (keyInfo.vkCode == VK_SNAPSHOT)
            {
                return (IntPtr)1;
            }

            // Block Windows + Shift + S (Snip & Sketch)
            if ((keyInfo.vkCode == VK_S) && 
                (GetAsyncKeyState(VK_LWIN) < 0 || GetAsyncKeyState(VK_RWIN) < 0) &&
                (GetAsyncKeyState(VK_SHIFT) < 0))
            {
                return (IntPtr)1;
            }
        }
        return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hInstance, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }
} 