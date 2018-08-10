using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using AspNetRouteVersions.TestWebApp;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AspNetRouteVersions.IntegrationTests
{
    public class StartupTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public StartupTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ConfigureServices_NotUseAcceptHeader_ThrowsHttpRequestException()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseAcceptHeader = false;
            });
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.api-version.v1+json"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await client.GetStringAsync("/api/items/"));
        }

        [Fact]
        public async Task ConfigureServices_NotUseCustomHeader_ThrowsHttpRequestException()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseCustomHeader = false;
            });
            client.DefaultRequestHeaders.Add("api-version", "1");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await client.GetStringAsync("/api/items/"));
        }

        [Fact]
        public async Task ConfigureServices_NotUseQuery_ThrowsHttpRequestException()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseQuery = false;
            });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await client.GetStringAsync("/api/items/?api-version=1"));
        }

        [Fact]
        public async Task ConfigureServices_NotUseRoute_ThrowsHttpRequestException()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseRoute = false;
            });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await client.GetStringAsync("/api/v1/items/"));
        }

        [Fact]
        public async Task ConfigureServices_UseAcceptHeader_ReturnsResult()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseAcceptHeader = true;
                options.UseCustomHeader = false;
                options.UseQuery = false;
                options.UseRoute = false;
            });
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.api-version.v1+json"));

            // Act
            var result = await client.GetStringAsync("/api/items/");

            // Assert
            Assert.Equal("Get Items Version 1", result);
        }

        [Fact]
        public async Task ConfigureServices_UseCustomHeader_ReturnsResult()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseAcceptHeader = false;
                options.UseCustomHeader = true;
                options.UseQuery = false;
                options.UseRoute = false;
            });
            client.DefaultRequestHeaders.Add("api-version", "1.5");

            // Act
            var result = await client.GetStringAsync("/api/items/");

            // Assert
            Assert.Equal("Get Items Version 1.5", result);
        }

        [Fact]
        public async Task ConfigureServices_UseQuery_ReturnsResult()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseAcceptHeader = false;
                options.UseCustomHeader = false;
                options.UseQuery = true;
                options.UseRoute = false;
            });
            
            // Act
            var result = await client.GetStringAsync("/api/items/?api-version=1");

            // Assert
            Assert.Equal("Get Items Version 1", result);
        }

        [Fact]
        public async Task ConfigureServices_UseRoute_ReturnsResult()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseAcceptHeader = false;
                options.UseCustomHeader = false;
                options.UseQuery = false;
                options.UseRoute = true;
            });
            
            // Act
            var result = await client.GetStringAsync("/api/v1/items/");

            // Assert
            Assert.Equal("Get Items Version 1", result);
        }

        [Fact]
        public async Task ConfigureServices_SetAcceptHeaderVendor_ReturnsResult()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.SetAcceptHeaderVendor("test-custom-header-123");
            });
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.test-custom-header-123.v1+json"));

            // Act
            var result = await client.GetStringAsync("/api/items/");

            // Assert
            Assert.Equal("Get Items Version 1", result);
        }

        [Fact]
        public async Task ConfigureServices_UseNone_ThrowsAmbiguousActionException()
        {
            // Arrange
            var client = GetHttpClientWithConfigureRouteVersions(_factory, options =>
            {
                options.UseAcceptHeader = false;
                options.UseCustomHeader = false;
                options.UseQuery = false;
                options.UseRoute = false;
                options.QueryKey = "v";
                options.CustomHeaderKey = "my-api-version";
                options.RouteKey = "version";
            });

            // Act & Assert
            await Assert.ThrowsAnyAsync<AmbiguousActionException>(async () => await client.GetStringAsync("/api/v2/items/"));
        }

        private static WebApplicationFactory<Startup> GetFactoryWithServicesConfigure(
            WebApplicationFactory<Startup> factory,
            Action<IServiceCollection> servicesConfiguration)
        {
            return factory.WithWebHostBuilder(config =>
            {
                config.ConfigureServices(servicesConfiguration);
            });
        }

        private static WebApplicationFactory<Startup> GetFactoryWithConfigureRouteVersions(
            WebApplicationFactory<Startup> factory,
            Action<RouteVersionsOptions> configureRouteVersions)
        {
            return factory.WithWebHostBuilder(config =>
            {
                config.ConfigureServices(services => {
                    services.ConfigureRouteVersions(configureRouteVersions);
                });
            });
        }

        private static HttpClient GetHttpClientWithConfigureRouteVersions(
            WebApplicationFactory<Startup> factory,
            Action<RouteVersionsOptions> configureRouteVersions)
        {
            var configuredFactory = GetFactoryWithConfigureRouteVersions(factory, configureRouteVersions);
            return configuredFactory.CreateClient();
        }
    }
}