// ===========================================================================
// OptalCP C# API - Internal Utilities
// Version: 2026.4.0
// ===========================================================================

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace OptalCP
{
    /// <summary>
    /// Shared utilities for solver communication.
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Detect if terminal supports colors.
        /// </summary>
        /// <param name="outputWriter">The output writer to check (e.g. Console.Out).</param>
        public static bool CanUseColors(TextWriter? outputWriter)
        {
            // Respect NO_COLOR standard: https://no-color.org/
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR")))
                return false;

            // Force colors if requested (useful for CI/CD, logging)
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FORCE_COLOR")))
                return true;

            // No stream = no colors
            if (outputWriter == null)
                return false;

            // Check for Jupyter/Interactive environment
            // In .NET Interactive / Jupyter, Console.IsOutputRedirected is theoretically true,
            // but we want colors. There isn't a single standard variable, but we can check specifically.
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_INTERACTIVE_CLI_TELEMETRY_OPTOUT")) || 
                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_JUPYTER_PORT")))
            {
               return true;
            }

            // Check if output stream is a TTY (traditional terminal check)
            // Note: We can only reliably check TTY for the standard console output
            if (outputWriter == Console.Out && !Console.IsOutputRedirected)
            {
                // On Windows, try to enable ANSI support
                if (OperatingSystem.IsWindows())
                    return EnableWindowsAnsi();
                return true;
            }

            // No color support detected
            return false;
        }

        private static bool EnableWindowsAnsi()
        {
            try
            {
                var handle = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
                if (NativeMethods.GetConsoleMode(handle, out var mode))
                {
                    mode |= NativeMethods.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                    return NativeMethods.SetConsoleMode(handle, mode);
                }
            }
            catch
            {
                // Ignore errors
            }
            return false;
        }

        private static class NativeMethods
        {
            internal const int STD_OUTPUT_HANDLE = -11;
            internal const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);

            [DllImport("kernel32.dll")]
            internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

            [DllImport("kernel32.dll")]
            internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        }
    }
}
