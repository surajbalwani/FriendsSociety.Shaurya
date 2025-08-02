using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;

namespace FriendsSociety.Shaurya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly DataContext _context;

        public TournamentsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Tournaments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tournament>>> GetTournaments()
        {
            return await _context.Tournaments
                .Where(t => !t.IsDeleted)
                .Include(t => t.Activities.Where(a => !a.IsDeleted))
                .ToListAsync();
        }

        // GET: api/Tournaments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournament(int id)
        {
            var tournament = await _context.Tournaments
                .Where(t => !t.IsDeleted)
                .Include(t => t.Activities.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.GroundAllocations)
                        .ThenInclude(ga => ga.Ground)
                .FirstOrDefaultAsync(t => t.TournamentID == id);

            if (tournament == null)
            {
                return NotFound();
            }

            return tournament;
        }

        // PUT: api/Tournaments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournament(int id, Tournament tournament)
        {
            if (id != tournament.TournamentID)
            {
                return BadRequest();
            }

            // Validate tournament dates
            if (tournament.StartDate >= tournament.EndDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            tournament.UpdatedAt = DateTime.UtcNow;
            _context.Entry(tournament).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(id))
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

        // POST: api/Tournaments
        [HttpPost]
        public async Task<ActionResult<Tournament>> PostTournament(Tournament tournament)
        {
            // Validate tournament dates
            if (tournament.StartDate >= tournament.EndDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            tournament.CreatedAt = DateTime.UtcNow;
            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTournament", new { id = tournament.TournamentID }, tournament);
        }

        // DELETE: api/Tournaments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            var tournament = await _context.Tournaments.FindAsync(id);
            if (tournament == null)
            {
                return NotFound();
            }

            // Soft delete
            tournament.IsDeleted = true;
            tournament.IsActive = false;
            tournament.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Tournaments/5/activities/3
        [HttpPost("{tournamentId}/activities/{activityId}")]
        public async Task<IActionResult> AddActivityToTournament(int tournamentId, int activityId)
        {
            var tournament = await _context.Tournaments.FindAsync(tournamentId);
            var activity = await _context.Activities.FindAsync(activityId);

            if (tournament == null || activity == null)
            {
                return NotFound();
            }

            if (tournament.IsDeleted || activity.IsDeleted)
            {
                return BadRequest("Cannot add deleted items.");
            }

            activity.TournamentID = tournamentId;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Tournaments/5/activities/3
        [HttpDelete("{tournamentId}/activities/{activityId}")]
        public async Task<IActionResult> RemoveActivityFromTournament(int tournamentId, int activityId)
        {
            var activity = await _context.Activities.FindAsync(activityId);

            if (activity == null || activity.TournamentID != tournamentId)
            {
                return NotFound();
            }

            activity.TournamentID = null;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: api/Tournaments/5/activities
        [HttpGet("{id}/activities")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetTournamentActivities(int id)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Activities.Where(a => !a.IsDeleted))
                    .ThenInclude(a => a.GroundAllocations)
                        .ThenInclude(ga => ga.Ground)
                .FirstOrDefaultAsync(t => t.TournamentID == id && !t.IsDeleted);

            if (tournament == null)
            {
                return NotFound();
            }

            return Ok(tournament.Activities);
        }

        private bool TournamentExists(int id)
        {
            return _context.Tournaments.Any(e => e.TournamentID == id && !e.IsDeleted);
        }
    }
}