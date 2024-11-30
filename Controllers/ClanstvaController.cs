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
    public class ClanstvaController : Controller
    {
        private readonly FitnesContext _context;

        public ClanstvaController(FitnesContext context)
        {
            _context = context;
        }

        // GET: Clanstva
        public async Task<IActionResult> Index()
        {
            var fitnesContext = _context.Clanstva.Include(c => c.Clan);
            return View(await fitnesContext.ToListAsync());
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
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id");
            return View();
        }

        // POST: Clanstva/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tip,Zacetek,Konec,ClanId")] Clanstvo clanstvo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clanstvo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClanId"] = new SelectList(_context.Clani, "Id", "Id", clanstvo.ClanId);
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