using Microsoft.AspNetCore.Mvc;

namespace CFT_Solutions.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}