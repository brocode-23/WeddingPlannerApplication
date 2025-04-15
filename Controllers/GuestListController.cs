using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    public class GuestListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestListController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetGuests(int coupleId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<Guest> query = _context.Guests
                    .Where(g => g.CoupleId == coupleId && !g.IsDeleted)
                    .OrderBy(g => g.LastName);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, Guests = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddGuest([FromBody] Guest guest)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid guest data.");

            try
            {
                guest.CreatedAt = DateTime.UtcNow;
                guest.UpdatedAt = DateTime.UtcNow;
                guest.IsDeleted = false;

                _context.Guests.Add(guest);
                _context.SaveChanges();

                return Ok(guest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateGuest([FromBody] Guest guest)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid update data.");

            try
            {
                var existing = _context.Guests.FirstOrDefault(g => g.Id == guest.Id && !g.IsDeleted);
                if (existing == null)
                    return NotFound("Guest not found.");

                existing.FirstName = guest.FirstName;
                existing.LastName = guest.LastName;
                existing.Email = guest.Email;
                existing.RSVPStatus = guest.RSVPStatus;
                existing.MealPreference = guest.MealPreference;
                existing.Allergies = guest.Allergies;
                existing.SeatingArrangement = guest.SeatingArrangement;
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
        public IActionResult DeleteGuest(int id)
        {
            try
            {
                var guest = _context.Guests.FirstOrDefault(g => g.Id == id && !g.IsDeleted);
                if (guest == null)
                    return NotFound("Guest not found.");

                guest.IsDeleted = true;
                guest.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Guest deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
