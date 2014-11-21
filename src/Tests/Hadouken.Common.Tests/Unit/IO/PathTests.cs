﻿using System;
using Hadouken.Common.IO;
using Xunit;
using Xunit.Extensions;

namespace Hadouken.Common.Tests.Unit.IO
{
    public sealed class PathTests
    {
        #region Private Test Classes

        private sealed class TestingPath : Path
        {
            public TestingPath(string path)
                : base(path)
            {
            }
        }

        #endregion

        public sealed class TheConstructor
        {
            [Fact]
            public void Should_Throw_If_Path_Is_Null()
            {
                // Given, When
                var result = Record.Exception(() => new TestingPath(null));

                // Then
                Assert.IsType<ArgumentNullException>(result);
                Assert.Equal("path", ((ArgumentNullException)result).ParamName);
            }

            [Theory]
            [InlineData("")]
            [InlineData("\t ")]
            public void Should_Throw_If_Path_Is_Empty(string fullPath)
            {
                // Given, When
                var result = Record.Exception(() => new TestingPath(fullPath));

                // Then
                Assert.IsType<ArgumentException>(result);
                Assert.Equal("path", ((ArgumentException)result).ParamName);

                Assert.True(result.Message.Contains("Path cannot be empty."));
            }

            [Fact]
            public void Current_Directory_Returns_Empty_Path()
            {
                // Given, When
                var path = new TestingPath("./");

                // Then
                Assert.Equal(string.Empty, path.FullPath);
            }

            [Fact]
            public void Will_Normalize_Path_Separators()
            {
                // Given, When
                var path = new TestingPath("shaders\\basic");

                // Then
                Assert.Equal("shaders/basic", path.FullPath);
            }

            [Fact]
            public void Will_Trim_WhiteSpace_From_Path()
            {
                // Given, When
                var path = new TestingPath(" shaders/basic ");

                // Then
                Assert.Equal("shaders/basic", path.FullPath);
            }

            [Fact]
            public void Will_Not_Remove_WhiteSpace_Within_Path()
            {
                // Given, When
                var path = new TestingPath("my awesome shaders/basic");

                // Then
                Assert.Equal("my awesome shaders/basic", path.FullPath);
            }

            [Fact]
            public void Should_Throw_If_Path_Contains_Illegal_Characters()
            {
                // Given
                var result = Record.Exception(() => new TestingPath("hello/**/world.txt"));

                // Then
                Assert.IsType<ArgumentException>(result);
                Assert.Equal("path", ((ArgumentException)result).ParamName);

                // We should not check the whole exception message because developet's machine may have
                // system language different from English so the system part of the exception message
                // will be localized.
                Assert.True(result.Message.Contains("Illegal characters in directory path (*)"));
            }

            [Theory]
            [InlineData("/Hello/World/", "/Hello/World")]
            [InlineData("\\Hello\\World\\", "/Hello/World")]
            [InlineData("file.txt/", "file.txt")]
            [InlineData("file.txt\\", "file.txt")]
            [InlineData("Temp/file.txt/", "Temp/file.txt")]
            [InlineData("Temp\\file.txt\\", "Temp/file.txt")]
            public void Should_Remove_Trailing_Slashes(string value, string expected)
            {
                // Given, When
                var path = new TestingPath(value);

                // Then
                Assert.Equal(expected, path.FullPath);
            }
        }

        public sealed class TheSegmentsProperty
        {
            [Theory]
            [InlineData("Hello/World")]
            [InlineData("/Hello/World")]
            [InlineData("/Hello/World/")]
            [InlineData("./Hello/World/")]
            public void Should_Return_Segments_Of_Path(string pathName)
            {
                // Given
                var path = new TestingPath("Hello/World");

                // When, Then
                Assert.Equal(2, path.Segments.Length);
                Assert.Equal("Hello", path.Segments[0]);
                Assert.Equal("World", path.Segments[1]);
            }
        }

        public sealed class TheFullPathProperty
        {
            [Fact]
            public void Should_Return_Full_Path()
            {
                // Given, When
                const string expected = "shaders/basic";
                var path = new TestingPath(expected);

                // Then
                Assert.Equal(expected, path.FullPath);
            }
        }

        public sealed class TheIsRelativeProperty
        {
            [Theory]
#if !UNIX
            [InlineData("c:/assets/shaders", false)]
            [InlineData("c:/assets/shaders/basic.frag", false)]
            [InlineData("c:/", false)]
            [InlineData("c:", false)]
#endif
            [InlineData("assets/shaders", true)]
            [InlineData("assets/shaders/basic.frag", true)]
            [InlineData("/assets/shaders", false)]
            [InlineData("/assets/shaders/basic.frag", false)]
            public void Should_Return_Whether_Or_Not_A_Path_Is_Relative(string fullPath, bool expected)
            {
                // Given, When
                var path = new TestingPath(fullPath);

                // Then
                Assert.Equal(expected, path.IsRelative);
            }
        }

        public sealed class TheToStringMethod
        {
            [Fact]
            public void Should_Return_The_Full_Path()
            {
                // Given, When
                var path = new TestingPath("temp/hello");

                // Then
                Assert.Equal("temp/hello", path.ToString());
            }
        }
    }
}