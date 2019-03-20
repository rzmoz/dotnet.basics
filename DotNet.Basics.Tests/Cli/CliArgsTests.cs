﻿using DotNet.Basics.Cli;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliArgsTests
    {
        [Theory]
        [InlineData("debug")]
        [InlineData("d")]
        public void IsSet_Debug_IsFound(string arg)
        {
            var args = new[] { arg.EnsurePrefix("-") };

            var cliArgs = new CliArgsBuilder().Build(args);

            cliArgs.IsDebug.Should().BeTrue();
        }
        
        [Theory]
        [InlineData("myKey", "myValue")]
        public void Index_FindByKey_ValueIsFound(string key, string value)
        {
            var args = new[] { key.EnsurePrefix("-"), value };

            var cliArgs = new CliArgsBuilder().Build(args);

            cliArgs.IsSet(key).Should().BeTrue();
            cliArgs.Config[key].Should().NotBeNull();
            cliArgs[key].Should().Be(cliArgs.Config[key]);
            cliArgs[key].Should().Be(value);
        }
    }
}