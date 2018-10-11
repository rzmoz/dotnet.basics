﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class Pipeline<T> : ManagedTask<T> where T : class, new()
    {
        private readonly Func<IServiceProvider> _getServiceProvider;
        private readonly ConcurrentQueue<ManagedTask<T>> _tasks;
        private readonly Func<T, ConcurrentLog, CancellationToken, Task> _innerRun;

        public Pipeline(Func<IServiceProvider> getServiceProvider = null) : this(getServiceProvider, Invoke.Sequential)
        { }

        public Pipeline(Invoke invoke) : this(null, null, invoke)
        { }

        public Pipeline(string name) : this(null, name, Invoke.Sequential)
        { }
        public Pipeline(string name, Invoke invoke)
            : this(null, name, invoke)
        { }
        public Pipeline(Func<IServiceProvider> getServiceProvider, Invoke invoke)
            : this(getServiceProvider, null, invoke)
        { }

        public Pipeline(Func<IServiceProvider> getServiceProvider, string name, Invoke invoke)
            : base(name)
        {
            _getServiceProvider = getServiceProvider;
            _tasks = new ConcurrentQueue<ManagedTask<T>>();
            Invoke = invoke;
            switch (Invoke)
            {
                case Invoke.Parallel:
                    _innerRun = InnerParallelRunAsync;
                    break;
                case Invoke.Sequential:
                    _innerRun = InnerSequentialRunAsync;
                    break;
                default:
                    throw new ArgumentException($"{nameof(Invoke)} not supported: {Invoke }");
            }
        }

        public IReadOnlyCollection<ManagedTask<T>> Tasks => _tasks.ToList();
        public Invoke Invoke { get; }

        public TaskResult AssertLazyLoadSteps()
        {
            return AssertLazyLoadSteps(Tasks);
        }

        private TaskResult AssertLazyLoadSteps(IReadOnlyCollection<ITask> tasks)
        {
            return new TaskResult(log =>
            {
                foreach (var pipeline in tasks.OfType<Pipeline<T>>())
                    log.AddRange(AssertLazyLoadSteps(pipeline.Tasks).Log);
                foreach (var lazyLoadStep in tasks.OfType<ILazyLoadStep>())
                {
                    try
                    {
                        lazyLoadStep.GetTask();
                    }
                    catch (InvalidOperationException e)
                    {
                        log.Add(LogLevel.Error, $"Failed to load: {lazyLoadStep.GetTaskType().Name} - {e.Message}", e);
                    }
                }
            });
        }

        public Pipeline<T> AddStep<TTask>(string name = null) where TTask : ManagedTask<T>
        {
            var lazyTask = new LazyLoadStep<T, TTask>(name, () =>
            {
                var serviceProvider = _getServiceProvider?.Invoke();
                if (serviceProvider == null)
                    throw new NoServiceProviderInPipelineException($"Pipeline must be instantiated with an IServiceProvider when adding tasks by type");
                return serviceProvider.GetService(typeof(TTask)) as TTask;
            });
            InitEvents(lazyTask);
            _tasks.Enqueue(lazyTask);
            return this;
        }

        public Pipeline<T> AddStep(Action<T, ConcurrentLog, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Action<T, ConcurrentLog, CancellationToken> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(Func<T, ConcurrentLog, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(string name, Func<T, ConcurrentLog, CancellationToken, Task> task)
        {
            var mt = new ManagedTask<T>(name, task);
            return AddStep(mt);
        }

        public Pipeline<T> AddStep(ManagedTask<T> mt)
        {
            InitEvents(mt);
            _tasks.Enqueue(mt);
            return this;
        }

        public Pipeline<T> AddBlock(params Func<T, ConcurrentLog, CancellationToken, Task>[] tasks)
        {
            return AddBlock(null, tasks);
        }

        public Pipeline<T> AddBlock(string name, params Func<T, ConcurrentLog, CancellationToken, Task>[] tasks)
        {
            return AddBlock(name, Invoke.Parallel, tasks);
        }

        public Pipeline<T> AddBlock(string name, Invoke invoke = Invoke.Parallel, params Func<T, ConcurrentLog, CancellationToken, Task>[] tasks)
        {
            var count = _tasks.Count(s => s.GetType() == typeof(Pipeline<>));
            var block = new Pipeline<T>(_getServiceProvider, name ?? $"Block {count}", invoke);
            foreach (var task in tasks)
                block.AddStep(task);
            InitEvents(block);
            _tasks.Enqueue(block);
            return block;
        }

        protected override async Task InnerRunAsync(T args, ConcurrentLog log, CancellationToken ct)
        {
            await _innerRun(args, log, ct).ConfigureAwait(false);
        }

        protected async Task InnerParallelRunAsync(T args, ConcurrentLog log, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return;
            var tasks = Tasks.Select(async t =>
            {
                var result = await t.RunAsync(args, ct).ConfigureAwait(false);
                log.AddRange(result.Log);

            }).ToList();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        protected async Task InnerSequentialRunAsync(T args, ConcurrentLog log, CancellationToken ct)
        {
            foreach (var task in Tasks)
            {
                if (ct.IsCancellationRequested)
                    break;
                var result = await task.RunAsync(args, ct).ConfigureAwait(false);
                log.AddRange(result.Log);
            }
        }

        private void InitEvents(ManagedTask<T> section)
        {
            section.Started += FireStarted;
            section.Ended += FireEnded;
        }
    }
}
