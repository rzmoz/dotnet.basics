﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Basics.Diagnostics
{
    public class Logger : ILogger
    {
        public delegate void MessageLoggedEventHandler(LogLevel level, string message, Exception e);
        public delegate void TimingLoggedEventHandler(LogLevel level, string name, string @event, TimeSpan duration);
        public event MessageLoggedEventHandler MessageLogged;
        public event TimingLoggedEventHandler TimingLogged;
        public bool HasListeners => MessageLogged != null || TimingLogged != null;
        
        /// <summary>
        /// Logs to nothing and doesn't raise events
        /// </summary>
        public static ILogger NullLogger { get; } = new NullLogger();

        private readonly ConcurrentStack<string> _context;

        public string Context { get; }

        public Logger(IEnumerable<string> context)
        : this((context ?? new string[0]).ToArray())
        {
        }
        public Logger(params string[] context)
        {
            _context = new ConcurrentStack<string>(context);
            if (_context.Any())
                Context = string.Join(" / ", _context.Reverse()) + " / ";
            else
                Context = string.Empty;
        }

        public ILogger AddLogTarget(ILogTarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (target.LogTarget != null)
                MessageLogged += target.LogTarget.Invoke;
            if (target.TimingTarget != null)
                TimingLogged += target.TimingTarget.Invoke;
            return this;
        }

        public virtual ILogger InContext(string context, bool floatMessageLogged = true)
        {
            var newLogger = string.IsNullOrWhiteSpace(context)
                ? new Logger(_context)
                : new Logger(_context.Append(context));
            if (floatMessageLogged)
            {
                newLogger.MessageLogged += (lvl, msg, e) => MessageLogged?.Invoke(lvl, msg, e);
                newLogger.TimingLogged += (lvl, name, @event, duration) => TimingLogged?.Invoke(lvl, name, @event, duration);
            }

            return newLogger;
        }
        public void Raw(string message)
        {
            Raw(message, null);
        }
        public void Raw(string message, Exception e)
        {
            Write(LogLevel.Raw, message, e);
        }
        public void Verbose(string message)
        {
            Verbose(message, null);
        }
        public void Verbose(string message, Exception e)
        {
            Write(LogLevel.Verbose, message, e);
        }
        public void Debug(string message)
        {
            Debug(message, null);
        }
        public void Debug(string message, Exception e)
        {
            Write(LogLevel.Debug, message, e);
        }
        public void Info(string message)
        {
            Info(message, null);
        }
        public void Info(string message, Exception e)
        {
            Write(LogLevel.Info, message, e);
        }
        public void Success(string message)
        {
            Success(message, null);
        }
        public void Success(string message, Exception e)
        {
            Write(LogLevel.Success, message, e);
        }
        public void Warning(string message)
        {
            Warning(message, null);
        }
        public void Warning(string message, Exception e)
        {
            Write(LogLevel.Warning, message, e);
        }
        public void Error(string message)
        {
            Error(message, null);
        }
        public void Error(string message, Exception e)
        {
            Write(LogLevel.Error, message, e);
        }
        public void Write(LogLevel level, string message)
        {
            Write(level, message, null);
        }
        public virtual void Write(LogLevel level, string message, Exception e)
        {
            MessageLogged?.Invoke(level, level == LogLevel.Raw ? message : $"{Context}{message}", e);
        }

        public void Timing(LogLevel level, string name, string @event)
        {
            Timing(level, name, @event, TimeSpan.MinValue);
        }

        public void Timing(LogLevel level, string name, string @event, TimeSpan duration)
        {
            TimingLogged?.Invoke(level, name, @event, duration);
        }

        public override string ToString()
        {
            return $"{nameof(Context)}: {Context}";
        }
    }
}
