﻿using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Shell;
using DotNet.Basics.Sys;
using DotNet.Basics.Tests.IO.Testa;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.Shell
{
    public class RobocopyTests
    {
        public RobocopyTests(ITestOutputHelper output)
        {
            DebugOut.Out += output.WriteLine;
        }
        
        [Fact]
        public void MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved()
        {
            var baseDir = TestRoot.Dir.Add("MoveContent_TargetFolderDoesntExist_SourceFolderIsMoved");
            var emptyDir = baseDir.ToDir("empty");
            var sourceDir = baseDir.ToDir("source");
            var targetDir = baseDir.ToDir("target");
            var testSource = new TestFile1();
            Robocopy.CopyDir(testSource.Directory().FullPath(), sourceDir.FullPath(), true, null);
            emptyDir.CreateIfNotExists();
            emptyDir.CleanIfExists();
            emptyDir.GetPaths().Length.Should().Be(0);//empty dir
            sourceDir.Exists().Should().BeTrue(sourceDir.FullPath());
            targetDir.DeleteIfExists();
            targetDir.Exists().Should().BeFalse(targetDir.FullPath());

            //act
            var result1 = Robocopy.MoveContent(sourceDir.FullPath(), targetDir.FullPath(), null, true, null);
            DebugOut.WriteLine(result1.Output);
            var result2 = Robocopy.MoveContent(emptyDir.FullPath(), targetDir.FullPath(), null, true, null);//move empty dir to ensure target dir is not cleaned
            DebugOut.WriteLine(result2.Output);

            //assert
            sourceDir.Exists().Should().BeTrue(sourceDir.FullPath());
            sourceDir.IsEmpty();
            targetDir.GetFiles().Single().Name.Should().Be(testSource.Name);
        }

        [Fact]
        public void Copy_CopySingleFileSourceExists_FileIsCopied()
        {
            var sourcefile = new TestFile1();
            sourcefile.Exists().Should().BeTrue("source file should exist");

            var targetFile = TestRoot.Dir.ToFile("Copy_CopySingleFileSourceExists_FileIsCopied", sourcefile.Name);
            targetFile.DeleteIfExists();
            targetFile.Exists().Should().BeFalse("target file should not exist before copy");

            //act
            var result = Robocopy.CopyFile(sourcefile.Directory().FullPath(), sourcefile.Name, targetFile.Directory().FullPath());
            DebugOut.WriteLine(result.Output);

            result.ExitCode.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            targetFile.Exists().Should().BeTrue("target file is copied");
        }

        [Fact]
        public void CopyDir_CopyDirSourceExists_DirIsCopied()
        {
            var source = TestRoot.Dir.Add("CopyDir_CopyDirSourceExists_DirIsCopied", "source");
            var target = TestRoot.Dir.Add("CopyDir_CopyDirSourceExists_DirIsCopied", "target");
            var sourceFile = source.ToFile("myfile.txt");
            var targetFile = source.ToFile(sourceFile.Name);
            var fileContent = "blavlsavlsdglsdflslfsdlfsdlfsd";
            target.DeleteIfExists();
            target.Exists().Should().BeFalse();

            source.CreateIfNotExists();
            sourceFile.WriteAllText(fileContent);
            sourceFile.Exists().Should().BeTrue();

            //act
            var result = Robocopy.CopyDir(source.FullPath(), target.FullPath(), true);

            //assert
            DebugOut.WriteLine(result.Output);
            result.ExitCode.Should().BeLessThan(8); //http://ss64.com/nt/robocopy-exit.html
            target.Exists().Should().BeTrue();
            targetFile.Exists().Should().BeTrue();
            targetFile.ReadAllText().Should().Be(fileContent);
        }
    }
}
