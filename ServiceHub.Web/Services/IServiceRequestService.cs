using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceHub.Web.DTOs;
using ServiceHub.Domain.Entities;
namespace ServiceHub.Web.Services
{
    public interface IServiceRequestService
    {
        // Accept optional parameters for advanced queries
        Task<IEnumerable<ServiceRequestResponseDto>> GetAllRequestsAsync(
            string? search = null, 
            string? status = null, 
            string? priority = null, 
            string? sortBy = null);

        Task<ServiceRequestResponseDto?> GetRequestByIdAsync(int id);
        Task<int> CreateRequestAsync(ServiceRequestCreateDto dto, string userId);
        Task<bool> UpdateRequestAsync(int id, ServiceRequestUpdateDto dto);
        
        Task<bool> DeleteRequestAsync(int id);
        Task<ServiceRequest?> GetRequestByIdForUserAsync(int id, string userId);
        Task<IEnumerable<KnowledgeArticle>> GetAvailableArticlesAsync();
        Task<AdminDashboardDto> GetAdminDashboardDataAsync();
    }
}