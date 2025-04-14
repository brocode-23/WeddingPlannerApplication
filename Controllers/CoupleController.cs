using DreamDayWeddingPlanner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    [Authorize(Roles = "Couple")]
    public class CoupleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoupleController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var coupleMember = _context.CoupleMembers.FirstOrDefault(cm => cm.UserId == user.Id && !cm.IsDeleted);
            if (coupleMember == null)
                return RedirectToAction("Index", "Home");

            ViewBag.CoupleId = coupleMember.CoupleId;
            return View();
        }

        [HttpGet]
        public IActionResult GetDashboardData(int coupleId)
        {
            try
            {
                var couple = _context.Couples.FirstOrDefault(c => c.Id == coupleId && !c.IsDeleted);
                if (couple == null)
                    return NotFound("Couple not found.");

                var guestCount = _context.Guests.Count(g => g.CoupleId == coupleId && !g.IsDeleted);

                var spent = _context.WeddingBudgets
                    .Where(b => b.CoupleId == coupleId && !b.IsDeleted)
                    .Sum(b => b.SpentAmount);

                var budgetLeft = couple.Budget - spent;

                var totalTasks = _context.WeddingChecklists.Count(t => t.CoupleId == coupleId && !t.IsDeleted);
                var completedTasks = _context.WeddingChecklists.Count(t => t.CoupleId == coupleId && t.TaskStatus == "Completed" && !t.IsDeleted);
                var percent = totalTasks == 0 ? 0 : (int)Math.Round((double)completedTasks / totalTasks * 100);

                var upcomingBookings = _context.Bookings
                    .Where(b => b.CoupleId == coupleId/* && b.BookingDate >= DateTime.UtcNow*/ && b.Status != "Cancelled" && !b.IsDeleted)
                    //.OrderBy(b => b.BookingDate)
                    .OrderByDescending(b => b.BookingDate)
                    .Include(b => b.Vendor)
                    .Take(5)
                    .Select(b => new
                    {
                        b.ServiceDetails,
                        b.BookingDate,
                        b.Status,
                        vendorName = b.Vendor.Name
                    }).ToList();

                return Ok(new
                {
                    weddingDate = couple.WeddingDate.ToString("yyyy-MM-dd"),
                    totalGuests = guestCount,
                    budgetLeft,
                    totalBudget = couple.Budget,
                    checklistPercent = percent,
                    upcomingBookings
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UpdateWeddingDetails([FromBody] Couple updatedData)
        {
            try
            {
                var couple = _context.Couples.FirstOrDefault(c => c.Id == updatedData.Id && !c.IsDeleted);
                if (couple == null)
                    return NotFound("Couple not found.");

                couple.WeddingDate = updatedData.WeddingDate;
                couple.Budget = updatedData.Budget;
                couple.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok(new { message = "Wedding details updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

        [HttpGet]
        public async Task<IActionResult> Vendors()
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
