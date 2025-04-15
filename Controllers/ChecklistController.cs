using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    public class ChecklistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChecklistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetChecklist(int coupleId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<WeddingChecklist> query = _context.WeddingChecklists
                    .Where(t => t.CoupleId == coupleId && !t.IsDeleted)
                    .OrderBy(t => t.DueDate);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, Checklist = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddTask([FromBody] WeddingChecklist task)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid checklist task.");

            try
            {
                task.CreatedAt = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;
                task.IsDeleted = false;

                _context.WeddingChecklists.Add(task);
                _context.SaveChanges();

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateTask([FromBody] WeddingChecklist task)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid update data.");

            try
            {
                var existing = _context.WeddingChecklists.FirstOrDefault(t => t.Id == task.Id && !t.IsDeleted);
                if (existing == null)
                    return NotFound("Checklist task not found.");

                existing.TaskName = task.TaskName;
                existing.TaskDescription = task.TaskDescription;
                existing.TaskStatus = task.TaskStatus;
                existing.DueDate = task.DueDate;
                existing.AssignedTo = task.AssignedTo;
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
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var task = _context.WeddingChecklists.FirstOrDefault(t => t.Id == id && !t.IsDeleted);
                if (task == null)
                    return NotFound("Checklist task not found.");

                task.IsDeleted = true;
                task.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                return Ok("Task deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
