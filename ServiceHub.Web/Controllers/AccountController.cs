using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ServiceHub.Web.Controllers
{
    [AllowAnonymous] 
    public class AccountController : Controller
    {
        public IActionResult Login() => View();
        public IActionResult Register() => View();
    }
}