using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ServiceHub.Domain.Entities;
using ServiceHub.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ServiceHub.Domain.Enums;
using ServiceHub.Web.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ServiceHub.Web.Controllers
{
    [Authorize]
    public class ServiceRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IServiceRequestService _requestService;

     
        public ServiceRequestController(
            ApplicationDbContext context, 
            UserManager<IdentityUser> userManager, 
            IServiceRequestService requestService)
        {
            _context = context;
            _userManager = userManager;
            _requestService = requestService;
        }

        // GET: /ServiceRequest
        public async Task<IActionResult> Index(string? search, string? status, string? priority, string? sortBy)
        {
            // Keep parameters alive in the UI form elements
            ViewData["CurrentSearch"] = search;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentPriority"] = priority;
            ViewData["CurrentSort"] = sortBy;

            // Fetch the data matching the precise pipeline parameters
            var requests = await _requestService.GetAllRequestsAsync(search, status, priority, sortBy);
            
            return View(requests);
        }

        // GET: /ServiceRequest/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: /ServiceRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest request)
        {
           request.RequesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
request.CreatedAt = DateTime.UtcNow;

if (ModelState.IsValid)
{
    _context.ServiceRequests.Add(request);
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(request);
        }

        // GET: /ServiceRequest/Edit/5
        [Authorize(Roles = "Admin,SupportAgent")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            var agents = await _userManager.GetUsersInRoleAsync("SupportAgent");
            ViewBag.SupportAgents = new SelectList(agents, "Id", "UserName", request.AssignedToId);
            return View(request);
        }

        // POST: /ServiceRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,SupportAgent")]
        public async Task<IActionResult> Edit(int id, ServiceRequest request, string? userComment)
        {
            if (id != request.Id) return NotFound();

            var existing = await _context.ServiceRequests.FindAsync(id);
            if (existing == null) return NotFound();

            string oldStatus = existing.Status.ToString();
            string? oldAgent = existing.AssignedToId;

            if (existing.Status == RequestStatus.New && !string.IsNullOrEmpty(request.AssignedToId))
            {
                existing.Status = RequestStatus.Assigned;
            }
            else
            {
                existing.Status = request.Status;
            }

            existing.Title = request.Title;
            existing.Description = request.Description;
            existing.Resolution = request.Resolution;
            existing.AssignedToId = request.AssignedToId;
            existing.CreatedAt = DateTime.SpecifyKind(existing.CreatedAt, DateTimeKind.Utc);

            string agentDisplay = "None";
            if (!string.IsNullOrEmpty(existing.AssignedToId))
            {
                var agentUser = await _userManager.FindByIdAsync(existing.AssignedToId);
                agentDisplay = agentUser?.Email ?? agentUser?.UserName ?? existing.AssignedToId;
            }

            if (oldStatus != existing.Status.ToString() || oldAgent != existing.AssignedToId || !string.IsNullOrEmpty(userComment))
            {
               var history = new RequestHistory
{
    RequestId = existing.Id,
    ChangedBy = User.Identity?.Name ?? "System",
    Action = "Updated",
    Details = $"Status: {oldStatus} → {existing.Status}. Agent: {agentDisplay}." +
              (string.IsNullOrEmpty(userComment) ? "" : $" Note: {userComment}"),
    ChangeDate = DateTime.UtcNow
};
                _context.RequestHistories.Add(history);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /ServiceRequest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var request = await _context.ServiceRequests
                .Include(s => s.Category)
                .Include(s => s.Department)
                .Include(s => s.RequestHistories)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (request == null) return NotFound();

            request.RequestHistories = request.RequestHistories?
                .OrderByDescending(h => h.ChangeDate)
                .ToList() ?? new List<RequestHistory>();

            var users = await _context.Users.ToListAsync();
            ViewBag.UserLookup = users
                .Where(u => u.Email != null)
                .ToDictionary(u => u.Email!, u => u.Email!);

            return View(request);
        }

        // GET: /ServiceRequest/Delete/5
       [Authorize(Roles = "Admin")]
public async Task<IActionResult> Delete(int? id)
{
    if (id == null) return NotFound();

    // Fixed: Properly using FirstOrDefaultAsync to fetch the single matching record
    var request = await _context.ServiceRequests
        .Include(s => s.Category)
        .Include(s => s.Department)
        .FirstOrDefaultAsync(m => m.Id == id); 

    if (request == null) return NotFound();

    return View(request);
}

        // POST: /ServiceRequest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request != null)
            {
                _context.ServiceRequests.Remove(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}