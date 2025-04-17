using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            DateTime? bookingStartDate,
            DateTime? bookingEndDate,
            DateTime? weddingStartDate,
            DateTime? weddingEndDate,
            int? vendorId,
            string? status,
            string? paymentStatus)
        {
            var vendors = await _context.Vendors.ToListAsync();
            ViewBag.Vendors = vendors;

            var bookings = _context.Bookings
                .Include(b => b.Vendor)
                .Include(b => b.Payments)
                .Include(b => b.Reviews)
                .Where(b => !b.IsDeleted);

            if (bookingStartDate.HasValue)
                bookings = bookings.Where(b => b.BookingDate >= bookingStartDate.Value);

            if (bookingEndDate.HasValue)
                bookings = bookings.Where(b => b.BookingDate <= bookingEndDate.Value);

            if (vendorId.HasValue && vendorId > 0)
                bookings = bookings.Where(b => b.VendorId == vendorId.Value);

            if (!string.IsNullOrEmpty(status))
                bookings = bookings.Where(b => b.Status == status);

            if (!string.IsNullOrEmpty(paymentStatus))
                bookings = bookings.Where(b => b.PaymentStatus == paymentStatus);

            return View(await bookings.ToListAsync());
        }
    }
}

