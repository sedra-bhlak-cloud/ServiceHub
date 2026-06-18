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
        }

        // --- NEW TEST 1: Creating a request with an empty title fails validation ---
        [Fact]
        public async Task CreateRequestAsync_WithEmptyTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var dto = new ServiceRequestCreateDto
            {
                Title = "", // Empty to trigger failure
                Description = "Valid description text",
                Priority = RequestPriority.Medium,
                DepartmentId = 1
            };

            // Act
            Func<Task> act = async () => await _service.CreateRequestAsync(dto, "user-123");

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        // --- NEW TEST 2: Employee cannot access another employee request ---
        [Fact]
        public async Task GetRequestByIdForUserAsync_WhenAccessedByDifferentEmployee_ShouldReturnNull()
        {
            // Arrange
            var targetRequest = new ServiceRequest
            {
                Id = 15,
                Title = "Employee A Issue",
                RequesterId = "employee-A",
                DepartmentId = 1
            };
            _context.ServiceRequests.Add(targetRequest);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetRequestByIdForUserAsync(15, "employee-B");

            // Assert
            result.Should().BeNull();
        }

        // --- NEW TEST 3: Archived articles are excluded from default article search ---
        [Fact]
        public async Task GetAvailableArticlesAsync_ShouldExcludeArchivedArticles()
        {
            // Arrange
            var activeArticle = new KnowledgeArticle { Id = 1, Title = "Active Guide", IsPublished = true, IsArchived = false };
            var archivedArticle = new KnowledgeArticle { Id = 2, Title = "Legacy Info", IsPublished = true, IsArchived = true };
            
            _context.KnowledgeArticles.AddRange(activeArticle, archivedArticle);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetAvailableArticlesAsync();

            // Assert
            results.Should().ContainSingle(a => a.Id == 1);
            results.Should().NotContain(a => a.Id == 2);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}