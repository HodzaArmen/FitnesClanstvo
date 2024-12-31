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
    public class VadbeController : Controller
    {
        private readonly FitnesContext _context;

        public VadbeController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Vadbe
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["KapacitetaSortParm"] = sortOrder == "Kapaciteta" ? "kapaciteta_desc" : "Kapaciteta";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var vadbe = from s in _context.Vadbe
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                if (int.TryParse(searchString, out int searchKapaciteta))
                {
                    vadbe = vadbe.Where(s => s.Kapaciteta == searchKapaciteta);
                }
                else
                {
                    vadbe = vadbe.Where(s => s.Ime.Contains(searchString));
                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    vadbe = vadbe.OrderByDescending(s => s.Ime);
                    break;
                case "Kapaciteta":
                    vadbe = vadbe.OrderBy(s => s.Kapaciteta);
                    break;
                case "kapaciteta_desc":
                    vadbe = vadbe.OrderByDescending(s => s.Kapaciteta);
                    break;
                default:
                    vadbe = vadbe.OrderBy(s => s.Ime);
                    break;
            }
            int pageSize = 5;
            return View(await PaginatedList<Vadba>.CreateAsync(vadbe.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        /*public async Task<IActionResult> Index()
        {
            return View(await _context.Vadbe.ToListAsync());
        }*/

        // GET: Vadbe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vadba = await _context.Vadbe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vadba == null)
            {
                return NotFound();
            }

            return View(vadba);
        }

        // GET: Vadbe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vadbe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,DatumInUra,Kapaciteta")] Vadba vadba)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vadba);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vadba);
        }

        // GET: Vadbe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vadba = await _context.Vadbe.FindAsync(id);
            if (vadba == null)
            {
                return NotFound();
            }
            return View(vadba);
        }

        // POST: Vadbe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ime,DatumInUra,Kapaciteta")] Vadba vadba)
        {
            if (id != vadba.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vadba);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VadbaExists(vadba.Id))
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
            return View(vadba);
        }

        // GET: Vadbe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vadba = await _context.Vadbe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vadba == null)
            {
                return NotFound();
            }

            return View(vadba);
        }

        // POST: Vadbe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vadba = await _context.Vadbe.FindAsync(id);
            if (vadba != null)
            {
                _context.Vadbe.Remove(vadba);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PrijaviClanaNaVadbo(int clanId, int vadbaId)
        {
            // Poiščite člana
            var clan = await _context.Clani.FindAsync(clanId);
            if (clan == null)
            {
                return NotFound("Član ni bil najden.");
            }

            // Poiščite vadbo
            var vadba = await _context.Vadbe.FindAsync(vadbaId);
            if (vadba == null)
            {
                return NotFound("Vadba ni bila najdena.");
            }

            // Preverite kapaciteto
            if (vadba.Kapaciteta <= 0)
            {
                return BadRequest("Vadba je polna.");
            }

            // Preverite, če je član že prijavljen na vadbo
            var prijavaObstaja = await _context.Prisotnosti
                .AnyAsync(p => p.ClanId == clanId && p.VadbaId == vadbaId);
            if (prijavaObstaja)
            {
                return BadRequest("Član je že prijavljen na to vadbo.");
            }

            // Zmanjšajte kapaciteto
            vadba.Kapaciteta--;

            // Dodajte novo prisotnost
            var prisotnost = new Prisotnost
            {
                ClanId = clan.Id,
                VadbaId = vadba.Id,
                DatumPrisotnosti = DateTime.Now
            };

            _context.Prisotnosti.Add(prisotnost);

            // Shranite spremembe v bazo
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        private bool VadbaExists(int id)
        {
            return _context.Vadbe.Any(e => e.Id == id);
        }
    }
}
