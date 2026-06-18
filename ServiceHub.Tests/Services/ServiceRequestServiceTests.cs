using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Moq;
using ServiceHub.Domain.Entities;
using ServiceHub.Domain.Enums;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Web.DTOs;
using ServiceHub.Web.Services;
using Xunit;

namespace ServiceHub.Tests.Services
{
    public class ServiceRequestServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ServiceRequestService _service;

        public ServiceRequestServiceTests()
        {
            // Set up a clean, isolated In-Memory database for each individual test run
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new ServiceRequestService(_context);
        }

        [Fact]
        public async Task CreateRequestAsync_WithValidData_ShouldReturnNewRequestId()
        {
            // Arrange
            var dto = new ServiceRequestCreateDto
            {
                Title = "Network Issue",
                Description = " cannot connect to local server",
                Priority = RequestPriority.High,
                RequestType = RequestType.Hardware, // Adjust to match your exact RequestType enum values
                DepartmentId = 1,
                CategoryId = 1
            };
            string mockUserId = "user-123";

            // Act
            int resultId = await _service.CreateRequestAsync(dto, mockUserId);

            // Assert
            resultId.Should().BeGreaterThan(0); // Verifies a new valid ID was generated

            var savedRequest = await _context.ServiceRequests.FindAsync(resultId);
            savedRequest.Should().NotBeNull();
            savedRequest!.Title.Should().Be(dto.Title);
            savedRequest.Status.Should().Be(RequestStatus.New); // Rules state default is New
        }

        [Fact]
        public async Task UpdateRequestAsync_WhenStatusChangesToClosed_ShouldSetClosedAtTimestamp()
        {
            // Arrange - Seed an existing active request into our test database
            var existingRequest = new ServiceRequest
            {
                Id = 99,
                Title = "Printer Jam",
                Description = "Paper jam in office room 3",
                Status = RequestStatus.New,
                Priority = RequestPriority.Low,
                CreatedAt = DateTime.Now.AddDays(-1),
                RequesterId = "user-789",
                DepartmentId = 1
            };
            _context.ServiceRequests.Add(existingRequest);
            await _context.SaveChangesAsync();

            // Prepare the update DTO setting status to Closed
            var updateDto = new ServiceRequestUpdateDto
            {
                Title = "Printer Jam",
                Description = "Paper jam in office room 3",
                Priority = RequestPriority.Low,
                Status = RequestStatus.Closed, // Triggering the closing condition
                AssignedToId = "agent-456"
            };

            // Act
            bool isUpdated = await _service.UpdateRequestAsync(99, updateDto);

            // Assert
            isUpdated.Should().BeTrue();
            
            var updatedRecord = await _context.ServiceRequests.FindAsync(99);
            updatedRecord.Should().NotBeNull();
            updatedRecord!.Status.Should().Be(RequestStatus.Closed);
            updatedRecord.ClosedAt.Should().NotBeNull(); // Verifies business rule is met
            updatedRecord.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}