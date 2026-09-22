using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using static NativeFunctions;

class Program
{
    static void Main()
    {
        Console.WriteLine($"INPUT size = {Marshal.SizeOf(typeof(INPUT))}, KEYBDINPUT size = {Marshal.SizeOf(typeof(KEYBDINPUT))}");

        // Launch calculator
        var proc = Process.Start("calc.exe");

        if (proc != null)
        {
            // Give the process a moment to create a window
            Thread.Sleep(800);
            proc.Refresh();

            // Wait for main window handle (process.MainWindowHandle may be 0 for modern UWP apps)
            int tries = 0;
            while (proc.MainWindowHandle == IntPtr.Zero && tries++ < 20)
            {
                Thread.Sleep(200);
                proc.Refresh();
            }

            IntPtr hwnd = proc.MainWindowHandle;

            // Fallback: try to find a top-level window with "Calculator" in the title
            if (hwnd == IntPtr.Zero)
            {
                hwnd = FindWindowWithTitleContaining("Calculator");
            }

            if (hwnd != IntPtr.Zero)
            {
                NativeFunctions.SetForegroundWindow(hwnd);
                Thread.Sleep(200);

                // Send 4 + 3 Enter to calculator using SetInput helper
                NativeFunctions.SetInput(VirtualKeys.VK_4);
                Thread.Sleep(1000);
                NativeFunctions.SetInput(VirtualKeys.VK_OEM_PLUS);
                Thread.Sleep(1000);
                NativeFunctions.SetInput(VirtualKeys.VK_3);
                Thread.Sleep(1000);
                NativeFunctions.SetInput(VirtualKeys.VK_RETURN);

                Console.WriteLine("All done!");
            }
            else
            {
                Console.WriteLine("Calculator launched but no main window handle found.");
            }
        }
        else
        {
            Console.WriteLine("Failed to start calculator.");
        }
    }

    // Win32 helpers to find a top-level window by title substring
    private static IntPtr FindWindowWithTitleContaining(string part)
    {
        IntPtr found = IntPtr.Zero;
        part = part?.ToLowerInvariant() ?? string.Empty;

        EnumWindows((hwnd, lParam) =>
        {
            if (IsWindowVisible(hwnd))
            {
                int length = GetWindowTextLength(hwnd);
                if (length > 0)
                {
                    var sb = new System.Text.StringBuilder(length + 1);
                    GetWindowText(hwnd, sb, sb.Capacity);
                    var text = sb.ToString();
                    if (!string.IsNullOrEmpty(text) && text.ToLowerInvariant().Contains(part))
                    {
                        found = hwnd;
                        return false; // stop enumeration
                    }
                }
            }
            return true; // continue
        }, IntPtr.Zero);

        return found;
    }

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

}