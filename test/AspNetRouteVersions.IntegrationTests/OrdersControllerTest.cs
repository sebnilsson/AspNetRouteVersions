using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using AspNetRouteVersions.TestWebApp;
using Xunit;

namespace AspNetRouteVersions.IntegrationTests
{
    public class OrdersController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public OrdersController(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetOrders_Version1InUrl_ReturnsVersion1Content()
        {
            // Act
            var result = await _client.GetStringAsync("/api/v1/orders/");

            // Assert
            Assert.Equal("Get Orders Version 1", result);
        }

        [Fact]
        public async Task GetOrders_Version2InUrl_ReturnsVersion2Content()
        {
            // Act
            var result = await _client.GetStringAsync("/api/v2/orders/");

            // Assert
            Assert.Equal("Get Orders Version 2", result);
        }

        [Fact]
        public async Task GetOrders_Version1InUrlAndVersion2InQueryString_ThrowsInvalidOperationException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _client.GetStringAsync("/api/v1/orders/?api-version=2"));
        }

        [Fact]
        public async Task GetOrders_Version1InUrlAndVersion2InAcceptHeader_ThrowsInvalidOperationException()
        {
            // Arrange
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.api-version.v2+json"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _client.GetStringAsync("/api/v1/orders/"));
        }

        [Fact]
        public async Task GetOrders_Version1InUrlAndVersionI2nCustomHeader_ThrowsInvalidOperationException()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("api-version", "2");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _client.GetStringAsync("/api/v1/orders/"));
        }

        [Fact]
        public async Task GetOrders_NoVersion_ThrowsHttpRequestException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                async () => await _client.GetStringAsync("/api/orders/"));
        }

        [Fact]
        public async Task GetOrder_Version2InUrl_ReturnsVersion2Content()
        {
            // Act
            var result = await _client.GetStringAsync("/api/v2/orders/123");

            // Assert
            Assert.Equal("Get Order Value: 123", result);
        }

        [Fact]
        public async Task PostOrders_VersionI2nUrl_ReturnsVersion2Content()
        {
            // Act
            var response = await _client.PostAsync("/api/v2/orders/", new StringContent(string.Empty));
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Post Orders Version 2", result);
        }
    }
}
