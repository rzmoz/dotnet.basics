﻿using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class DescendantStep : PipelineStep<DescendantArgs>
    {
        protected override Task RunImpAsync(DescendantArgs args, CancellationToken ct)
        {
            args.DescendantUpdated = true;
            return Task.CompletedTask;
        }
    }
}
