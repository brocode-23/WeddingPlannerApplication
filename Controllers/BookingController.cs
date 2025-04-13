using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;
using Microsoft.EntityFrameworkCore;
using WeddingPlannerApplication.ViewModels;

namespace WeddingPlannerApplication.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateVenueBooking([FromBody] Booking booking)
        {
            if (booking.CoupleId <= 0 || booking.ServiceId <= 0)
                return BadRequest("Invalid booking data.");

            try
            {
                var existing = _context.Bookings.FirstOrDefault(b =>
                    b.CoupleId == booking.CoupleId &&
                    !b.IsDeleted &&
                    b.Status != "Cancelled" &&
                    _context.Venues.Any(v => v.Id == b.ServiceId));

                if (existing != null)
                    return BadRequest("You already have a booked venue. Please cancel it first.");

                booking.Status = "Confirmed";
                booking.PaymentStatus = "Pending";
                booking.CreatedAt = DateTime.UtcNow;
                booking.UpdatedAt = DateTime.UtcNow;
                booking.IsDeleted = false;

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetBookings(int coupleId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<Booking> query = _context.Bookings
                    .Include(b => b.Payments)
                    .Include(b => b.Reviews)
                    .Where(b => b.CoupleId == coupleId && !b.IsDeleted)
                    .OrderByDescending(b => b.BookingDate);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, Bookings = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetVenueBooking(int coupleId)
        {
            try
            {
                var booking = _context.Bookings
                    .Where(b => b.CoupleId == coupleId && !b.IsDeleted && b.Status != "Cancelled")
                    .OrderByDescending(b => b.CreatedAt)
                    .Select(b => new VenueBookingViewModel
                    {
                        Id = b.Id,
                        ServiceId = b.ServiceId,
                        Status = b.Status,
                        PaymentStatus = b.PaymentStatus
                    })
                    .FirstOrDefault();

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid booking data.");

            try
            {
                booking.CreatedAt = DateTime.UtcNow;
                booking.UpdatedAt = DateTime.UtcNow;
                booking.IsDeleted = false;

                _context.Bookings.Add(booking);
                _context.SaveChanges();

                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateBookingStatus(int id, string status, string paymentStatus)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
                if (booking == null)
                    return NotFound("Booking not found.");

                booking.Status = status;
                booking.PaymentStatus = paymentStatus;
                booking.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult CancelBooking(int id)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
                if (booking == null)
                    return NotFound("Booking not found.");

                booking.IsDeleted = true;
                booking.Status = "Cancelled";
                booking.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Booking cancelled.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult MakePayment([FromBody] Payment payment)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid payment data.");

            try
            {
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.IsDeleted = false;

                _context.Payments.Add(payment);

                var booking = _context.Bookings.FirstOrDefault(b => b.Id == payment.BookingId && !b.IsDeleted);
                if (booking != null)
                {
                    booking.PaymentStatus = payment.PaymentStatus;
                    booking.UpdatedAt = DateTime.UtcNow;
                }

                _context.SaveChanges();

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetPayments(int coupleId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<Payment> query = _context.Payments
                    .Include(p => p.Booking)
                    .Where(p => !p.IsDeleted && p.Booking.CoupleId == coupleId)
                    .OrderByDescending(p => p.PaymentDate);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, Payments = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeletePayment(int id)
        {
            try
            {
                var payment = _context.Payments.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
                if (payment == null)
                    return NotFound("Payment not found.");

                payment.IsDeleted = true;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Payment deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
