using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingPlannerApplication.Data;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Controllers
{
    public class WeddingPlannersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public WeddingPlannersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: WeddingPlanners
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (userId != null)
            {
                // Fetch weddings the planner manages
                var coupleIds = await _context.WeddingPlanners
                    .Where(wp => wp.PlannerUserId == userId && !wp.IsDeleted)
                    .Select(wp => wp.CoupleId)
                    .ToListAsync();

                var weddings = await _context.Couples
                    .Where(c => coupleIds.Contains(c.Id) && !c.IsDeleted)
                    .Include(c => c.Checklists)
                    .Include(c => c.Bookings)
                    .ToListAsync();

                // Stats Overview: Calculate based on wedding data
                var weddingCount = weddings.Count;
                var avgBudget = weddings.Any() ? weddings.Average(c => c.Budget) : 0;

                // Upcoming Deadlines: Fetch the next tasks and vendor booking deadlines
                var nextTask = weddings.SelectMany(c => c.Checklists)
                                        .Where(c => !c.IsDeleted && c.TaskStatus != "Completed")
                                        .OrderBy(c => c.DueDate)
                                        .FirstOrDefault();

                var nextVendorBooking = weddings.SelectMany(c => c.Bookings)
                                                .Where(b => !b.IsDeleted && b.BookingDate > DateTime.Now)
                                                .OrderBy(b => b.BookingDate)
                                                .FirstOrDefault();

                // Task Overview: Calculate total tasks, completed tasks, and pending tasks
                var totalTasks = weddings.Sum(c => c.Checklists.Count);
                var completedTasks = weddings.Sum(c => c.Checklists.Count(t => t.TaskStatus == "Completed"));
                var pendingTasks = totalTasks - completedTasks;

                // Pass data to the view using ViewData
                ViewData["WeddingCount"] = weddingCount;
                ViewData["AvgBudget"] = avgBudget;
                ViewData["NextTask"] = nextTask?.TaskName;
                ViewData["NextVendorBooking"] = nextVendorBooking?.BookingDate.ToString("dd MMM");
                ViewData["CompletedTasks"] = completedTasks;
                ViewData["PendingTasks"] = pendingTasks;

                // Return the view with the Wedding Planners list data
                return View(await _context.WeddingPlanners.ToListAsync());
            }

            return View();
        }


        // GET: WeddingPlanners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weddingPlanner = await _context.WeddingPlanners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weddingPlanner == null)
            {
                return NotFound();
            }

            return View(weddingPlanner);
        }

        // GET: WeddingPlanners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeddingPlanners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlannerUserId,CoupleId,AssignedDate,CreatedAt,UpdatedAt,IsDeleted")] WeddingPlanner weddingPlanner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(weddingPlanner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weddingPlanner);
        }

        // GET: WeddingPlanners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weddingPlanner = await _context.WeddingPlanners.FindAsync(id);
            if (weddingPlanner == null)
            {
                return NotFound();
            }
            return View(weddingPlanner);
        }

        // POST: WeddingPlanners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlannerUserId,CoupleId,AssignedDate,CreatedAt,UpdatedAt,IsDeleted")] WeddingPlanner weddingPlanner)
        {
            if (id != weddingPlanner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weddingPlanner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeddingPlannerExists(weddingPlanner.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(weddingPlanner);
        }

        // GET: WeddingPlanners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weddingPlanner = await _context.WeddingPlanners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weddingPlanner == null)
            {
                return NotFound();
            }

            return View(weddingPlanner);
        }

        // POST: WeddingPlanners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weddingPlanner = await _context.WeddingPlanners.FindAsync(id);
            if (weddingPlanner != null)
            {
                _context.WeddingPlanners.Remove(weddingPlanner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeddingPlannerExists(int id)
        {
            return _context.WeddingPlanners.Any(e => e.Id == id);
        }



        public async Task<IActionResult> ManageWeddings()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var coupleIds = await _context.WeddingPlanners
                    .Where(wp => wp.PlannerUserId == userId && !wp.IsDeleted)
                    .Select(wp => wp.CoupleId)
                    .ToListAsync();

                var weddings = await _context.Couples
                    .Where(c => coupleIds.Contains(c.Id) && !c.IsDeleted)
                    .Include(c => c.Members)
                    .Include(c => c.Checklists)
                    .Include(c => c.Bookings)
                    .ToListAsync();

                return View(weddings);
            }

            return View();
        }

        //View
        public async Task<IActionResult> ViewChecklists(int coupleId)
        {
            var couple = await _context.Couples
                .Include(c => c.Checklists)
                .FirstOrDefaultAsync(c => c.Id == coupleId && !c.IsDeleted);

            if (couple == null)
            {
                return NotFound();
            }

            var checklistItems = couple.Checklists
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.DueDate)
                .ToList();

            ViewBag.CoupleId = coupleId;
            return View(checklistItems);
        }


        //Create
        [HttpGet]
        public IActionResult CreateChecklist(int coupleId)
        {
            ViewBag.CoupleId = coupleId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChecklist(WeddingChecklist model)
        {
            Console.WriteLine("***********");

            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;
                model.IsDeleted = false;

                _context.WeddingChecklists.Add(model);

                Console.WriteLine("Saving checklist...");
                await _context.SaveChangesAsync();
                Console.WriteLine("Checklist saved!");

                return RedirectToAction("ViewChecklists", new { coupleId = model.CoupleId });
            }

            // Debug why ModelState is invalid
            foreach (var state in ModelState)
            {
                var key = state.Key;
                var errors = state.Value.Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState Error - Field: {key}, Error: {error.ErrorMessage}");
                }
            }

            ViewBag.CoupleId = model.CoupleId;
            return View(model);
        }

        //Update
        public async Task<IActionResult> UpdateChecklist(int id, string taskStatus, DateTime dueDate, int coupleId)
        {
            var checklistItem = await _context.WeddingChecklists
                .FirstOrDefaultAsync(c => c.Id == id && c.CoupleId == coupleId && !c.IsDeleted);

            if (checklistItem == null)
            {
                return NotFound();
            }

            // Update the checklist item
            checklistItem.TaskStatus = taskStatus;
            checklistItem.DueDate = dueDate;

            _context.Update(checklistItem);
            await _context.SaveChangesAsync();

            // Optionally, redirect to ViewChecklists after update
            return RedirectToAction("ViewChecklists", new { coupleId });
        }



        //Delete
        public async Task<IActionResult> DeleteChecklist(int id, int coupleId)
        {
            var item = await _context.WeddingChecklists.FindAsync(id);
            if (item != null && !item.IsDeleted)
            {
                item.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewChecklists", new { coupleId = coupleId });
        }


        /*Timeline*/
        public IActionResult CreateTimeline(int coupleId)
        {
            ViewBag.CoupleId = coupleId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTimeline(WeddingTimeline model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                model.UpdatedAt = DateTime.Now;
                model.IsDeleted = false;

                _context.WeddingTimelines.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction("ViewWeddingTimeline", new { coupleId = model.CoupleId });
            }

            ViewBag.CoupleId = model.CoupleId;
            return View(model);
        }


        public async Task<IActionResult> ViewWeddingTimeline(int coupleId)
        {
            var couple = await _context.Couples
                .Include(c => c.Timelines)
                .FirstOrDefaultAsync(c => c.Id == coupleId && !c.IsDeleted);

            if (couple == null)
            {
                return NotFound();
            }

            var timelineItems = await _context.WeddingTimelines
            .Where(t => t.CoupleId == coupleId && !t.IsDeleted)
            .OrderBy(t => t.EventTime)
            .ToListAsync();

            return View(timelineItems);
        }

        //Update
        public async Task<IActionResult> UpdateTimeline(int id, DateTime eventTime, int coupleId)
        {
            var timelineItem = await _context.WeddingTimelines
                .FirstOrDefaultAsync(t => t.Id == id && t.CoupleId == coupleId && !t.IsDeleted);

            if (timelineItem == null)
            {
                return NotFound();
            }

            timelineItem.EventTime = eventTime;
            _context.Update(timelineItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewWeddingTimeline", new { coupleId });
        }

        //Delete
        public async Task<IActionResult> DeleteTimeline(int id, int coupleId)
        {
            var item = await _context.WeddingTimelines.FindAsync(id);
            if (item != null && !item.IsDeleted)
            {
                item.IsDeleted = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewWeddingTimeline", new { coupleId });
        }

        /*Vendor*/

        public IActionResult ViewWeddingVendors(int coupleId)
        {
            var bookings = _context.Bookings
                .Where(b => b.CoupleId == coupleId && !b.IsDeleted)
                .ToList();

            var vendors = _context.Vendors
                .Where(v => bookings.Select(b => b.VendorId).Contains(v.Id) && !v.IsDeleted)
                .ToList();

            // Combine vendor and booking info into a list of anonymous objects
            var vendorBookings = bookings
                .Select(b => new
                {
                    Vendor = vendors.FirstOrDefault(v => v.Id == b.VendorId),
                    Booking = b
                })
                .Where(vb => vb.Vendor != null)
                .ToList();

            ViewBag.VendorBookings = vendorBookings;

            return View();
        }

      







    }
}
