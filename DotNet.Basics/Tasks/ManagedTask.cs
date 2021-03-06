﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using DotNet.Basics.Sys;

namespace DotNet.Basics.Tasks
{
    public abstract class ManagedTask : ITask, ILogDispatcher
    {
        public delegate void TaskStartedEventHandler(string taskName);
        public delegate void TaskEndedEventHandler(string taskName, Exception e);

        public event TaskStartedEventHandler Started;
        public event TaskEndedEventHandler Ended;
        public event Logger.MessageLoggedEventHandler MessageLogged;
        public event Logger.TimingLoggedEventHandler TimingLogged;
        public bool HasListeners => MessageLogged != null || TimingLogged != null;

        protected ManagedTask(string name = null, params string[] removeSuffixes)
        {
            Name = name ?? GetType().GetNameWithGenericsExpanded();
            foreach (var removeSuffix in removeSuffixes)
                Name = Name.RemoveSuffix(removeSuffix);
        }

        public string Name { get; }

        protected virtual void FireTimingLogged(LogLevel level, string name, string @event, TimeSpan duration)
        {
            TimingLogged?.Invoke(level, name, @event, duration);
        }

        protected virtual void FireMessageLogged(LogLevel level, string message, Exception e)
        {
            MessageLogged?.Invoke(level, message, e);
        }

        protected virtual void FireStarted(string taskName)
        {
            Started?.Invoke(taskName);
        }

        protected virtual void FireEnded(string taskName, Exception e = null)
        {
            Ended?.Invoke(taskName, e);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ManagedTask<T> : ManagedTask
    {
        private readonly Func<T, ILogger, CancellationToken, Task> _task;
        private readonly ILogger _log;

        public ManagedTask(string name, params string[] removeSuffixes)
            : this(name, (args, log, ct) => { }, removeSuffixes)
        { }

        public ManagedTask(Action syncTask, params string[] removeSuffixes)
            : this((args, log, ct) => syncTask(), removeSuffixes)
        { }

        public ManagedTask(Func<Task> asyncTask, params string[] removeSuffixes)
            : this((args, log, ct) => asyncTask(), removeSuffixes)
        { }

        public ManagedTask(Action<T, ILogger, CancellationToken> task, params string[] removeSuffixes)
            : this(null, task, removeSuffixes)
        { }

        public ManagedTask(Func<T, ILogger, CancellationToken, Task> task, params string[] removeSuffixes)
            : this(null, task, removeSuffixes)
        { }

        public ManagedTask(string name, Action<T, ILogger, CancellationToken> task, params string[] removeSuffixes)
            : this(name, (args, log, ct) =>
            {
                task?.Invoke(args, log, ct);
                return Task.FromResult(string.Empty);
            }, removeSuffixes)
        { }

        public ManagedTask(string name, Func<T, ILogger, CancellationToken, Task> task, params string[] removeSuffixes)
        : base(name, removeSuffixes)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
            _log = new Logger().InContext(Name);
            _log.MessageLogged += base.FireMessageLogged;
            _log.TimingLogged += base.FireTimingLogged;
        }

        public Task<T> RunAsync(T args)
        {
            return RunAsync(args, default);
        }

        public async Task<T> RunAsync(T args, CancellationToken ct)
        {
            FireStarted(Name);
            try
            {
                if (ct.IsCancellationRequested == false)
                    await InnerRunAsync(args, _log, ct).ConfigureAwait(false);

                FireEnded(Name);
                return args;
            }
            catch (Exception e)
            {
                FireEnded(Name, e);
                throw;
            }
        }

        protected override void FireTimingLogged(LogLevel level, string name, string @event, TimeSpan duration)
        {
            _log.Timing(level, name, @event, duration);
        }
        protected override void FireMessageLogged(LogLevel level, string message, Exception e)
        {
            _log.Write(level, message, e);
        }

        protected virtual Task InnerRunAsync(T args, ILogger log, CancellationToken ct)
        {
            return _task?.Invoke(args, log, ct);
        }
    }
}