using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Infrastructure.Data;
using ServiceHub.Web.DTOs;
using ServiceHub.Domain.Entities;

namespace ServiceHub.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeArticlesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public KnowledgeArticlesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/KnowledgeArticlesApi
        [HttpGet]
        public async Task<IActionResult> GetArticles()
        {
            var articles = await _context.KnowledgeArticles
                .Select(a => new KnowledgeArticleResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CategoryName = a.Category != null ? a.Category.Name : "Uncategorized",
                    Views = a.Views
                })
                .ToListAsync();

            return Ok(articles);
        }

        // 2. GET: api/KnowledgeArticlesApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _context.KnowledgeArticles
                .Where(a => a.Id == id)
                .Select(a => new KnowledgeArticleResponseDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CategoryName = a.Category != null ? a.Category.Name : "Uncategorized",
                    Views = a.Views
                })
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound(new { message = $"Article with ID {id} was not found." });
            }

            var rawArticle = await _context.KnowledgeArticles.FindAsync(id);
            if (rawArticle != null)
            {
                rawArticle.Views++;
                await _context.SaveChangesAsync();
            }

            return Ok(article);
        }

        // 3. POST: api/KnowledgeArticlesApi
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] KnowledgeArticleCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newArticle = new KnowledgeArticle
            {
                Title = dto.Title,
                Content = dto.Content,
                CategoryId = dto.CategoryId,
                Views = 0,
                CreatedAt = DateTime.Now
            };

            _context.KnowledgeArticles.Add(newArticle);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Article created successfully!", id = newArticle.Id });
        }

        // 4. PUT: api/KnowledgeArticlesApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] KnowledgeArticleCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingArticle = await _context.KnowledgeArticles.FindAsync(id);
            if (existingArticle == null)
            {
                return NotFound(new { message = $"Article with ID {id} was not found." });
            }

            existingArticle.Title = dto.Title;
            existingArticle.Content = dto.Content;
            existingArticle.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Article updated successfully!" });
        }

        // 5. DELETE: api/KnowledgeArticlesApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.KnowledgeArticles.FindAsync(id);
            if (article == null)
            {
                return NotFound(new { message = $"Article with ID {id} was not found." });
            }

            _context.KnowledgeArticles.Remove(article);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Article deleted successfully!" });
        }
    }
}