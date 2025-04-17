using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlannerApplication.Data;

namespace WeddingPlannerApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Vendor)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .Where(b => !b.IsDeleted)
                .ToListAsync();

            ViewBag.TotalBookings = bookings.Count;
            ViewBag.TotalRevenue = bookings.Sum(b => b.TotalAmount);
            ViewBag.PendingPayments = bookings.Count(b => b.PaymentStatus == "Pending");
            ViewBag.CancelledBookings = bookings.Count(b => b.Status == "Cancelled");

            return View(bookings);
        }

    }
}



