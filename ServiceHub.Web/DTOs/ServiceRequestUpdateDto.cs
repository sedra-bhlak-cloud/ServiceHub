using ServiceHub.Domain.Enums;

namespace ServiceHub.Web.DTOs
{
    public class ServiceRequestUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public string? AssignedToId { get; set; }
    }
}