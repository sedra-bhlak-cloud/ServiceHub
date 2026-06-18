using System.Collections.Generic;

namespace ServiceHub.Web.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalRequests { get; set; }
        public int NewRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ResolvedRequests { get; set; }
        public List<ServiceRequestResponseDto> RecentRequests { get; set; } = new();
    }
}