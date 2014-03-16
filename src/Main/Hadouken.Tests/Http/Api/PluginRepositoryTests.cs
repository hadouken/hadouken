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
    public class PluginRepositoryTests
    {
        public class TheConstructor
        {
            [Fact]
            public void Should_Throw_ArgumentNullException_If_Configuration_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(() => new PluginRepository(null, Substitute.For<IApiConnection>()));
            }

            [Fact]
            public void Should_Throw_ArgumentNullException_If_ApiConnection_Is_Null()
            {
                // Given, When, Then
                Assert.Throws<ArgumentNullException>(() => new PluginRepository(Substitute.For<IConfiguration>(), null));
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
                var repository = new PluginRepository(conf, api);

                // When
                repository.GetAll();

                // Then
                api.Received(1).GetAsync<IEnumerable<PluginListItem>>(new Uri("http://test.com/plugins"));
            }
        }

        public class TheGetByIdMethod
        {
            [Fact]
            public void Should_Call_Correct_Uri()
            {
                // Given
                var conf = Substitute.For<IConfiguration>();
                conf.Plugins.RepositoryUri.Returns(new Uri("http://test.com"));
                var api = Substitute.For<IApiConnection>();
                var repository = new PluginRepository(conf, api);

                // When
                repository.GetById("pluginid");

                // Then
                api.Received(1).GetAsync<Plugin>(new Uri("http://test.com/plugins/pluginid"));
            }
        }
    }
}
