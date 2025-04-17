using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WeddingPlannerApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace WeddingPlannerApplication.Controllers
{
    public class VendorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendor = _context.Vendors.FirstOrDefault(v => v.UserId == user.Id && !v.IsDeleted);
            if (vendor == null)
                return RedirectToAction("Index", "Home");

            ViewBag.VendorId = vendor.Id;
            return View();
        }

        [HttpGet]
        public IActionResult GetDashboardData(int vendorId)
        {
            try
            {
                var vendor = _context.Vendors.FirstOrDefault(v => v.Id == vendorId && !v.IsDeleted);
                if (vendor == null)
                    return NotFound("Vendor not found.");

                var serviceCount = _context.VendorServices.Count(s => s.VendorId == vendorId && !s.IsDeleted);
                var availabilityCount = _context.VendorAvailabilities.Count(a => a.VendorId == vendorId && !a.IsDeleted);
                var reviewCount = _context.Reviews.Count(r => r.VendorId == vendorId && !r.IsDeleted);

                var avgRating = _context.Reviews
                    .Where(r => r.VendorId == vendorId && !r.IsDeleted)
                    .Select(r => (double?)r.Rating)
                    .Average() ?? 0.0;

                return Ok(new
                {
                    totalServices = serviceCount,
                    totalAvailability = availabilityCount,
                    totalReviews = reviewCount,
                    averageRating = Math.Round(avgRating, 1)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Services()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendor = _context.Vendors.FirstOrDefault(v => v.UserId == user.Id && !v.IsDeleted);
            if (vendor == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.VendorId = vendor.Id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Availability()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendor = _context.Vendors.FirstOrDefault(v => v.UserId == user.Id && !v.IsDeleted);
            if (vendor == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.VendorId = vendor.Id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Reviews()
        {
            var user = await _userManager.GetUserAsync(User);
            var vendor = _context.Vendors.FirstOrDefault(v => v.UserId == user.Id && !v.IsDeleted);
            if (vendor == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.VendorId = vendor.Id;
            return View();
        }

        [HttpGet]
        public IActionResult GetAllVendors(int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<Vendor> query = _context.Vendors
                    .Where(v => !v.IsDeleted)
                    .Include(v => v.Category)
                    .Include(v => v.Services)
                    .OrderBy(v => v.Name);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new
                {
                    TotalCount = totalCount,
                    Vendors = query.ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetVendorById(int id)
        {
            try
            {
                var vendor = _context.Vendors
                    .Include(v => v.Services)
                    .Include(v => v.Category)
                    .Include(v => v.Availabilities)
                    .Include(v => v.Venues)
                    .FirstOrDefault(v => v.Id == id && !v.IsDeleted);

                if (vendor == null)
                    return NotFound("Vendor not found.");

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddVendor(Vendor vendor)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {
                vendor.CreatedAt = DateTime.UtcNow;
                vendor.UpdatedAt = DateTime.UtcNow;
                vendor.IsDeleted = false;

                _context.Vendors.Add(vendor);
                _context.SaveChanges();

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateVendor(Vendor vendor)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            try
            {
                var existing = _context.Vendors.FirstOrDefault(v => v.Id == vendor.Id && !v.IsDeleted);
                if (existing == null)
                    return NotFound("Vendor not found.");

                existing.Name = vendor.Name;
                existing.Description = vendor.Description;
                existing.Location = vendor.Location;
                existing.Pricing = vendor.Pricing;
                existing.ProfilePicture = vendor.ProfilePicture;
                existing.CategoryId = vendor.CategoryId;
                existing.ContactEmail = vendor.ContactEmail;
                existing.Phone = vendor.Phone;
                existing.WebsiteUrl = vendor.WebsiteUrl;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteVendor(int id)
        {
            try
            {
                var vendor = _context.Vendors.FirstOrDefault(v => v.Id == id && !v.IsDeleted);
                if (vendor == null)
                    return NotFound("Vendor not found.");

                vendor.IsDeleted = true;
                vendor.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok("Vendor deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllVendorServices(int vendorId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<VendorService> query = _context.VendorServices
                    .Where(s => s.VendorId == vendorId && !s.IsDeleted)
                    .OrderBy(s => s.ServiceName);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, Services = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddVendorService([FromBody] VendorService service)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors)
            //            .Select(e => e.ErrorMessage + " " + e.Exception?.Message)
            //            .ToList();
            //    return BadRequest("Invalid booking data. " + string.Join(" | ", errors));
            //}

            if (!ModelState.IsValid)
                return BadRequest("Invalid service data.");

            try
            {
                service.CreatedAt = DateTime.UtcNow;
                service.UpdatedAt = DateTime.UtcNow;
                service.IsDeleted = false;

                _context.VendorServices.Add(service);
                _context.SaveChanges();

                return Ok(service);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateVendorService([FromBody] VendorService service)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid service data.");

            try
            {
                var existing = _context.VendorServices.FirstOrDefault(s => s.Id == service.Id && !s.IsDeleted);
                if (existing == null)
                    return NotFound("Service not found.");

                existing.ServiceName = service.ServiceName;
                existing.Description = service.Description;
                existing.Price = service.Price;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteVendorService(int id)
        {
            try
            {
                var service = _context.VendorServices.FirstOrDefault(s => s.Id == id && !s.IsDeleted);
                if (service == null)
                    return NotFound("Service not found.");

                service.IsDeleted = true;
                service.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Service deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetVendorAvailability(int vendorId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<VendorAvailability> query = _context.VendorAvailabilities
                    .Where(a => a.VendorId == vendorId && !a.IsDeleted)
                    .OrderBy(a => a.AvailableDate);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, Availability = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddVendorAvailability([FromBody] VendorAvailability availability)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors)
            //            .Select(e => e.ErrorMessage + " " + e.Exception?.Message)
            //            .ToList();
            //    return BadRequest("Invalid booking data. " + string.Join(" | ", errors));
            //}

            if (!ModelState.IsValid)
                return BadRequest("Invalid availability data.");

            try
            {
                availability.CreatedAt = DateTime.UtcNow;
                availability.UpdatedAt = DateTime.UtcNow;
                availability.IsDeleted = false;

                _context.VendorAvailabilities.Add(availability);
                _context.SaveChanges();

                return Ok(availability);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateVendorAvailability([FromBody] VendorAvailability availability)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors)
            //            .Select(e => e.ErrorMessage + " " + e.Exception?.Message)
            //            .ToList();
            //    return BadRequest("Invalid booking data. " + string.Join(" | ", errors));
            //}

            if (!ModelState.IsValid)
                return BadRequest("Invalid availability data.");

            try
            {
                var existing = _context.VendorAvailabilities.FirstOrDefault(a => a.Id == availability.Id && !a.IsDeleted);
                if (existing == null)
                    return NotFound("Availability not found.");

                existing.AvailableDate = availability.AvailableDate;
                existing.FromTime = availability.FromTime;
                existing.ToTime = availability.ToTime;
                existing.Status = availability.Status;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteVendorAvailability(int id)
        {
            try
            {
                var availability = _context.VendorAvailabilities.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
                if (availability == null)
                    return NotFound("Availability not found.");

                availability.IsDeleted = true;
                availability.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Availability deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllVendorCategories(int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<VendorCat> query = _context.VendorCategories;
                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                var categories = query.ToList();

                return Ok(new { TotalCount = totalCount, Categories = categories });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddVendorCategory([FromBody] VendorCat category)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid category data.");

            try
            {
                _context.VendorCategories.Add(category);
                _context.SaveChanges();

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateVendorCategory([FromBody] VendorCat category)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid category data.");

            try
            {
                var existing = _context.VendorCategories.FirstOrDefault(c => c.Id == category.Id);
                if (existing == null)
                    return NotFound("Category not found.");

                existing.Name = category.Name;
                _context.SaveChanges();

                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteVendorCategory(int id)
        {
            try
            {
                var category = _context.VendorCategories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                    return NotFound("Category not found.");

                _context.VendorCategories.Remove(category);
                _context.SaveChanges();

                return Ok("Category deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview([FromBody] Review review)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid review data.");

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var exists = _context.Reviews
                    .Any(r => r.VendorId == review.VendorId && r.UserId == user.Id && !r.IsDeleted);

                if (exists)
                    return Conflict("You have already submitted a review for this vendor.");

                review.UserId = user.Id;
                review.CreatedAt = DateTime.UtcNow;
                review.UpdatedAt = DateTime.UtcNow;
                review.IsDeleted = false;

                _context.Reviews.Add(review);
                _context.SaveChanges();

                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetVendorReviews(int vendorId, int pageNumber = 1, int pageSize = 10, int? minRating = null)
        {
            try
            {
                var query = _context.Reviews
                    .Where(r => r.VendorId == vendorId && !r.IsDeleted);

                if (minRating.HasValue)
                    query = query.Where(r => r.Rating >= minRating.Value);

                var totalCount = query.Count();

                var reviews = query
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new VendorReviewResponseViewModel
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Reviews = reviews
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetVendorReviewsList(int vendorId, int pageNumber = 1, int pageSize = 10, int? minRating = null)
        {
            try
            {
                var query = _context.Reviews
                    .Where(r => r.VendorId == vendorId && !r.IsDeleted);

                if (minRating.HasValue)
                    query = query.Where(r => r.Rating >= minRating.Value);

                var totalCount = query.Count();

                var reviews = query
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Join(
                        _context.Users.OfType<ApplicationUser>(),
                        review => review.UserId,
                        user => user.Id,
                        (review, user) => new
                        {
                            review.Id,
                            review.Comment,
                            review.Rating,
                            review.CreatedAt,
                            FullName = user.FirstName + " " + user.LastName
                        })
                    .ToList();

                return Ok(new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Reviews = reviews
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetVendorAverageRating(int vendorId)
        {
            try
            {
                var avg = _context.Reviews
                    .Where(r => r.VendorId == vendorId && !r.IsDeleted)
                    .Select(r => (double?)r.Rating)
                    .Average();

                return Ok(new { VendorId = vendorId, AverageRating = avg ?? 0.0 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Not logged in.");

                var review = _context.Reviews.FirstOrDefault(r => r.Id == id && !r.IsDeleted);
                if (review == null)
                    return NotFound("Review not found.");

                if (review.UserId != userId)
                    return Forbid("You can only delete your own reviews.");

                review.IsDeleted = true;
                review.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Review deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
