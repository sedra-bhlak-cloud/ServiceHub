namespace ServiceHub.Web.DTOs
{
    public class KnowledgeArticleCreateUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}