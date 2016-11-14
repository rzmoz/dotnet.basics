﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Basics.Tasks;
using DotNet.Basics.Tasks.Pipelines;

namespace DotNet.Basics.Tests.Tasks.Pipelines
{
    public class GenericThatTakesAnotherConcreteClassAsArgStep<T> : PipelineStep<T> where T : class, new()
    {
        private ClassThatTakesAnotherConcreteClassAsArgStepDependsOn _argStepDependsOn;

        public GenericThatTakesAnotherConcreteClassAsArgStep(ClassThatTakesAnotherConcreteClassAsArgStepDependsOn argStepDependsOn)
        {
            _argStepDependsOn = argStepDependsOn;
        }

        protected override Task RunImpAsync(T args, TaskIssueList issues, CancellationToken ct)
        {
            return Task.FromResult("");
        }
    }
}
