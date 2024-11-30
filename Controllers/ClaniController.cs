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
    public class ClaniController : Controller
    {
        private readonly FitnesContext _context;

        public ClaniController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Clani
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clani.ToListAsync());
        }

        // GET: Clani/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Clani/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clani/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Ime,Priimek,DatumRojstva,Email")] Clan clan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
