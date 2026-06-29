using System.Collections.Generic;
using System.Threading.Tasks;
using ServiceHub.Domain.Entities;

namespace ServiceHub.Application.Services
{
    public interface IKnowledgeHubService
    {
        Task<int> CloneTicketToHubAsync(int serviceRequestId);
        Task<List<KnowledgeArticle>> SearchArticlesAsync(string searchTerm, int? categoryId = null);
        Task CreateArticleAsync(KnowledgeArticle article);
    }
}