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
using Microsoft.AspNetCore.Identity;

namespace FitnesClanstvo.Controllers
{
    [Authorize(Roles = "Administrator, Manager")]
    public class PlacilaController : Controller
    {
        private readonly FitnesContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;

        public PlacilaController(FitnesContext context, UserManager<ApplicationUser> usermanager)
        {
            _context = context;
            _usermanager = usermanager;
        }

        // GET: Placila
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ZnesekSortParm"] = String.IsNullOrEmpty(sortOrder) ? "znesek_desc" : "";
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

            var placila = from s in _context.Placila
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                placila = placila.Where(p =>
                    p.DatumPlacila.Year.ToString().Contains(searchString) || 
                    p.DatumPlacila.Month.ToString().Contains(searchString) || 
                    p.DatumPlacila.Day.ToString().Contains(searchString) ||   
                    p.Znesek.ToString().Contains(searchString));              
            }

            switch (sortOrder)
            {
                case "znesek_desc":
                    placila = placila.OrderByDescending(s => s.Znesek);
                    break;
                case "Date":
                    placila = placila.OrderBy(s => s.DatumPlacila);
                    break;
                case "date_desc":
                    placila = placila.OrderByDescending(s => s.DatumPlacila);
                    break;
                default:
                    placila = placila.OrderBy(s => s.Znesek);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Placilo>.CreateAsync(placila.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        /*public async Task<IActionResult> Index()
        {
            var fitnesContext = _context.Placila.Include(p => p.Clanstvo);
            return View(await fitnesContext.ToListAsync());
        }*/

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
            var currentUser = await _usermanager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                placilo.DateCreated = DateTime.Now;
                placilo.DateEdited = DateTime.Now;
                placilo.Owner = currentUser;

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
