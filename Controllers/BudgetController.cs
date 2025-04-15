using Microsoft.AspNetCore.Mvc;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BudgetController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBudgetItems(int coupleId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                IQueryable<WeddingBudget> query = _context.WeddingBudgets
                    .Where(b => b.CoupleId == coupleId && !b.IsDeleted)
                    .OrderBy(b => b.Category);

                var totalCount = query.Count();

                if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
                {
                    query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
                }

                return Ok(new { TotalCount = totalCount, BudgetItems = query.ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddBudgetItem([FromBody] WeddingBudget item)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid budget data.");

            try
            {
                item.CreatedAt = DateTime.UtcNow;
                item.UpdatedAt = DateTime.UtcNow;
                item.IsDeleted = false;

                _context.WeddingBudgets.Add(item);
                _context.SaveChanges();

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateBudgetItem([FromBody] WeddingBudget item)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid update data.");

            try
            {
                var existing = _context.WeddingBudgets.FirstOrDefault(b => b.Id == item.Id && !b.IsDeleted);
                if (existing == null)
                    return NotFound("Budget item not found.");

                existing.Category = item.Category;
                existing.AllocatedAmount = item.AllocatedAmount;
                existing.SpentAmount = item.SpentAmount;
                existing.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();

                if (existing.SpentAmount > existing.AllocatedAmount)
                    return Ok(new { Warning = "Spent amount exceeds allocated budget!", Item = existing });

                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult DeleteBudgetItem(int id)
        {
            try
            {
                var item = _context.WeddingBudgets.FirstOrDefault(b => b.Id == id && !b.IsDeleted);
                if (item == null)
                    return NotFound("Budget item not found.");

                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;

                _context.SaveChanges();
                return Ok("Budget item deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
