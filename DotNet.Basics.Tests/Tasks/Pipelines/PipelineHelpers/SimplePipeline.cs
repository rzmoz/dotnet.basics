﻿using DotNet.Basics.Sys;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines.PipelineHelpers
{
    public class SimplePipeline : Pipeline<EventArgs<int>>
    {
        public SimplePipeline()
        {
            AddBlock("MyBlock").AddStep<IncrementArgsStep>();
        }
    }
}
