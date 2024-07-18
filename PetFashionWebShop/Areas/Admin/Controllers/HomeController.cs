using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PetFashionWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class HomeController : Controller
    {
        [Authorize(Policy = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Policy = "Admin")]
        public IActionResult DashboardPage()
        {
            return View();
        }
        public IActionResult Notification()
        {
            return View();
        }
        public IActionResult NotificationValid()
        {
            return View();
        }
    }
}
