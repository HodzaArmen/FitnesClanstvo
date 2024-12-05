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
    public class PrisotnostiController : Controller
    {
        private readonly FitnesContext _context;

        public PrisotnostiController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Prisotnosti
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
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

            var prisotnosti = from s in _context.Prisotnosti
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                prisotnosti = prisotnosti.Where(p =>
                    p.DatumPrisotnosti.Year.ToString().Contains(searchString) || 
                    p.DatumPrisotnosti.Month.ToString().Contains(searchString) || 
                    p.DatumPrisotnosti.Day.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Date":
                    prisotnosti = prisotnosti.OrderBy(s => s.DatumPrisotnosti);
                    break;
                default:
                    prisotnosti = prisotnosti.OrderByDescending(s => s.DatumPrisotnosti);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Prisotnost>.CreateAsync(prisotnosti.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        /*public async Task<IActionResult> Index()
        {
            var fitnesContext = _context.Prisotnosti.Include(p => p.Clan).Include(p => p.Vadba);
            return View(await fitnesContext.ToListAsync());
        }*/

        // GET: Prisotnosti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prisotnost = await _context.Prisotnosti
                .Include(p => p.Clan)
                .Include(p => p.Vadba)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prisotnost == null)
            {
                return NotFound();
            }

            return View(prisotnost);
        }

        // GET: Prisotnosti/Create
        public IActionResult Create()
        {
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id");
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id");
            return View();
        }

        // POST: Prisotnosti/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumPrisotnosti,ClanId,VadbaId")] Prisotnost prisotnost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prisotnost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", prisotnost.ClanId);
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id", prisotnost.VadbaId);
            return View(prisotnost);
        }

        // GET: Prisotnosti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prisotnost = await _context.Prisotnosti.FindAsync(id);
            if (prisotnost == null)
            {
                return NotFound();
            }
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", prisotnost.ClanId);
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id", prisotnost.VadbaId);
            return View(prisotnost);
        }

        // POST: Prisotnosti/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumPrisotnosti,ClanId,VadbaId")] Prisotnost prisotnost)
        {
            if (id != prisotnost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prisotnost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrisotnostExists(prisotnost.Id))
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
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", prisotnost.ClanId);
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id", prisotnost.VadbaId);
            return View(prisotnost);
        }

        // GET: Prisotnosti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prisotnost = await _context.Prisotnosti
                .Include(p => p.Clan)
                .Include(p => p.Vadba)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prisotnost == null)
            {
                return NotFound();
            }

            return View(prisotnost);
        }

        // POST: Prisotnosti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prisotnost = await _context.Prisotnosti.FindAsync(id);
            if (prisotnost != null)
            {
                _context.Prisotnosti.Remove(prisotnost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrisotnostExists(int id)
        {
            return _context.Prisotnosti.Any(e => e.Id == id);
        }
    }
}
