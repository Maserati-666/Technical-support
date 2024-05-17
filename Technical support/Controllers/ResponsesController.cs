using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Technical_support.Data;
using Technical_support.Models;
using Response = Technical_support.Models.Response;

namespace Technical_support.Controllers
{
    public class ResponsesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponsesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Manager,Admin")]
        // GET: Responses/Create
        public IActionResult Create(int id)
        {
            ViewData["RequestId"] = id;
            var request = _context.Request.Find(id);
            ViewData["TextRequest"] = request.TextRequest;
            return View();
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResponseId,RequestId,TextResponse")] Response response, int id)
        {
            response.RequestId = id;
            ModelState.Remove("Request");
            ModelState.Remove("RequestId");
            if (ModelState.IsValid)
            {
                _context.Add(response);
                await _context.SaveChangesAsync();
                var request = _context.Request.Find(id);
                return RedirectToAction("RequestsInProgress", new RouteValueDictionary(new
                {
                    Controller = "Manager",
                    Action = "RequestsInProgress",
                    id = request.ManagerId
                }));
            }
            ViewData["RequestId"] = new SelectList(_context.Request, "RequestId", "TextRequest", response.RequestId);
            return View(response);
        }

        [Authorize(Roles = "Admin")]
        // GET: Responses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Responses == null)
            {
                return NotFound();
            }
            var response = await _context.Responses
                 .Include(r => r.Request)
                 .FirstOrDefaultAsync(m => m.ResponseId == id);
            //var response = await _context.Responses.FindAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            return View(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResponseId,RequestId,TextResponse")] Response response)
        {
            if (id != response.ResponseId)
            {
                return NotFound();
            }
            ModelState.Remove("Request");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(response);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResponseExists(response.ResponseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var request = _context.Request.Find(id);
                return RedirectToAction("RequestsInProgress", new RouteValueDictionary(new
                {
                    Controller = "Manager",
                    Action = "RequestsInProgress",
                    id = request.ManagerId
                }));
            }
            return View(response);
        }

        [Authorize(Roles = "Admin")]
        // GET: Responses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Responses == null)
            {
                return NotFound();
            }

            var response = await _context.Responses
                .Include(r => r.Request)
                .FirstOrDefaultAsync(m => m.ResponseId == id);
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Responses == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Responses'  is null.");
            }
            var response = await _context.Responses.FindAsync(id);
            if (response != null)
            {
                _context.Responses.Remove(response);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResponseExists(int id)
        {
          return (_context.Responses?.Any(e => e.ResponseId == id)).GetValueOrDefault();
        }
    }
}
