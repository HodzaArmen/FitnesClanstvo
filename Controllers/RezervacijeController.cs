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
    public class RezervacijeController : Controller
    {
        private readonly FitnesContext _context;

        public RezervacijeController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Rezervacije
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

            var rezervacije = from s in _context.Rezervacije
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                rezervacije = rezervacije.Where(p =>
                    p.DatumRezervacije.Year.ToString().Contains(searchString) || 
                    p.DatumRezervacije.Month.ToString().Contains(searchString) || 
                    p.DatumRezervacije.Day.ToString().Contains(searchString));
            }
            switch (sortOrder)
            {
                case "Date":
                    rezervacije = rezervacije.OrderBy(s => s.DatumRezervacije);
                    break;
                default:
                    rezervacije = rezervacije.OrderByDescending(s => s.DatumRezervacije);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Rezervacija>.CreateAsync(rezervacije.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        /*public async Task<IActionResult> Index()
        {
            var fitnesContext = _context.Rezervacije.Include(r => r.Clan).Include(r => r.Vadba);
            return View(await fitnesContext.ToListAsync());
        }*/

        // GET: Rezervacije/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacije
                .Include(r => r.Clan)
                .Include(r => r.Vadba)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rezervacija == null)
            {
                return NotFound();
            }

            return View(rezervacija);
        }

        // GET: Rezervacije/Create
        public IActionResult Create()
        {
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id");
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id");
            return View();
        }

        // POST: Rezervacije/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumRezervacije,ClanId,VadbaId")] Rezervacija rezervacija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rezervacija);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", rezervacija.ClanId);
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id", rezervacija.VadbaId);
            return View(rezervacija);
        }

        // GET: Rezervacije/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacije.FindAsync(id);
            if (rezervacija == null)
            {
                return NotFound();
            }
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", rezervacija.ClanId);
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id", rezervacija.VadbaId);
            return View(rezervacija);
        }

        // POST: Rezervacije/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumRezervacije,ClanId,VadbaId")] Rezervacija rezervacija)
        {
            if (id != rezervacija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rezervacija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RezervacijaExists(rezervacija.Id))
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
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", rezervacija.ClanId);
            ViewData["VadbaId"] = new SelectList(_context.Vadbe, "Id", "Id", rezervacija.VadbaId);
            return View(rezervacija);
        }

        // GET: Rezervacije/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacije
                .Include(r => r.Clan)
                .Include(r => r.Vadba)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rezervacija == null)
            {
                return NotFound();
            }

            return View(rezervacija);
        }

        // POST: Rezervacije/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervacija = await _context.Rezervacije.FindAsync(id);
            if (rezervacija != null)
            {
                _context.Rezervacije.Remove(rezervacija);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RezervacijaExists(int id)
        {
            return _context.Rezervacije.Any(e => e.Id == id);
        }
    }
}
