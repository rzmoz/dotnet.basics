﻿using System;
using DotNet.Basics.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace DotNet.Basics.Tests.IO
{
    
    public class PathInfoExtensionsTests
    {
        [Fact]
        public void Serialization_JsonSerialization_DirIsSerialized()
        {
            var dir = ".".ToPath();

            Action action = () => JsonConvert.SerializeObject(dir);

            action.ShouldNotThrow();
        }


        [Theory]
        [InlineData("FOLDER_THAT_DOES_NOT_EXIST_WO_FOLDER_MARKER", false)]//folder that doesnt exist without marker
        [InlineData("FOLDER_THAT_DOES_NOT_EXIST_WITH_FOLDER_MARKER//", true)]//folder that doesnt exist with marker
        [InlineData("IsFolder_DetectIfFolder_FoldersAreDetected", true)]//folder that exists without marker
        [InlineData("IsFolder_DetectIfFolder_FoldersAreDetected\\myfile.txt\\", false)]//file that exists with folder marker
        public void IsFolder_DetectIfFolder_FoldersAreDetected(string input, bool expectedIsFolder)
        {
            var dir = @"IsFolder_DetectIfFolder_FoldersAreDetected".ToDir();
            dir.CreateIfNotExists();
            "dummycontent".WriteAllText(dir.ToFile("myfile.txt"));

            var path = ".".ToPath(input);

            path.IsFolder.Should().Be(expectedIsFolder);
        }

        [Theory]
        [InlineData("Exists_TestPath_PathIsVerified", true)]//dir
        [InlineData("Exists_TestPath_PathIsVerified\\file.txt", false)]//file
        public void Exists_TestPath_PathIsVerified(string path, bool isFolder)
        {
            PathInfo p;
            if (isFolder)
                p = path.ToDir();
            else
                p = path.ToFile();

            p.DeleteIfExists();
            p.Exists().Should().BeFalse(p.FullName);

            if (p.IsFolder)
                p.ToDir().CreateIfNotExists();
            else
                "dummycontent".WriteAllText(p);

            p.Exists().Should().BeTrue(p.FullName);
        }

        [Theory]
        [InlineData("http://", true)]
        [InlineData("http:/", false)]
        [InlineData("http://folder", false)]//
        public void IsProtocol_ProtocolIsDetected(string path, bool expected)
        {
            path.IsProtocol().Should().Be(expected);
        }
        [Theory]
        [InlineData("http://", true)]
        [InlineData("http:/", false)]
        [InlineData("http://folder", true)]//
        public void HasProtocol_ProtocolIsDetected(string path, bool expected)
        {
            path.HasProtocol().Should().Be(expected);
        }

        [Fact]
        public void SplitSplitSegments_UriSUpport_UriIsSPlitProperly()
        {
            var segments = new[]
            {
                "http://localhost:80/",
                "myfile.aspx"
            };

            var split = segments.SplitToSegments();

            split.Length.Should().Be(3);
            split[0].Should().Be("http://");
            split[1].Should().Be("localhost:80");
            split[2].Should().Be("myfile.aspx");
        }

        [Theory]
        [InlineData(PathDelimiter.Slash, '/')]
        [InlineData(PathDelimiter.Backslash, '\\')]
        public void ToChar_DelimiterToChar_DelimiterIsConverted(PathDelimiter delimiter, char expectedChar)
        {
            var foundChar = delimiter.ToChar();
            foundChar.Should().Be(expectedChar);
        }

        [Theory]
        [InlineData('/', PathDelimiter.Slash)]
        [InlineData('\\', PathDelimiter.Backslash)]
        public void ToPathDelimiter_CharToDelimiter_CharIsConverted(char delimiter, PathDelimiter expectedPathDelimiter)
        {
            var foundPathDelimiter = delimiter.ToPathDelimiter();
            foundPathDelimiter.Should().Be(expectedPathDelimiter);
        }

        [Theory]
        [InlineData('a')]
        [InlineData('1')]
        [InlineData('¤')]
        public void ToPathDelimiter_UnsupportedChar_ExceptionIsThrown(char delimiter)
        {
            Action action = () => delimiter.ToPathDelimiter();
            action.ShouldThrow<NotSupportedException>();
        }

        [Theory]
        [InlineData("//pt101", "pt2", PathDelimiter.Slash)]//file
        [InlineData("\\pt101", "pt2", PathDelimiter.Backslash)]//file
        [InlineData("//pt101", "pt2/", PathDelimiter.Slash)]//dir
        [InlineData("\\pt101", "pt2\\", PathDelimiter.Backslash)]//dir
        public void ToPath_Combine_PathIsGenerated(string pt1, string pt2, PathDelimiter pathDelimiter)
        {
            var path = pt1.ToPath(pt2);

            var refPath = pt1 + pathDelimiter.ToChar() + pt2;
            if (path.IsFolder == false)
                refPath = refPath.TrimEnd(pathDelimiter.ToChar());
            refPath = refPath.TrimStart(pathDelimiter.ToChar());
            path.RawName.Should().Be(refPath);
        }


        [Theory]
        [InlineData("http://localhost/", "myFolder/myFile.html")]//url
        public void ToPath_Uris_PathIsGenerated(string pt1, string pt2)
        {
            var path = pt1.ToPath(pt2);
            path.FullName.Should().Be("http://localhost/myFolder/myFile.html");
        }


        [Theory]
        [InlineData("mypath", false)]//file
        [InlineData("mypath", true)]//file
        public void IsFolder_Set_IsFolderIsSet(string pth, bool isFolder)
        {
            PathInfo p;
            if (isFolder)
                p = pth.ToDir();
            else
                p = pth.ToFile();

            p.IsFolder.Should().Be(isFolder);
        }
    }
}
