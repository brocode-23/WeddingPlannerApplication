using Microsoft.AspNetCore.Mvc;

namespace WeddingPlannerApplication.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
