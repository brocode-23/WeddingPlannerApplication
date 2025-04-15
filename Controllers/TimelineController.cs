using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    public class TimelineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimelineController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTimeline(int coupleId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<WeddingTimeline> query = _context.WeddingTimelines
                    .Where(t => t.CoupleId == coupleId && !t.IsDeleted)
                    .OrderBy(t => t.EventTime);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, Timeline = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddTimelineEvent([FromBody] WeddingTimeline eventModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid timeline event.");

            try
            {
                eventModel.CreatedAt = DateTime.UtcNow;
                eventModel.UpdatedAt = DateTime.UtcNow;
                eventModel.IsDeleted = false;

                _context.WeddingTimelines.Add(eventModel);
                _context.SaveChanges();

                return Ok(eventModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateTimelineEvent([FromBody] WeddingTimeline eventModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid update data.");

            try
            {
                var existing = _context.WeddingTimelines.FirstOrDefault(t => t.Id == eventModel.Id && !t.IsDeleted);
                if (existing == null)
                    return NotFound("Timeline event not found.");

                existing.EventName = eventModel.EventName;
                existing.EventTime = eventModel.EventTime;
                existing.EventDescription = eventModel.EventDescription;
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
        public IActionResult DeleteTimelineEvent(int id)
        {
            try
            {
                var eventItem = _context.WeddingTimelines.FirstOrDefault(t => t.Id == id && !t.IsDeleted);
                if (eventItem == null)
                    return NotFound("Timeline event not found.");

                eventItem.IsDeleted = true;
                eventItem.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Timeline event deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
