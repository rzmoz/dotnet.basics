﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace DotNet.Basics.Tasks.Pipelines
{
    public class Pipeline<T> : PipelineBlock<T> where T : EventArgs, new()
    {
        public Pipeline(string name = null) : this(name, null)
        {
        }
        public Pipeline(IContainer container) : this(null, container)
        {
        }
        public Pipeline(string name, IContainer container) : base(name ?? PipelineTaskTypes.Pipeline, container)
        {
        }
        protected override async Task InnerRunAsync(T args, CancellationToken ct)
        {
            foreach (var section in SubSections)
            {
                await section.RunAsync(args, ct).ConfigureAwait(false);
                if (ct.IsCancellationRequested)
                    break;
            }
        }
        public override string TaskType => PipelineTaskTypes.Pipeline;
    }
}
