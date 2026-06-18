using System.ComponentModel.DataAnnotations;
using ServiceHub.Domain.Enums; // <-- Make sure to add this import at the top!

namespace ServiceHub.Web.DTOs
{
    public class ServiceRequestCreateDto
    {
        [Required(ErrorMessage = "The request title is mandatory.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department ID is required.")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }

        public string Description { get; set; } = string.Empty;

        // Change these from int to their respective Enum types:
        public RequestPriority Priority { get; set; }
        public RequestType RequestType { get; set; }
    }
}