﻿using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace DotNet.Basics.Rest.Tests
{
    public class HttpResponseMessageExtensionsTests
    {
        [Theory]
        [InlineData(null, null)]//null
        [InlineData("", "")]//empty
        [InlineData("     ", "")]//empty spaces
        [InlineData("\"", "")]//single quote
        [InlineData("\"\"", "")]//double quotes
        [InlineData("\"trimmed\"", "trimmed")]//double quotes
        public void TrimQuotes(string input, string expected)
        {
            var result = HttpResponseMessageExtensions.TrimQuotes(input);

            result.Should().Be(expected);
        }
    }
}
