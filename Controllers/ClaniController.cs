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
    public class ClaniController : Controller
    {
        private readonly FitnesContext _context;

        public ClaniController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Clani
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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

            var clani = from s in _context.Clani
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                clani = clani.Where(s => s.Priimek.Contains(searchString)
                                    || s.Ime.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    clani = clani.OrderByDescending(s => s.Priimek);
                    break;
                case "Date":
                    clani = clani.OrderBy(s => s.DatumRojstva);
                    break;
                case "date_desc":
                    clani = clani.OrderByDescending(s => s.DatumRojstva);
                    break;
                default:
                    clani = clani.OrderBy(s => s.Priimek);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Clan>.CreateAsync(clani.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        /*public async Task<IActionResult> Index()
        {
            return View(await _context.Clani.ToListAsync());
        }*/
        [Authorize]
        // GET: Clani/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var clan = await _context.Clani
                .Include(c => c.Clanstvo) // Vključi članstvo
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            //var clan = await _context.Clani
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (clan == null)
            {
                return NotFound();
            }

            return View(clan);
        }

        // GET: Clani/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clani/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ime,Priimek,DatumRojstva,Email")] Clan clan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(clan);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            
            return View(clan);
        }

        // GET: Clani/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clan = await _context.Clani.FindAsync(id);
            if (clan == null)
            {
                return NotFound();
            }
            return View(clan);
        }

        // POST: Clani/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,Priimek,DatumRojstva,Email")] Clan clan)
        {
            if (id != clan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClanExists(clan.Id))
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
            return View(clan);
        }

        // GET: Clani/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clan = await _context.Clani
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clan == null)
            {
                return NotFound();
            }

            return View(clan);
        }

        // POST: Clani/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clan = await _context.Clani.FindAsync(id);
            if (clan != null)
            {
                _context.Clani.Remove(clan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClanExists(int id)
        {
            return _context.Clani.Any(e => e.Id == id);
        }
    }
}
