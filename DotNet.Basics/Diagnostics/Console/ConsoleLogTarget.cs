﻿using System;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Diagnostics.Console
{
    public abstract class ConsoleLogTarget : ILogTarget
    {
        protected object SyncRoot { get; } = new object();
        private const string _space = " ";

        public Action<LogLevel, string, Exception> LogTarget => Write;
        public Action<LogLevel, string, string, TimeSpan> TimingTarget => (level, name, @event, duration) =>
        {
            var durationString = duration > TimeSpan.MinValue ? $" in {duration.ToString("hh\\:mm\\:ss").Highlight()}" : string.Empty;
            Write(level, $"[{name.Highlight()} {@event}{durationString}]".WithGutter());
        };

        public virtual void Write(LogLevel level, string message, Exception e = null)
        {
            var output = FormatLogOutput(level, message, e);
            WriteFormattedOutput(level, output);
        }
        public virtual void WriteFormattedOutput(LogLevel level, string formattedOutput)
        {
            lock (SyncRoot)
            {
                if (level < LogLevel.Error)
                    System.Console.Out.Write(formattedOutput);
                else
                    System.Console.Error.Write(formattedOutput);
                System.Console.Out.Flush();
            }
        }

        protected virtual string FormatLogOutput(LogLevel level, string message, Exception e = null)
        {
            return (FormatLogLevel(level) + FormatMessage(message, e) + Environment.NewLine);
        }

        protected virtual string FormatLogLevel(LogLevel level) =>
            level switch
            {
                LogLevel.Raw => string.Empty,
                LogLevel.Verbose => "VRB" + _space,
                LogLevel.Debug => "DBG" + _space,
                LogLevel.Info => "INF" + _space,
                LogLevel.Success => "SUC" + _space,
                LogLevel.Warning => "WRN" + _space,
                LogLevel.Error => "ERR" + _space,
                _ => $"[{level.ToName().ToUpperInvariant()}]" + _space
            };


        protected virtual string FormatMessage(string message, Exception e = null)
        {
            if (e == null || e is IConsoleException cli && cli.ConsoleLogOptions == ConsoleLogOptions.ExcludeStackTrace)
                return message;

            var exceptionMessage = e.ToString();
            return exceptionMessage.StartsWith($"{e.GetType().FullName}: {message}")
                ? e.ToString()
                : $"{message}\r\n{e}";
        }
    }
}