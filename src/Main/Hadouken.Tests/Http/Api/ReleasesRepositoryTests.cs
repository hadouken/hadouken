using System;
using System.Collections.Generic;
using Hadouken.Configuration;
using Hadouken.Http;
using Hadouken.Http.Api;
using Hadouken.Http.Api.Models;
using NSubstitute;
using Xunit;

namespace Hadouken.Tests.Http.Api
{
    public class ReleasesRepositoryTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_When_Configuration_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () => new ReleasesRepository(null, Substitute.For<IApiConnection>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_When_ApiConnection_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(
                    () => new ReleasesRepository(Substitute.For<IConfiguration>(), null));
            }
        }

        public class TheGetAllMethod
        {
            [Fact]
            public void Should_Call_Correct_Uri()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                conf.Plugins.RepositoryUri.Returns(new Uri("http://test.com"));
                var api = Substitute.For<IApiConnection>();
                var repository = new ReleasesRepository(conf, api);

                // When
                repository.GetAll();

                api.Received(1).GetAsync<IEnumerable<ReleaseItem>>(new Uri("http://test.com/releases"));
            }
        }
    }
}
