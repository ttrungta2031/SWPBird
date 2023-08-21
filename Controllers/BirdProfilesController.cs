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
    public class BirdProfilesController : ControllerBase
    {
        private readonly SWPBirdBoardingContext _context;

        public BirdProfilesController(SWPBirdBoardingContext context)
        {
            _context = context;
        }

        // GET: api/BirdProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BirdProfile>>> GetBirdProfiles()
        {
            return await _context.BirdProfiles.ToListAsync();
        }

        // GET: api/BirdProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BirdProfile>> GetBirdProfile(int id)
        {
            var birdProfile = await _context.BirdProfiles.FindAsync(id);

            if (birdProfile == null)
            {
                return NotFound();
            }

            return birdProfile;
        }

        // PUT: api/BirdProfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBirdProfile(int id, BirdProfile birdProfile)
        {
            if (id != birdProfile.Id)
            {
                return BadRequest();
            }

            _context.Entry(birdProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BirdProfileExists(id))
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

        // POST: api/BirdProfiles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
      /*  [HttpPost]
        public async Task<ActionResult<BirdProfile>> PostBirdProfile(BirdProfile birdProfile)
        {
            _context.BirdProfiles.Add(birdProfile);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBirdProfile", new { id = birdProfile.Id }, birdProfile);
        }
*/
        // DELETE: api/BirdProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var birdProfile = await _context.Accounts.FindAsync(id);
            if (birdProfile == null)
            {
                return StatusCode(200, new { StatusCode = 200, message = "Xác nhận thanh cong" });
            }

            _context.Accounts.Remove(birdProfile);
            await _context.SaveChangesAsync();

            return StatusCode(409, new { StatusCode = 409, message = "Xác nhận thất bại" });
        }

        private bool BirdProfileExists(int id)
        {
            return _context.BirdProfiles.Any(e => e.Id == id);
        }
    }
}
