using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Testing;
using AspNetRouteVersions.TestWebApp;
using Xunit;

namespace AspNetRouteVersions.IntegrationTests
{
    public class CustomersController : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public CustomersController(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCustomers_Version1InUrl_ReturnsVersion1Content()
        {
            // Act
            var result = await _client.GetStringAsync("/api/v1/customers/");

            // Assert
            Assert.Equal("Get Customers Version 1", result);
        }

        [Fact]
        public async Task GetCustomers_Version2InUrl_ReturnsVersion2Content()
        {
            // Act
            var result = await _client.GetStringAsync("/api/v2/customers/");

            // Assert
            Assert.Equal("Get Customers Version 2", result);
        }

        [Fact]
        public async Task GetCustomers_Version2InQueryString_ReturnsVersion2Content()
        {
            // Act
            var result = await _client.GetStringAsync("/api/customers/?api-version=2");

            // Assert
            Assert.Equal("Get Customers Version 2", result);

        }

        [Fact]
        public async Task GetCustomers_Version2InAcceptHeader_ReturnsVersion2Content()
        {
            // Arrange
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.api-version.v2+json"));

            // Act
            var result = await _client.GetStringAsync("/api/customers/");

            // Assert
            Assert.Equal("Get Customers Version 2", result);
        }

        [Fact]
        public async Task GetCustomers_VersionI2nCustomHeader_ReturnsVersion2Content()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("api-version", "2");

            // Act
            var result = await _client.GetStringAsync("/api/customers/");

            // Assert
            Assert.Equal("Get Customers Version 2", result);
        }

        [Fact]
        public async Task GetCustomers_NoVersion_ReturnsDefaultContent()
        {
            // Act
            var result = await _client.GetStringAsync("/api/customers/");

            // Assert
            Assert.Equal("Get Customers Version 3", result);
        }

        [Fact]
        public async Task GetCustomer_NoVersionRoute_ReturnsContent()
        {
            // Act
            var result = await _client.GetStringAsync("/api/customers/123");

            // Assert
            Assert.Equal("Get Customer Value: 123", result);
        }

        [Fact]
        public async Task PostCustomer_VersionI2nCustomHeader_ReturnsVersion2Content()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("api-version", "2");

            // Act
            var response = await _client.PostAsync("/api/customers/", new StringContent(string.Empty));
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Post Customers Version 2", result);
        }
    }
}
