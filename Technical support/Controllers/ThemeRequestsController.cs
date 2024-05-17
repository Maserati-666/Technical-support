using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Technical_support.Data;
using Technical_support.Models;

namespace Technical_support.Controllers
{
    public class ThemeRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThemeRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ThemeRequests
        public async Task<IActionResult> Index()
        {
              return _context.ThemeRequests != null ? 
                          View(await _context.ThemeRequests.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ThemeRequests'  is null.");
        }

      
        // GET: ThemeRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ThemeRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ThemeId,NameTheme,DescriptionTheme")] ThemeRequest themeRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(themeRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(themeRequest);
        }

        // GET: ThemeRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ThemeRequests == null)
            {
                return NotFound();
            }

            var themeRequest = await _context.ThemeRequests.FindAsync(id);
            if (themeRequest == null)
            {
                return NotFound();
            }
            return View(themeRequest);
        }

        // POST: ThemeRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ThemeId,NameTheme,DescriptionTheme")] ThemeRequest themeRequest)
        {
            if (id != themeRequest.ThemeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(themeRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThemeRequestExists(themeRequest.ThemeId))
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
            return View(themeRequest);
        }

        // GET: ThemeRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ThemeRequests == null)
            {
                return NotFound();
            }

            var themeRequest = await _context.ThemeRequests
                .FirstOrDefaultAsync(m => m.ThemeId == id);
            if (themeRequest == null)
            {
                return NotFound();
            }

            return View(themeRequest);
        }

        // POST: ThemeRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ThemeRequests == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ThemeRequests'  is null.");
            }
            var themeRequest = await _context.ThemeRequests.FindAsync(id);
            var requests = _context.Request
                           .Where(m => m.ThemeId == id);
            if (requests.Count() > 0)
            {
                ModelState.AddModelError(string.Empty, "Нельзя удалить тему с которой есть заявка");
                return View(themeRequest);
            }
            else 
            {
                if (themeRequest != null)
                {
                    _context.ThemeRequests.Remove(themeRequest);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
        }

        private bool ThemeRequestExists(int id)
        {
          return (_context.ThemeRequests?.Any(e => e.ThemeId == id)).GetValueOrDefault();
        }
    }
}
