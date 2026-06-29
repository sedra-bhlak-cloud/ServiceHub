using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Application.Services;
using ServiceHub.Infrastructure.Data;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceHub.Domain.Entities;

namespace ServiceHub.Web.Controllers
{
    public class KnowledgeHubController : Controller
    {
        private readonly IKnowledgeHubService _knowledgeHubService;
        private readonly ApplicationDbContext _context;

        public KnowledgeHubController(IKnowledgeHubService knowledgeHubService, ApplicationDbContext context)
        {
            _knowledgeHubService = knowledgeHubService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm, int? categoryId)
        {
            ViewData["CurrentSearch"] = searchTerm;
            ViewData["CurrentCategory"] = categoryId;

            ViewBag.Categories = await _context.Categories.ToListAsync();

            var articles = await _knowledgeHubService.SearchArticlesAsync(searchTerm ?? "", categoryId);

            return View(articles);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,SupportAgent")]
        public async Task<IActionResult> CloneFromTicket(int ticketId)
        {
            try
            {
                int newArticleId = await _knowledgeHubService.CloneTicketToHubAsync(ticketId);
                TempData["SuccessMessage"] = "Ticket successfully cloned to Knowledge Hub as a draft!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "ServiceRequest", new { id = ticketId });
            }
        }
        [HttpGet]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Create()
{
    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
    return View();
}

[HttpPost]
[Authorize(Roles = "Admin")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(KnowledgeArticle article)
{
    if (ModelState.IsValid)
    {
        article.IsPublished = true;
        article.IsArchived = false;
        await _knowledgeHubService.CreateArticleAsync(article);
        TempData["SuccessMessage"] = "FAQ article created successfully.";
        return RedirectToAction(nameof(Index));
    }
    ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
    return View(article);
}
    }
}