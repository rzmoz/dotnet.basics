﻿using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Standard.Tasks.Pipelines
{
    public abstract class PipelineStep<T> : ManagedTask<T> where T : class, new()
    {
        protected PipelineStep() : this(null)
        { }

        protected PipelineStep(string name) : base(name)
        { }

        protected override Task InnerRunAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            return RunImpAsync(args, issues, ct);
        }

        protected abstract Task RunImpAsync(T args, TaskIssueList issues, CancellationToken ct);
    }
}