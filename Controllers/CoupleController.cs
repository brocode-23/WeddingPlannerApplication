using Microsoft.AspNetCore.Mvc;

namespace WeddingPlannerApplication.Controllers
{
    public class CoupleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
