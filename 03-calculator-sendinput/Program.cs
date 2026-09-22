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

            // Wait for main window handle
            int tries = 0;
            while (proc.MainWindowHandle == IntPtr.Zero && tries++ < 20)
            {
                Thread.Sleep(200);
                proc.Refresh();
            }

            if (proc.MainWindowHandle != IntPtr.Zero)
            {
                NativeFunctions.SetForegroundWindow(proc.MainWindowHandle);
                Thread.Sleep(200);

                // Send 4 + 3 Enter to calculator using SetInput helper
                NativeFunctions.SetInput(VirtualKeys.VK_4);
                Thread.Sleep(200);
                NativeFunctions.SetInput(VirtualKeys.VK_OEM_PLUS);
                Thread.Sleep(200);
                NativeFunctions.SetInput(VirtualKeys.VK_3);
                Thread.Sleep(200);
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
}
