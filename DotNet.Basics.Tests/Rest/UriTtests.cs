﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Basics.Rest;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Tests.Rest
{
    
    public class UriTtests
    {
        [Fact]
        public void Ctor_FullUri_AllelementsAreSplit()
        {
            const string absoluteUri = "https://my.domain.dk:1010/folder/file?myparam=yes&nextParam=somethingElse";
            var uri = new Uri(absoluteUri);
            uri.AbsoluteUri.Should().Be(absoluteUri);
            uri.AbsolutePath.Should().Be("/folder/file");
            uri.Authority.Should().Be("my.domain.dk:1010");
            uri.Host.Should().Be("my.domain.dk");
            uri.PathAndQuery.Should().Be("/folder/file?myparam=yes&nextParam=somethingElse");
            uri.Port.Should().Be(1010);
            uri.Query.Should().Be("?myparam=yes&nextParam=somethingElse");
            uri.Scheme.Should().Be("https");
        }

        [Fact]
        public void BaseUri_GetBaseUri_BaseUriIsPersed()
        {
            const string absoluteUri = "https://my.domain.dk:1010/folder/file?myparam=yes&nextParam=somethingElse";
            var uri = new Uri(absoluteUri);
            uri.BaseUri().Should().Be("https://my.domain.dk:1010/");
        }


        [Theory]
        [InlineData("/localhost/")]//no scheme
        [InlineData("http:/localhost/")]//invalid scheme separator
        public void Ctor_NotUri_ExceptionIsThrown(string nonUri)
        {
            Action action = () => new Uri(nonUri);
            action.ShouldThrow<UriFormatException>();
        }
    }
}
