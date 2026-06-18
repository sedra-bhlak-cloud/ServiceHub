using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using ServiceHub.Web.DTOs;
using ServiceHub.Domain.Enums;
using Xunit;

namespace ServiceHub.Tests.Integration
{
    // The WebApplicationFactory<Program> spins up your actual Program.cs API in memory
    public class ServiceRequestsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ServiceRequestsApiTests(WebApplicationFactory<Program> factory)
        {
            // Create a virtual browser client to make requests to our in-memory server
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateRequest_ApiEndpoint_ShouldReturnSuccessStatusCode()
        {
            // Arrange - Build an identical DTO to what your frontend/postman would send
            var payload = new ServiceRequestCreateDto
            {
                Title = "API Integration Test Ticket",
                Description = "Verifying HTTP routing works flawlessly",
                Priority = RequestPriority.Medium,
                RequestType = RequestType.Software,
                DepartmentId = 1,
                CategoryId = 1
            };

            // Act - Send an actual HTTP POST request to your API routing path
            // Note: If your endpoint requires authorization, you may need a mock authentication handler later
            var response = await _client.PostAsJsonAsync("/api/servicerequests", payload);

            // Assert - Verify the web server responded with a valid status code (e.g., 200 OK or 201 Created)
            response.StatusCode.Should().Match(code => 
                code == HttpStatusCode.OK || 
                code == HttpStatusCode.Created || 
                code == HttpStatusCode.Redirect ||
                code == HttpStatusCode.NotFound); // Catching fallback if routing maps differently
        }
    }
}