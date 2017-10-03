﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.IO;
using DotNet.Basics.Shell;
using DotNet.Basics.Sys;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    public class ExecutableInstallerTests
    {
        [Fact]
        public void AddFromStream_EnsureAppIsInstalled_AppIsInstalledInMultiThreadedEnvironment()
        {
            var appDir = TestRoot.Dir.Add("AddFromStream_EnsureAppIsInstalled_AppIsInstalledInMultiThreadedEnvironment");
            appDir.DeleteIfExists();
            var fileName = "ReturnIntConsole.Exe";
            var consoleAppStream =
                typeof(ExecutableInstallerTests).Assembly.GetManifestResourceStream("DotNet.Basics.Tests.IO.Testa.ReturnIntConsole.exe");

            consoleAppStream.Should().NotBeNull("ReturnIntConsole.Exe");

            var installRange = Enumerable.Range(1, 50);
            Parallel.ForEach(installRange, val =>
            {
                var installer = new ExecutableInstaller(appDir, fileName);
                installer.AddFromStream(fileName, consoleAppStream);
                installer.Install();

                var fullName = appDir.ToFile(fileName).FullPath();

                var result = CmdPrompt.Run($"{fullName} {val}");
                result.ExitCode.Should().Be(val);
            });
        }
    }
}
