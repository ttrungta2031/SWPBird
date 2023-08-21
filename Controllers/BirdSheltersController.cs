using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWPBirdBoarding.Models;

namespace SWPBirdBoarding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirdSheltersController : ControllerBase
    {
        private readonly SWPBirdBoardingContext _context;

        public BirdSheltersController(SWPBirdBoardingContext context)
        {
            _context = context;
        }

        // GET: api/BirdShelters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BirdShelter>>> GetBirdShelters()
        {
            return await _context.BirdShelters.ToListAsync();
        }

        // GET: api/BirdShelters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BirdShelter>> GetBirdShelter(int id)
        {
            var birdShelter = await _context.BirdShelters.FindAsync(id);

            if (birdShelter == null)
            {
                return NotFound();
            }

            return birdShelter;
        }

        // PUT: api/BirdShelters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBirdShelter(int id, BirdShelter birdShelter)
        {
            if (id != birdShelter.Id)
            {
                return BadRequest();
            }

            _context.Entry(birdShelter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BirdShelterExists(id))
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

        // POST: api/BirdShelters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BirdShelter>> PostBirdShelter(BirdShelter birdShelter)
        {
            _context.BirdShelters.Add(birdShelter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBirdShelter", new { id = birdShelter.Id }, birdShelter);
        }

        // DELETE: api/BirdShelters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBirdShelter(int id)
        {
            var birdShelter = await _context.BirdShelters.FindAsync(id);
            if (birdShelter == null)
            {
                return NotFound();
            }

            _context.BirdShelters.Remove(birdShelter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BirdShelterExists(int id)
        {
            return _context.BirdShelters.Any(e => e.Id == id);
        }
    }
}
