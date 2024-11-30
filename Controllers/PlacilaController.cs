using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.Data;
using FitnesClanstvo.Models;

namespace FitnesClanstvo.Controllers
{
    public class PlacilaController : Controller
    {
        private readonly FitnesContext _context;

        public PlacilaController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Placila
        public async Task<IActionResult> Index()
        {
            var fitnesContext = _context.Placila.Include(p => p.Clanstvo);
            return View(await fitnesContext.ToListAsync());
        }

        // GET: Placila/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placilo = await _context.Placila
                .Include(p => p.Clanstvo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (placilo == null)
            {
                return NotFound();
            }

            return View(placilo);
        }

        // GET: Placila/Create
        public IActionResult Create()
        {
            ViewData["ClanstvoId"] = new SelectList(_context.Clanstva, "Id", "Id");
            return View();
        }

        // POST: Placila/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumPlacila,Znesek,ClanstvoId")] Placilo placilo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(placilo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanstvoId"] = new SelectList(_context.Clanstva, "Id", "Id", placilo.ClanstvoId);
            return View(placilo);
        }

        // GET: Placila/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placilo = await _context.Placila.FindAsync(id);
            if (placilo == null)
            {
                return NotFound();
            }
            ViewData["ClanstvoId"] = new SelectList(_context.Clanstva, "Id", "Id", placilo.ClanstvoId);
            return View(placilo);
        }

        // POST: Placila/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumPlacila,Znesek,ClanstvoId")] Placilo placilo)
        {
            if (id != placilo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(placilo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaciloExists(placilo.Id))
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
            ViewData["ClanstvoId"] = new SelectList(_context.Clanstva, "Id", "Id", placilo.ClanstvoId);
            return View(placilo);
        }

        // GET: Placila/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var placilo = await _context.Placila
                .Include(p => p.Clanstvo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (placilo == null)
            {
                return NotFound();
            }

            return View(placilo);
        }

        // POST: Placila/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var placilo = await _context.Placila.FindAsync(id);
            if (placilo != null)
            {
                _context.Placila.Remove(placilo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaciloExists(int id)
        {
            return _context.Placila.Any(e => e.Id == id);
        }
    }
}
