using Microsoft.AspNetCore.Mvc;

namespace WeddingPlannerApplication.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}



