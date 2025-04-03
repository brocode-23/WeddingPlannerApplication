using Microsoft.AspNetCore.Mvc;

namespace WeddingPlannerApplication.Controllers
{
    public class PlannerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
