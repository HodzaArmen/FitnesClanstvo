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
    [Route("api/v1/vadba")]
    [ApiController]
    [ApiKeyAuth]
    public class VadbaApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VadbaApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/VadbaApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vadba>>> GetVadba()
        {
            return await _context.Vadba.ToListAsync();
        }

        // GET: api/VadbaApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vadba>> GetVadba(int id)
        {
            var vadba = await _context.Vadba.FindAsync(id);

            if (vadba == null)
            {
                return NotFound();
            }

            return vadba;
        }

        // PUT: api/VadbaApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVadba(int id, Vadba vadba)
        {
            if (id != vadba.Id)
            {
                return BadRequest();
            }

            _context.Entry(vadba).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VadbaExists(id))
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

        // POST: api/VadbaApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vadba>> PostVadba(Vadba vadba)
        {
            _context.Vadba.Add(vadba);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVadba", new { id = vadba.Id }, vadba);
        }

        // DELETE: api/VadbaApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVadba(int id)
        {
            var vadba = await _context.Vadba.FindAsync(id);
            if (vadba == null)
            {
                return NotFound();
            }

            _context.Vadba.Remove(vadba);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VadbaExists(int id)
        {
            return _context.Vadba.Any(e => e.Id == id);
        }
    }
}
