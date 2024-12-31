using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.Data;
using FitnesClanstvo.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnesClanstvo.Controllers
{
    [Authorize(Roles = "Administrator, Manager")]
    public class ClanstvaController : Controller
    {
        private readonly FitnesContext _context;

        public ClanstvaController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Clanstva
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TipSortParm"] = String.IsNullOrEmpty(sortOrder) ? "tip_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var clanstva = _context.Clanstva
                .Include(c => c.Clan)
                .AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                clanstva = clanstva.Where(s => s.Tip.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "tip_desc":
                    clanstva = clanstva.OrderByDescending(s => s.Tip);
                    break;
                case "Date":
                    clanstva = clanstva.OrderBy(s => s.Zacetek);
                    break;
                case "date_desc":
                    clanstva = clanstva.OrderByDescending(s => s.Zacetek);
                    break;
                default:
                    clanstva = clanstva.OrderBy(s => s.Tip);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Clanstvo>.CreateAsync(clanstva.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Clanstva/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clanstvo = await _context.Clanstva
                .Include(c => c.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clanstvo == null)
            {
                return NotFound();
            }

            return View(clanstvo);
        }

        // GET: Clanstva/Create
        public IActionResult Create()
        {
            ViewBag.ClanId = new SelectList(_context.Clani.Select(c => new 
            {
                Id = c.Id,
                FullName = c.Ime + " " + c.Priimek
            }), "Id", "FullName");
            return View();
        }

        // POST: Clanstva/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Clanstvo clanstvo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clanstvo);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            // Ponovno napolni ViewBag v primeru napake
            ViewBag.ClanId = new SelectList(_context.Clani.Select(c => new 
            {
                Id = c.Id,
                FullName = c.Ime + " " + c.Priimek
            }), "Id", "FullName");

            return View(clanstvo);
        }

        // GET: Clanstva/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clanstvo = await _context.Clanstva.FindAsync(id);
            if (clanstvo == null)
            {
                return NotFound();
            }
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", clanstvo.ClanId);
            return View(clanstvo);
        }

        // POST: Clanstva/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tip,Zacetek,Konec,ClanId")] Clanstvo clanstvo)
        {
            if (id != clanstvo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clanstvo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClanstvoExists(clanstvo.Id))
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
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", clanstvo.ClanId);
            return View(clanstvo);
        }

        // GET: Clanstva/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clanstvo = await _context.Clanstva
                .Include(c => c.Clan)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clanstvo == null)
            {
                return NotFound();
            }

            return View(clanstvo);
        }

        // POST: Clanstva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clanstvo = await _context.Clanstva.FindAsync(id);
            if (clanstvo != null)
            {
                _context.Clanstva.Remove(clanstvo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClanstvoExists(int id)
        {
            return _context.Clanstva.Any(e => e.Id == id);
        }
    }
}
