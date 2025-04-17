using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingPlannerApplication.Models;
using WeddingPlannerApplication.Data;

namespace WeddingPlannerApplication.Controllers
{
    public class VendorCatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendorCatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VendorCats
        public async Task<IActionResult> Index()
        {
            return View(await _context.VendorCategories.ToListAsync());
        }

        // GET: VendorCats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendorCat = await _context.VendorCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendorCat == null)
            {
                return NotFound();
            }

            return View(vendorCat);
        }

        // GET: VendorCats/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: VendorCats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] VendorCat vendorCat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vendorCat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vendorCat);
        }

        // GET: VendorCats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendorCat = await _context.VendorCategories.FindAsync(id);
            if (vendorCat == null)
            {
                return NotFound();
            }
            return View(vendorCat);
        }

        // POST: VendorCats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] VendorCat vendorCat)
        {
            if (id != vendorCat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendorCat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendorCatExists(vendorCat.Id))
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
            return View(vendorCat);
        }

        // GET: VendorCats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendorCat = await _context.VendorCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendorCat == null)
            {
                return NotFound();
            }

            return View(vendorCat);
        }

        // POST: VendorCats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendorCat = await _context.VendorCategories.FindAsync(id);
            if (vendorCat != null)
            {
                _context.VendorCategories.Remove(vendorCat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendorCatExists(int id)
        {
            return _context.VendorCategories.Any(e => e.Id == id);
        }
    }
}
