using Microsoft.EntityFrameworkCore;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Domain.Entities;
using ServiceHub.Domain.Enums;
using ServiceHub.Web.DTOs;

namespace ServiceHub.Web.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequestResponseDto>> GetAllRequestsAsync(string? search, string? status, string? priority, string? sortBy)
        {
            var query = _context.ServiceRequests.AsQueryable();

            // 1. Handle Text Search Filtering
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Title.Contains(search) || r.Description.Contains(search));
            }

            // 2. FIXED: Handle Status Filtering Safely
            if (!string.IsNullOrEmpty(status))
            {
                // Force both UI variations ("InProgress" or "Assigned") to map cleanly to your domain enum
                if (status == "InProgress" || status == "Assigned")
                {
                    query = query.Where(r => r.Status == RequestStatus.Assigned);
                }
                else if (Enum.TryParse<RequestStatus>(status, out var parsedStatus))
                {
                    query = query.Where(r => r.Status == parsedStatus);
                }
            }

            // 3. Handle Priority Filtering
            if (!string.IsNullOrEmpty(priority) && Enum.TryParse<RequestPriority>(priority, out var parsedPriority))
            {
                query = query.Where(r => r.Priority == parsedPriority);
            }

            // 4. Handle Sorting Pipelines
            query = sortBy switch
            {
                "date_asc" => query.OrderBy(r => r.CreatedAt),
                "priority_desc" => query.OrderByDescending(r => r.Priority),
                "title_asc" => query.OrderBy(r => r.Title),
                _ => query.OrderByDescending(r => r.CreatedAt) // Default: Newest first
            };

            // 5. Project into DTO format for the View
            return await query.Select(r => new ServiceRequestResponseDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status.ToString(),
                Priority = r.Priority.ToString(), 
                CreatedAt = r.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            }).ToListAsync();
        }

        public async Task<ServiceRequestResponseDto?> GetRequestByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Where(r => r.Id == id)
                .Select(r => new ServiceRequestResponseDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Status = r.Status.ToString(),
                    Priority = r.Priority.ToString(), 
                    CreatedAt = r.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateRequestAsync(ServiceRequestCreateDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
    {
        throw new ArgumentException("Title cannot be empty.", nameof(dto.Title));
    }
            var newRequest = new ServiceRequest
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                RequestType = dto.RequestType,
                DepartmentId = dto.DepartmentId,
                CategoryId = dto.CategoryId,
                Status = RequestStatus.New,
                CreatedAt = DateTime.Now,
                RequesterId = userId
            };

            _context.ServiceRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return newRequest.Id;
        }

        public async Task<bool> UpdateRequestAsync(int id, ServiceRequestUpdateDto dto)
        {
            var existingRequest = await _context.ServiceRequests.FindAsync(id);
            if (existingRequest == null) return false;

            existingRequest.Title = dto.Title;
            existingRequest.Description = dto.Description;
            existingRequest.Priority = dto.Priority;
            existingRequest.AssignedToId = dto.AssignedToId;

            if (dto.Status == RequestStatus.Closed)
            {
                existingRequest.Status = RequestStatus.Closed;
                existingRequest.ClosedAt = DateTime.UtcNow; // Automatically set closed timestamp when status changes to Closed
            }
            else
            {
                existingRequest.Status = dto.Status;
                existingRequest.ClosedAt = null;
            }

            var affectedRows = await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRequestAsync(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return false;

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardDataAsync()
        {
            var totalRequests = await _context.ServiceRequests.CountAsync();
            
            var newRequests = await _context.ServiceRequests
                .CountAsync(r => r.Status == RequestStatus.New);
                
            // Safe fallback: Counts everything that isn't brand new or fully completed/resolved
            var pendingRequests = await _context.ServiceRequests
                .CountAsync(r => r.Status != RequestStatus.New && r.Status != RequestStatus.Resolved);
                
            var resolvedRequests = await _context.ServiceRequests
                .CountAsync(r => r.Status == RequestStatus.Resolved);

            var recentRequests = await _context.ServiceRequests
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new ServiceRequestResponseDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Status = r.Status.ToString(),
                    Priority = r.Priority.ToString(),
                    CreatedAt = r.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                })
                .ToListAsync();

            return new AdminDashboardDto
            {
                TotalRequests = totalRequests,
                NewRequests = newRequests,
                PendingRequests = pendingRequests,
                ResolvedRequests = resolvedRequests,
                RecentRequests = recentRequests
            };
        }

        // 1. Enforce that an ordinary employee can only access their own requests
        public async Task<ServiceRequest?> GetRequestByIdForUserAsync(int id, string userId)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return null;

            // Security check: If the user is not the creator, block access (return null)
            if (request.RequesterId != userId)
            {
                return null; 
            }

            return request;
        }

        // 2. Return articles that are published but NOT archived
        public async Task<IEnumerable<KnowledgeArticle>> GetAvailableArticlesAsync()
        {
            return await _context.KnowledgeArticles
                .Where(a => a.IsPublished && !a.IsArchived)
                .ToListAsync();
        }
    }
}