using System;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        EnumWindows((hWnd, lParam) =>
        {
            try
            {
                // Only consider visible top-level windows
                if (!IsWindowVisible(hWnd))
                    return true; // continue enumeration

                int length = GetWindowTextLength(hWnd);
                if (length <= 0)
                    return true; // no title -> skip

                var sb = new StringBuilder(length + 1);
                GetWindowText(hWnd, sb, sb.Capacity);
                string title = sb.ToString();

                if (!string.IsNullOrWhiteSpace(title))
                {
                    // Print handle (hex) and title
                    long handleValue = hWnd.ToInt64() & 0xFFFFFFFF;
                    Console.WriteLine($"{handleValue:X}: {title}");
                }
            }
            catch (Exception ex)
            {
                // Don't crash enumeration for unexpected errors
                Console.Error.WriteLine("Error reading window: " + ex.Message);
            }

            return true; // continue enumeration
        }, IntPtr.Zero);
    }
}
