﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Basics.Tasks.Repeating
{
    public static class Repeat
    {
        private static readonly TaskFactory _taskFactory = new TaskFactory();

        public static RunTask<RepeatOptions> Task(Func<CancellationToken, Task> task, RepeatOptions options = null)
        {
            return _taskFactory.Create(task, options);
        }

        public static RunTask<RepeatOptions> TaskOnce(Func<CancellationToken, Task> task, RepeatOptions options = null)
        {
            var onceOnlyTask = new OnceOnlyAsyncTask(task);
            return _taskFactory.Create(onceOnlyTask.RunAsync, options);
        }

        public static RunTask<RepeatOptions> Task(Action task, RepeatOptions options = null)
        {
            return _taskFactory.Create(task, options);
        }

        public static RunTask<RepeatOptions> TaskOnce(Action task, RepeatOptions options = null)
        {
            var onceOnlyTask = new OnceOnlySyncTask(task);
            return _taskFactory.Create(onceOnlyTask.Run, options);
        }
    }
}
