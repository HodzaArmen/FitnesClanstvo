using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnesClanstvo.Data;
using FitnesClanstvo.Models;
using FitnesClanstvo.Filters;

namespace FitnesClanstvo.Controllers_Api
{
    [Route("api/v1/clanstvo")]
    [ApiController]
    [ApiKeyAuth]
    public class ClanstvoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClanstvoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ClanstvoApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clanstvo>>> GetClanstvo()
        {
            return await _context.Clanstvo.ToListAsync();
        }

        // GET: api/ClanstvoApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clanstvo>> GetClanstvo(int id)
        {
            var clanstvo = await _context.Clanstvo.FindAsync(id);

            if (clanstvo == null)
            {
                return NotFound();
            }

            return clanstvo;
        }

        // PUT: api/ClanstvoApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClanstvo(int id, Clanstvo clanstvo)
        {
            if (id != clanstvo.Id)
            {
                return BadRequest();
            }

            _context.Entry(clanstvo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClanstvoExists(id))
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

        // POST: api/ClanstvoApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Clanstvo>> PostClanstvo(Clanstvo clanstvo)
        {
            _context.Clanstvo.Add(clanstvo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClanstvo", new { id = clanstvo.Id }, clanstvo);
        }

        // DELETE: api/ClanstvoApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClanstvo(int id)
        {
            var clanstvo = await _context.Clanstvo.FindAsync(id);
            if (clanstvo == null)
            {
                return NotFound();
            }

            _context.Clanstvo.Remove(clanstvo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClanstvoExists(int id)
        {
            return _context.Clanstvo.Any(e => e.Id == id);
        }
    }
}
