using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    public class PlannerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlannerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAvailablePlanners(int coupleId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var existing = _context.WeddingPlanners
                    .FirstOrDefault(wp => wp.CoupleId == coupleId && !wp.IsDeleted);

                if (existing != null)
                {
                    var planner = _context.Users
                        .OfType<ApplicationUser>()
                        .FirstOrDefault(u => u.Id == existing.PlannerUserId && !u.IsDeleted);

                    if (planner == null)
                        return NotFound("Assigned planner not found.");

                    return Ok(new
                    {
                        alreadyAssigned = true,
                        assignedPlanner = new
                        {
                            planner.Id,
                            Name = planner.FirstName + " " + planner.LastName,
                            planner.Email
                        }
                    });
                }

                var query = _context.Users
                    .OfType<ApplicationUser>()
                    .Where(u => u.Role == "Planner" && !u.IsDeleted)
                    .Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName, u.Email });

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { alreadyAssigned = false, TotalCount = totalCount, planners = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AssignPlanner([FromBody] AssignPlannerViewModel data)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(data.PlannerUserId))
                return BadRequest("Invalid planner data.");

            try
            {
                var exists = _context.WeddingPlanners
                    .FirstOrDefault(w => w.CoupleId == data.CoupleId && !w.IsDeleted);
                if (exists != null)
                    return BadRequest("A planner has already been assigned. Please remove the current planner before assigning a new one.");

                var newAssignment = new WeddingPlanner
                {
                    CoupleId = data.CoupleId,
                    PlannerUserId = data.PlannerUserId,
                    AssignedDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.WeddingPlanners.Add(newAssignment);
                _context.SaveChanges();

                return Ok("Planner assigned successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult RemovePlanner(int coupleId)
        {
            try
            {
                var planner = _context.WeddingPlanners
                    .FirstOrDefault(w => w.CoupleId == coupleId && !w.IsDeleted);

                if (planner == null)
                    return NotFound("No planner is currently assigned to this couple.");

                planner.IsDeleted = true;
                planner.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok("Planner removed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class AssignPlannerViewModel
    {
        public int CoupleId { get; set; }
        public string PlannerUserId { get; set; }
    }
}
