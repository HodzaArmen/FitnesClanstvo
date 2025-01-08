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
    [Authorize(Roles = "Administrator, Manager, User")]
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

            var rezervacije = _context.Rezervacije
                .Include(r => r.Clan)
                .Include(r => r.Vadba)
                .AsQueryable();

            // If the user is not an Admin or Manager, filter reservations to show only their own
            if (!User.IsInRole("Administrator") && !User.IsInRole("Manager"))
            {
                var currentUser = User.Identity.Name;
                var clan = _context.Clani.FirstOrDefault(c => c.Email == currentUser);
                if (clan == null)
                {
                    return NotFound("User's clan not found.");
                }
                rezervacije = rezervacije.Where(r => r.ClanId == clan.Id);  // Only show reservations for the logged-in user
            }

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

            int pageSize = 5;
            return View(await PaginatedList<Rezervacija>.CreateAsync(rezervacije.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

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
            // Get the logged-in user's ID (Username) from the authenticated user.
            var currentUser = User.Identity.Name;
            var clan = _context.Clani.FirstOrDefault(c => c.Email == currentUser);

            // Check if the logged-in user is an Admin or Manager
            bool isAdminOrManager = User.IsInRole("Administrator") || User.IsInRole("Manager");

            // If the user is an Admin or Manager, allow them to select any member.
            // If the user is not an Admin or Manager (i.e., a regular user), only allow them to reserve for themselves.
            if (isAdminOrManager)
            {
                ViewBag.ClanId = new SelectList(_context.Clani.Select(c => new
                {
                    Id = c.Id,
                    FullName = c.Ime + " " + c.Priimek
                }), "Id", "FullName");
            }
            else
            {
                // Regular users can only reserve for themselves
                ViewBag.ClanId = new SelectList(new[] 
                {
                    new { Id = clan.Id, FullName = clan.Ime + " " + clan.Priimek }
                }, "Id", "FullName");
            }

            // Populate the ViewBag with available classes for all users.
            ViewBag.VadbaId = new SelectList(_context.Vadbe.Select(v => new
            {
                Id = v.Id,
                Ime = v.Ime
            }), "Id", "Ime");

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
                // Get the logged-in user's details
                var currentUser = User.Identity.Name;
                var clan = await _context.Clani.FirstOrDefaultAsync(c => c.Email == currentUser);
                // If no matching clan is found, return an error or handle it as needed
                if (clan == null)
                {
                    // You can log an error, return an error page, or redirect the user
                    return NotFound("User's clan not found.");
                }
                // Check if the logged-in user is an Admin or Manager
                bool isAdminOrManager = User.IsInRole("Administrator") || User.IsInRole("Manager");

                if (!isAdminOrManager)
                {
                    // If the user is not an Admin or Manager, they can only reserve for themselves
                    if (rezervacija.ClanId != clan.Id)
                    {
                        return Unauthorized(); // If the logged-in user tries to reserve for someone else, deny the action
                    }
                }

                // Add the reservation
                _context.Add(rezervacija);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If model is not valid, re-render the view with the same data
            ViewBag.ClanId = new SelectList(_context.Clani.Select(c => new 
            {
                Id = c.Id,
                FullName = c.Ime + " " + c.Priimek
            }), "Id", "FullName");
            ViewBag.VadbaId = new SelectList(_context.Vadbe.Select(v => new
            {
                Id = v.Id,
                Ime = v.Ime
            }), "Id", "Ime");
            return View(rezervacija);
        }

        // GET: Rezervacije/Edit/5
        [Authorize(Roles = "Administrator, Manager")]
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
            ViewBag.ClanId = new SelectList(_context.Clani.Select(c => new 
            {
                Id = c.Id,
                FullName = c.Ime + " " + c.Priimek
            }), "Id", "FullName");
            ViewBag.VadbaId = new SelectList(_context.Vadbe.Select(v => new
            {
                Id = v.Id,
                Ime = v.Ime
            }), "Id", "Ime");
            return View(rezervacija);
        }

        // POST: Rezervacije/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Manager")]
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
            ViewBag.ClanId = new SelectList(_context.Clani.Select(c => new 
            {
                Id = c.Id,
                FullName = c.Ime + " " + c.Priimek
            }), "Id", "FullName");
            ViewBag.VadbaId = new SelectList(_context.Vadbe.Select(v => new
            {
                Id = v.Id,
                Ime = v.Ime
            }), "Id", "Ime");
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
