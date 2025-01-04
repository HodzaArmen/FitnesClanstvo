using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.Data;
using FitnesClanstvo.Models;

namespace FitnesClanstvo.Controllers_Api
{
    [Route("api/v1/prisotnost")]
    [ApiController]
    public class PrisotnostApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrisotnostApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PrisotnostApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prisotnost>>> GetPrisotnost()
        {
            return await _context.Prisotnost.ToListAsync();
        }

        // GET: api/PrisotnostApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prisotnost>> GetPrisotnost(int id)
        {
            var prisotnost = await _context.Prisotnost.FindAsync(id);

            if (prisotnost == null)
            {
                return NotFound();
            }

            return prisotnost;
        }

        // PUT: api/PrisotnostApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrisotnost(int id, Prisotnost prisotnost)
        {
            if (id != prisotnost.Id)
            {
                return BadRequest();
            }

            _context.Entry(prisotnost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrisotnostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PrisotnostApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Prisotnost>> PostPrisotnost(Prisotnost prisotnost)
        {
            _context.Prisotnost.Add(prisotnost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrisotnost", new { id = prisotnost.Id }, prisotnost);
        }

        // DELETE: api/PrisotnostApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrisotnost(int id)
        {
            var prisotnost = await _context.Prisotnost.FindAsync(id);
            if (prisotnost == null)
            {
                return NotFound();
            }

            _context.Prisotnost.Remove(prisotnost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrisotnostExists(int id)
        {
            return _context.Prisotnost.Any(e => e.Id == id);
        }
    }
}
