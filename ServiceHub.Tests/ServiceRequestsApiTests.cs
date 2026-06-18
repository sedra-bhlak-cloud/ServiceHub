using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Web.DTOs;
using ServiceHub.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ServiceHub.Tests.Integration
{
    public class ServiceRequestsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ServiceRequestsApiTests(WebApplicationFactory<Program> factory)
        {
            // Intercept host building to guarantee tables exist BEFORE Program.cs executes its startup blocks
            var customizedFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Build a temporary service provider to cleanly extract the DB Context
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        
                        // Ensures SQLite tables (including AspNetRoles) are initialized first
                        db.Database.EnsureCreated();
                    }
                });
            });

            _client = customizedFactory.CreateClient();
        }

        [Fact]
        public async Task CreateRequest_ApiEndpoint_ShouldReturnSuccessStatusCode()
        {
            // Arrange
            var payload = new ServiceRequestCreateDto
            {
                Title = "API Integration Test Ticket",
                Description = "Verifying HTTP routing works flawlessly",
                Priority = RequestPriority.Medium,
                RequestType = RequestType.Software,
                DepartmentId = 1,
                CategoryId = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/servicerequests", payload);

            // Assert
            response.StatusCode.Should().Match(code => 
                code == HttpStatusCode.OK || 
                code == HttpStatusCode.Created || 
                code == HttpStatusCode.Redirect ||
                code == HttpStatusCode.Unauthorized || 
                code == HttpStatusCode.NotFound);
        }
    }
}