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
    [Route("api/v1/clan")]
    [ApiController]
    public class ClanApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClanApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ClanApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clan>>> GetClan()
        {
            return await _context.Clan.ToListAsync();
        }

        // GET: api/ClanApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clan>> GetClan(int id)
        {
            var clan = await _context.Clan.FindAsync(id);

            if (clan == null)
            {
                return NotFound();
            }

            return clan;
        }

        // PUT: api/ClanApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClan(int id, Clan clan)
        {
            if (id != clan.Id)
            {
                return BadRequest();
            }

            _context.Entry(clan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClanExists(id))
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

        // POST: api/ClanApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Clan>> PostClan(Clan clan)
        {
            _context.Clan.Add(clan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClan", new { id = clan.Id }, clan);
        }

        // DELETE: api/ClanApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClan(int id)
        {
            var clan = await _context.Clan.FindAsync(id);
            if (clan == null)
            {
                return NotFound();
            }

            _context.Clan.Remove(clan);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClanExists(int id)
        {
            return _context.Clan.Any(e => e.Id == id);
        }
    }
}
