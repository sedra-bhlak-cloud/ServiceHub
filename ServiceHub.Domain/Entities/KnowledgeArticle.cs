using System;

namespace ServiceHub.Domain.Entities
{
    public class KnowledgeArticle
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public bool IsPublished { get; set; }
    public bool IsArchived { get; set; }
        public int Views { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

      
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}