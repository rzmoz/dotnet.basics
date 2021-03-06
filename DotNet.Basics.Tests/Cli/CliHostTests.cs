﻿using System;
using System.Linq;
using DotNet.Basics.Cli;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Cli
{
    public class CliHostTests
    {
        [Fact]
        public void HasValue_CheckIsSet_Flag_FlagIsSet()
        {
            var flagWithValue = "myFlagWithValue";
            var flagWoValue = "myFlagWoValue";

            var args = new[] { $"-{flagWoValue}", $"-{flagWithValue}", "SomeValue" };

            var host = new CliHostBuilder(args).Build();

            host.HasValue(flagWoValue).Should().BeFalse();
            host.HasValue(flagWithValue).Should().BeTrue();
        }

        [Fact]
        public void IsSet_Flag_FlagIsSet()
        {
            var flag = "myFlag";
            var paramWithValue = "myKey";
            var args = new[] { $"-{flag}", $"-{paramWithValue}", "SomeValue" };

            var host = new CliHostBuilder(args).Build();

            host.IsSet(flag).Should().BeTrue();
            host.IsSet(paramWithValue).Should().BeTrue();
            host.IsSet("SOME_FLAG_THAT_IS_NOT_SET").Should().BeFalse();
        }

        [Fact]
        public void Configuration_AppSettingsJson_ConfigurationIsEnvironmentSpecific()
        {
            var environment = "test";
            var args = new[] { "-envs", environment };//should make config look for appsettings.test.json

            var host = new CliHostBuilder(args).Build();

            host.Environments.Contains(environment, StringComparer.InvariantCultureIgnoreCase).Should().BeTrue();
            host["settingFrom"].Should().Be("appSettings.test.json");
            host["hello"].Should().Be("Test World!");
        }

        [Fact]
        public void Environments_Order_OrderIsKept()
        {
            var environment1 = "lorem";
            var environment2 = "ipsum";
            var environment3 = "golem";

            var args = new[] { "-envs", $"{environment1}|{environment2}|{environment3}" };

            var host = new CliHostBuilder(args).Build();

            //order must be kept
            host.Environments.First().Should().Be(environment1.ToTitleCase());
            host.Environments.Skip(1).Take(1).Single().Should().Be(environment2.ToTitleCase());
            host.Environments.Last().Should().Be(environment3.ToTitleCase());
        }
    }
}
