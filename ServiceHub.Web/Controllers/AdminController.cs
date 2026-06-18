using Microsoft.AspNetCore.Mvc;
using ServiceHub.Web.Services;

namespace ServiceHub.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IServiceRequestService _requestService;

        public AdminController(IServiceRequestService requestService)
        {
            _requestService = requestService;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var data = await _requestService.GetAdminDashboardDataAsync();
            return View(data);
        }
    }
}