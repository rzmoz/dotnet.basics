﻿using DotNet.Basics.Sys;
using DotNet.Basics.Sys.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Sys.Text
{
    public class SemVersionJsonConverterTests
    {
        private const string _rawJson = @"""1.0.3-beta.1.2\u002BHelloWorld""";
        private const string _rawVersion = @"1.0.3-beta.1.2+HelloWorld";

        [Fact]
        public void Convert_Serialize_PathIsSerialized()
        {
            var semVer = new SemVersion(_rawVersion);

            var json = semVer.SerializeToJson();

            json.Should().Be(_rawJson);
        }

        [Fact]
        public void Convert_Deserialize_PathIsDeserialized()
        {
            var version = _rawJson.DeserializeJson<SemVersion>();

            version.SemVer20String.Should().Be(_rawVersion);
        }
    }
}
