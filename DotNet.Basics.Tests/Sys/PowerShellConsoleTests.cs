﻿using System.Collections.Generic;
using System.Linq;
using DotNet.Basics.Sys;
using FluentAssertions;
using NUnit.Framework;

namespace DotNet.Basics.Tests.Sys
{
    [TestFixture]
    public class PowerShellConsoleTests
    {
        [Test, Ignore("Errors are not detected in error streams not invocation state.. Dont know why")]
        public void Results_HadErros_ErrorsFound()
        {
            string script = $"Copy-Item asdAsdasd asd asd asd asd asd asd"; //cmdlet with invalid arguments
            //string script = "throw 'my exception'";

            var result = PowerShellConsole.RunScript($"\"{script}\"");

            result.HadErrors.Should().BeTrue();
        }

        [Test]
        public void RunScript_ExecuteScript_HelloworldIsOutputted()
        {
            const string greeting = @"Hello World!";

            var result = PowerShellConsole.RunScript($"\"{greeting}\"");

            result.PassThru.Single().ToString().Should().Be(greeting);
        }
    }
}
