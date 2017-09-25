﻿using System;
using System.ComponentModel;
using DotNet.Basics.Sys;
using DotNet.Basics.Win32;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Win32
{
    public class ExecutableTests
    {
        [Fact]
        public void Run_StandardOut_OutputIsCaptured()
        {
            const string expected = "sdsdfsdfsdf";

            var fullPath = TestRoot.CurrentDir.Combine("NetCore.Basics.Tests.EchoOut.exe").FullName;

            var result = Executable.Run(fullPath, $"{expected}");

            result.Item2.Should().Be(expected);//std out
        }

        [Fact]
        public void Run_SimpleExecutable_ExitCodeIsCaptured()
        {
            const int expected = 16;

            var fullPath = TestRoot.CurrentDir.Combine("NetCore.Basics.Tests.EchoOut.exe").FullName;

            var result = Executable.Run(fullPath, $"{expected}");

            result.Item1.Should().Be(expected);//exit code
        }

        [Fact]
        public void Run_ExeNotFound_ExceptionIsThrown()
        {
            var fullPath = TestRoot.CurrentDir.Combine("Run_ExeNotFound_ExceptionIsThrown").FullName;

            Action action = () => Executable.Run(fullPath);

            action.ShouldThrow<Win32Exception>();
        }
    }
}
