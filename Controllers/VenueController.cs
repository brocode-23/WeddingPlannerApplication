using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlannerApplication.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenueController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetVenuesFiltered(string? address, string? type, int? capacity)
        {
            try
            {
                var query = _context.Venues.Where(v => !v.IsDeleted);

                if (!string.IsNullOrEmpty(address))
                    query = query.Where(v => v.Address.Contains(address));

                if (!string.IsNullOrEmpty(type))
                    query = query.Where(v => v.IndoorOutdoor == type);

                if (capacity.HasValue)
                    query = query.Where(v => v.Capacity >= capacity.Value);

                return Ok(query.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult HasActiveVenueBooking(int coupleId)
        {
            try
            {
                var exists = _context.Bookings.Any(b =>
                    b.CoupleId == coupleId &&
                    !b.IsDeleted &&
                    b.Status != "Cancelled" &&
                    _context.Venues.Any(v => v.Id == b.ServiceId));

                return Ok(exists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllVenues(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var venues = _context.Venues
                    .Where(v => !v.IsDeleted)
                    .Include(v => v.Bookings)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(venues);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetVenueById(int id)
        {
            try
            {
                var venue = _context.Venues
                    .Include(v => v.Bookings)
                    .FirstOrDefault(v => v.Id == id && !v.IsDeleted);

                if (venue == null)
                    return NotFound("Venue not found.");

                return Ok(venue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddVenue([FromBody] Venue venue)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid venue data.");

            try
            {
                venue.CreatedAt = DateTime.UtcNow;
                venue.UpdatedAt = DateTime.UtcNow;
                venue.IsDeleted = false;

                _context.Venues.Add(venue);
                _context.SaveChanges();

                return Ok(venue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateVenue([FromBody] Venue venue)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid venue data.");

            try
            {
                var existing = _context.Venues.FirstOrDefault(v => v.Id == venue.Id && !v.IsDeleted);
                if (existing == null)
                    return NotFound("Venue not found.");

                existing.Address = venue.Address;
                existing.Latitude = venue.Latitude;
                existing.Longitude = venue.Longitude;
                existing.Capacity = venue.Capacity;
                existing.IndoorOutdoor = venue.IndoorOutdoor;
                existing.Amenities = venue.Amenities;
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
        public IActionResult DeleteVenue(int id)
        {
            try
            {
                var venue = _context.Venues.FirstOrDefault(v => v.Id == id && !v.IsDeleted);
                if (venue == null)
                    return NotFound("Venue not found.");

                venue.IsDeleted = true;
                venue.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Venue deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
