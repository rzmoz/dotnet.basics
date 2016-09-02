﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks
{
    public class SyncTask : SyncTask<TaskOptions>
    {
        public SyncTask(Action task, TaskOptions options = null) : base(task, options)
        {
        }
    }
    public class SyncTask<T> : RunTask<T> where T : TaskOptions, new()
    {
        private readonly Action _syncTask;
        private readonly Func<Task> _asyncTask;

        public SyncTask(Action task, T options = default(T)) : base(options)
        {
            _syncTask = task;
            _asyncTask = () => { task.Invoke(); return Task.CompletedTask; };
        }

        public override void Run()
        {
            _syncTask.Invoke();
        }

        public override async Task RunAsync(CancellationToken ct = new CancellationToken())
        {
            await _asyncTask.Invoke().ConfigureAwait(false);
        }
    }
}
