using DreamDayWeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;

namespace WeddingPlannerApplication.Controllers
{
    public class CoupleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoupleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checklist()
        {
            var user = await _userManager.GetUserAsync(User);
            var coupleMember = _context.CoupleMembers.FirstOrDefault(cm => cm.UserId == user.Id && !cm.IsDeleted);
            if (coupleMember == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CoupleId = coupleMember.CoupleId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GuestList()
        {
            var user = await _userManager.GetUserAsync(User);
            var coupleMember = _context.CoupleMembers.FirstOrDefault(cm => cm.UserId == user.Id && !cm.IsDeleted);
            if (coupleMember == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CoupleId = coupleMember.CoupleId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Budget()
        {
            var user = await _userManager.GetUserAsync(User);
            var coupleMember = _context.CoupleMembers.FirstOrDefault(cm => cm.UserId == user.Id && !cm.IsDeleted);
            if (coupleMember == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CoupleId = coupleMember.CoupleId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Timeline()
        {
            var user = await _userManager.GetUserAsync(User);
            var coupleMember = _context.CoupleMembers.FirstOrDefault(cm => cm.UserId == user.Id && !cm.IsDeleted);
            if (coupleMember == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CoupleId = coupleMember.CoupleId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Venue()
        {
            var user = await _userManager.GetUserAsync(User);
            var coupleMember = _context.CoupleMembers.FirstOrDefault(cm => cm.UserId == user.Id && !cm.IsDeleted);
            if (coupleMember == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CoupleId = coupleMember.CoupleId;
            return View();
        }
    }
}
