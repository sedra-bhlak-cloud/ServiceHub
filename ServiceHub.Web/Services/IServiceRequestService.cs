using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceHub.Web.DTOs;

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
        Task<AdminDashboardDto> GetAdminDashboardDataAsync();
    }
}