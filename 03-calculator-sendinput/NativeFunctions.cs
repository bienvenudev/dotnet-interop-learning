using System;
using System.Runtime.InteropServices;

static class NativeFunctions
{
    public const int INPUT_KEYBOARD = 1;
    public const uint KEYEVENTF_KEYUP = 0x0002;

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

    public static void SetInput(ushort vk)
    {
        INPUT[] inputs = new INPUT[2];

        // Key down
        inputs[0].type = INPUT_KEYBOARD;
        inputs[0].U.ki.wVk = vk;
        inputs[0].U.ki.wScan = 0;
        inputs[0].U.ki.dwFlags = 0;
        inputs[0].U.ki.time = 0;
        inputs[0].U.ki.dwExtraInfo = IntPtr.Zero;

        // Key up
        inputs[1].type = INPUT_KEYBOARD;
        inputs[1].U.ki.wVk = vk;
        inputs[1].U.ki.wScan = 0;
        inputs[1].U.ki.dwFlags = KEYEVENTF_KEYUP;
        inputs[1].U.ki.time = 0;
        inputs[1].U.ki.dwExtraInfo = IntPtr.Zero;

        var result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        if (result != inputs.Length)
        {
            // swallow error for demo; in production you'd log Marshal.GetLastWin32Error()
        }

    }

    // Find a top-level visible window that belongs to the given process id
    public static IntPtr FindMainWindowForProcess(int processId)
    {
        IntPtr found = IntPtr.Zero;

        EnumWindows((hwnd, lParam) =>
        {
            if (!IsWindowVisible(hwnd))
                return true; // continue

            GetWindowThreadProcessId(hwnd, out uint pid);
            if ((int)pid == processId)
            {
                found = hwnd;
                return false; // stop enumeration
            }

            return true; // continue
        }, IntPtr.Zero);

        return found;
    }

    // Search all top-level windows and return the first visible one whose title contains 'part'
    public static IntPtr FindWindowWithTitleContaining(string part)
    {
        if (string.IsNullOrEmpty(part))
            return IntPtr.Zero;

        string lower = part.ToLowerInvariant();
        IntPtr found = IntPtr.Zero;

        EnumWindows((hwnd, lParam) =>
        {
            if (!IsWindowVisible(hwnd))
                return true; // continue

            int len = GetWindowTextLength(hwnd);
            if (len <= 0)
                return true;

            var sb = new System.Text.StringBuilder(len + 1);
            GetWindowText(hwnd, sb, sb.Capacity);
            var text = sb.ToString();
            if (!string.IsNullOrEmpty(text) && text.ToLowerInvariant().Contains(lower))
            {
                found = hwnd;
                return false; // stop
            }

            return true; // continue
        }, IntPtr.Zero);

        return found;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public int type;
        public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }
}

static class VirtualKeys
{
    public const ushort VK_0 = 0x30;
    public const ushort VK_1 = 0x31;
    public const ushort VK_2 = 0x32;
    public const ushort VK_3 = 0x33;
    public const ushort VK_4 = 0x34;
    public const ushort VK_5 = 0x35;
    public const ushort VK_6 = 0x36;
    public const ushort VK_7 = 0x37;
    public const ushort VK_8 = 0x38;
    public const ushort VK_9 = 0x39;

    public const ushort VK_OEM_PLUS = 0x6B; // '+' key (depends on layout)
    public const ushort VK_RETURN = 0x0D;
}



