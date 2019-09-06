﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using DotNet.Basics.Diagnostics;

namespace DotNet.Basics.Cli.ConsoleOutput
{
    public class AnsiConsoleWriter : ConsoleWriter
    {
        private readonly object _syncRoot = new object();
        private readonly ConsoleTheme _consoleTheme;
        private static readonly AnsiForegroundColor _gutterColor = new AnsiForegroundColor(Color.DarkGray);

        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public AnsiConsoleWriter(ConsoleTheme consoleTheme = null)
        {
            _consoleTheme = consoleTheme ?? ConsoleTheme.Default;
        }

        public static bool IsSupported
        {
            get
            {
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
                    return false;

                outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                return SetConsoleMode(iStdOut, outConsoleMode);
            }
        }

        private string WriteOutput(LogLevel level, string message, Exception e = null)
        {
            var format = _consoleTheme.Get(level);

            var outputBuilder = new StringBuilder();
            outputBuilder.Append($"[{ToOutputString(level)}]".AnsiColorize(format));
            outputBuilder.Append(" ");
            outputBuilder.Append($"{message.AnsiColorize(format)}\r\n{e?.ToString().AnsiColorize(_gutterColor)}");
            return outputBuilder.ToString();
        }


        public override void Write(LogLevel level, string message, Exception e = null)
        {
            lock (_syncRoot)
            {
                var output = WriteOutput(level, message, e);
                Console.Out.Write(output);
                Console.Out.Flush();
            }
        }
    }
}
