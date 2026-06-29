using Microsoft.EntityFrameworkCore;
using ServiceHub.Application.Services;
using ServiceHub.Domain.Entities;
using ServiceHub.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceHub.Infrastructure.Services
{
    public class KnowledgeHubService : IKnowledgeHubService
    {
        private readonly ApplicationDbContext _context;

        public KnowledgeHubService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Added optional int? categoryId parameter to allow category filtering
        public async Task<List<KnowledgeArticle>> SearchArticlesAsync(string searchTerm, int? categoryId = null)
        {
            var query = _context.KnowledgeArticles.AsQueryable();

            // 1. Filter by category if one is selected
            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            // 2. Filter by search term if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) || a.Content.Contains(searchTerm));
            }

            return await query.ToListAsync();
        }

        public async Task<int> CloneTicketToHubAsync(int ticketId)
        {
            var ticket = await _context.ServiceRequests
                .FirstOrDefaultAsync(r => r.Id == ticketId);

            if (ticket == null)
                throw new Exception("Service Request ticket not found.");

            string problemDescription = !string.IsNullOrWhiteSpace(ticket.Description)
                ? ticket.Description
                : "No description was provided in the original ticket.";

            string officialResolution = !string.IsNullOrWhiteSpace(ticket.Resolution)
                ? ticket.Resolution
                : "No resolution details were provided in the original ticket.";

            string articleContent = $"### Problem Description\n{problemDescription}\n\n### Official Resolution\n{officialResolution}";

            string expectedTitle = $"FAQ: {ticket.Title}";
            var existingArticle = await _context.KnowledgeArticles
                .FirstOrDefaultAsync(a => a.Title == expectedTitle);

            // Kept your recent fix exactly as it is to prevent duplication:
            if (existingArticle != null)
            {
                existingArticle.Content = articleContent;
                await _context.SaveChangesAsync();
                return existingArticle.Id;
            }

            var newArticle = new KnowledgeArticle
            {
                Title = expectedTitle,
                Content = articleContent,
                IsPublished = true,
                IsArchived = false,
                Views = 0,
                CreatedAt = DateTime.UtcNow,
                CategoryId = ticket.CategoryId
            };

            await _context.KnowledgeArticles.AddAsync(newArticle);
            await _context.SaveChangesAsync();

            return newArticle.Id;
        }
        public async Task CreateArticleAsync(KnowledgeArticle article)
{
    article.CreatedAt = DateTime.UtcNow;
    await _context.KnowledgeArticles.AddAsync(article);
    await _context.SaveChangesAsync();
}
    }
}