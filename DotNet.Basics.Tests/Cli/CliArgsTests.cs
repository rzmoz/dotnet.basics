﻿using DotNet.Basics.Cli;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliArgsTests
    {

        [Theory]
        [InlineData("key", "myValue")]
        public void GetByPosition_KeyNoySet_ValueIsFound(string key, string value)
        {
            var args = new[] { value };

            var cliArgs = new CliHostBuilder(args).Build();
            cliArgs[key].Should().BeNull();//key not set
            cliArgs[key,0].Should().Be(value);//found by position
        }

        [Theory]
        [InlineData("debug", "debug", true)]
        [InlineData("d", "debug", true)]
        [InlineData("v", "nvvvvvv", false)]
        public void IsSet_Flag_IsFound(string arg, string flag, bool isSet)
        {
            var args = new[] { arg.EnsurePrefix("-") };
            args.IsSet(flag).Should().Be(isSet);
        }

        [Theory]
        [InlineData("myKey", "myValue")]
        public void Index_FindByKey_ValueIsFound(string key, string value)
        {
            var args = new[] { key.EnsurePrefix("-"), value };

            var cliArgs = new CliHostBuilder(args).Build();

            args.IsSet(key).Should().BeTrue();
            cliArgs.Config[key].Should().NotBeNull();
            cliArgs[key].Should().Be(cliArgs.Config[key]);
            cliArgs[key].Should().Be(value);
        }
        [Theory]
        [InlineData("myKey", "myValue")]
        public void Index_FindByIndex_ValueIsFound(string key, string value)
        {
            var pos1 = "HelloWorld!";
            var args = new[] { "pos0", pos1, "pos2" };

            var cliArgs = new CliHostBuilder(args).Build();

            cliArgs[1].Should().Be(pos1);
        }
    }
}

