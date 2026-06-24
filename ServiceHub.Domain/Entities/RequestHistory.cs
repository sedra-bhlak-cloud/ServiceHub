namespace ServiceHub.Domain.Entities
{
    public class RequestHistory
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        
        // Using '?' tells the compiler these can be null
        public ServiceRequest? Request { get; set; } 
        public string? ChangedBy { get; set; }
        public string? Action { get; set; }
        public string? Details { get; set; }
       public string? Comment { get; set; }
       public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
    }
}