﻿using System;
using System.IO;
using System.Linq;
using DotNet.Basics.IO;
using DotNet.Basics.Sys;
using DotNet.Basics.TestsRoot;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Basics.Tests.IO
{
    public class DirPathExtensionsTests : TestWithHelpers
    {
        public DirPathExtensionsTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public void SearchOption_SubDirs_OptionsAreObeyed()
        {
            ArrangeActAssertPaths(testDir =>
            {
                var subDir = testDir.CreateSubDir("1");
                subDir.CreateSubDir("12");
                subDir.CreateSubDir("23");
                subDir.ToFile("myFile1.txt").WriteAllText("bla");

                testDir.GetPaths(searchOption: SearchOption.TopDirectoryOnly).Count.Should().Be(1);
                testDir.GetPaths(searchOption: SearchOption.AllDirectories).Count.Should().Be(4);

                testDir.GetDirectories(searchOption: SearchOption.TopDirectoryOnly).Count.Should().Be(1);
                testDir.GetDirectories(searchOption: SearchOption.AllDirectories).Count.Should().Be(3);

                testDir.GetFiles(searchOption: SearchOption.TopDirectoryOnly).Count.Should().Be(0);
                testDir.GetFiles(searchOption: SearchOption.AllDirectories).Count.Should().Be(1);
            });
        }

        [Fact]
        public void SearchPattern_OnlyTxtFiles_PatternIsObeyed()
        {
            ArrangeActAssertPaths(testDir =>
            {
                testDir.CreateSubDir("1");
                testDir.ToFile("myFile1.txt").WriteAllText("bla");
                testDir.ToFile("myFile2.json").WriteAllText("bla");

                var jsonFromPaths = testDir.GetPaths("*.txt");
                var jsonFromDirs = testDir.GetDirectories("*.txt");
                var jsonFromFiles = testDir.GetFiles("*.txt");

                jsonFromPaths.Count.Should().Be(1);
                jsonFromDirs.Count.Should().Be(0);
                jsonFromFiles.Count.Should().Be(1);
            });
        }


        [Fact]
        public void ToDir_CombineToDir_FullNameIsCorrect()
        {
            var _testDirRoot = @"K:\testDir";
            var _testDoubleDir = @"\testa\testb";

            var actual = _testDirRoot.ToDir(_testDoubleDir);
            var expected = _testDirRoot + _testDoubleDir + actual.Separator;
            actual.FullName().Should().Be(expected);

            actual = @"c:\BuildLibrary\Dir\Module 2.0.1".ToDir("Website");
            expected = @"c:\BuildLibrary\Dir\Module 2.0.1\Website\";
            actual.FullName().Should().Be(expected);
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsCleaned()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

                testDir.Exists().Should().BeTrue();
                testDir.GetFiles().Count.Should().Be(1);

                //act
                testDir.CreateIfNotExists();
                testDir.CleanIfExists();

                //assert
                testDir.Exists().Should().BeTrue();
                testDir.GetFiles().Count.Should().Be(0);
            });
        }

        [Fact]
        public void CreateIfExists_CreateOptions_ExistingDirIsNotCleaned()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                testDir.DeleteIfExists();
                testDir.ToFile("myFile.txt").WriteAllText(@"bllll");

                testDir.Exists().Should().BeTrue();
                testDir.GetFiles().Count.Should().Be(1);

                //act
                testDir.CreateIfNotExists();

                //assert
                testDir.Exists().Should().BeTrue();
                testDir.GetFiles().Count.Should().Be(1);
            });
        }
        [Fact]
        public void CleanIfExists_DirDoesntExists_NoActionAndNoExceptions()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                var dirThatDoesntExists = testDir.ToDir("SOMETHINGTHAT DOESNT EXIST_BLAAAAAA");

                //act
                dirThatDoesntExists.CleanIfExists();

                //assert
                dirThatDoesntExists.Exists().Should().BeFalse();
            });

        }
        [Fact]
        public void CleanIfExists_RemoveAllContentFromADir_DirIsCleaned()
        {
            ArrangeActAssertPaths(testDir =>
            {
                //arrange
                testDir.CreateIfNotExists();

                const int numOfTestFiles = 3;

                AddTestContent(testDir, 0, numOfTestFiles);

                testDir.GetFiles().Count.Should().Be(numOfTestFiles);

                //act
                testDir.CleanIfExists();

                testDir.GetFiles().Count.Should().Be(0);
            });
        }

        [Fact]
        public void CopyTo_IncludeSubDirectories_DirIsCopied()
        {
            ArrangeActAssertPaths(dir =>
            {
                var sourceDir = dir.CreateSubDir("MAH1_SOURCE");
                var targetDir = dir.CreateSubDir("MAH_TARGET");

                AddTestContent(sourceDir, 3, 3);

                targetDir.DeleteIfExists();
                targetDir.Exists().Should().BeFalse();

                sourceDir.CopyTo(targetDir, includeSubfolders: true);

                targetDir.Exists().Should().BeTrue();
                targetDir.EnumeratePaths().Count().Should().Be(6);
            });
        }

        private void AddTestContent(DirPath dir, int numOfTestDirs, int numOfTestFiles)
        {
            for (var i = 0; i < numOfTestDirs; i++)
            {
                var testFolder = dir.ToDir("MyTestFolder" + i);
                testFolder.CreateIfNotExists();
                testFolder.ToFile($"blaa{i}.txt").WriteAllText("blaaa");
            }
            for (var i = 0; i < numOfTestFiles; i++)
                dir.ToFile($"myFile{i}.txt").WriteAllText("blaaaaa");
        }
    }
}
