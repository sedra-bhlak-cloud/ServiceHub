using System;
using System.ComponentModel.DataAnnotations;
using ServiceHub.Domain.Enums;

namespace ServiceHub.Domain.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        public RequestType RequestType { get; set; }
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.New;

        public string RequesterId { get; set; } = string.Empty;
        public string? AssignedToId { get; set; } // Keep only the string ID

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Navigation Properties (Keep these, they refer to your domain entities)
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public List<RequestHistory> RequestHistories { get; set; } = new List<RequestHistory>();
    }
}