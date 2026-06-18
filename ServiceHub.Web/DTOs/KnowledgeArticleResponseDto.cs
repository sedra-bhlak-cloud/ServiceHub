namespace ServiceHub.Web.DTOs
{
    public class KnowledgeArticleResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty; // Send a clean string instead of a category ID integer
        public int Views { get; set; }
    }
}