namespace ServiceHub.Web.DTOs
{
    public class ServiceRequestResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        // أضف هذا السطر هنا ليقبل الـ View الخاصية دون أخطاء
        public string Priority { get; set; } = string.Empty; 
        
        public string CreatedAt { get; set; } = string.Empty;
    }
}