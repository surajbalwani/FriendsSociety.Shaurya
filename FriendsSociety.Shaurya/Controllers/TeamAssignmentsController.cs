using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;

namespace FriendsSociety.Shaurya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamAssignmentsController : ControllerBase
    {
        private readonly DataContext _context;

        public TeamAssignmentsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/TeamAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTeamAssignments()
        {
            var teams = await _context.TeamAssignments
                .Include(t => t.Leader)
                .Include(t => t.Ground)
                .Where(t => !t.IsDeleted)
                .Select(t => new
                {
                    t.TeamAssignmentID,
                    t.TeamName,
                    t.LeaderID,
                    LeaderName = t.Leader != null ? t.Leader.Name : "Unknown",
                    t.MemberIDs,
                    t.GroundID,
                    GroundName = t.Ground != null ? t.Ground.Name : null,
                    t.CreatedDate,
                    t.IsDeleted
                })
                .ToListAsync();

            return Ok(teams);
        }

        // GET: api/TeamAssignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTeamAssignment(int id)
        {
            var team = await _context.TeamAssignments
                .Include(t => t.Leader)
                .Include(t => t.Ground)
                .Where(t => t.TeamAssignmentID == id && !t.IsDeleted)
                .Select(t => new
                {
                    t.TeamAssignmentID,
                    t.TeamName,
                    t.LeaderID,
                    LeaderName = t.Leader != null ? t.Leader.Name : "Unknown",
                    t.MemberIDs,
                    t.GroundID,
                    GroundName = t.Ground != null ? t.Ground.Name : null,
                    t.CreatedDate,
                    t.IsDeleted
                })
                .FirstOrDefaultAsync();

            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // GET: api/TeamAssignments/5/Members
        [HttpGet("{id}/Members")]
        public async Task<ActionResult<object>> GetTeamMembers(int id)
        {
            var team = await _context.TeamAssignments
                .Include(t => t.Leader)
                .Include(t => t.Ground)
                .FirstOrDefaultAsync(t => t.TeamAssignmentID == id && !t.IsDeleted);

            if (team == null)
            {
                return NotFound();
            }

            // Get member details
            var memberIds = new List<int>();
            if (!string.IsNullOrEmpty(team.MemberIDs))
            {
                memberIds = team.MemberIDs.Split(',')
                    .Select(id => int.TryParse(id.Trim(), out int result) ? result : 0)
                    .Where(id => id > 0)
                    .ToList();
            }

            var members = await _context.Volunteers
                .Where(v => memberIds.Contains(v.VolunteerID) && !v.IsDeleted)
                .Select(v => new
                {
                    VolunteerID = v.VolunteerID,
                    Name = v.Name,
                    Contact = v.Contact
                })
                .ToListAsync();

            return Ok(new
            {
                team.TeamAssignmentID,
                team.TeamName,
                LeaderID = team.LeaderID,
                LeaderName = team.Leader?.Name ?? "Unknown",
                Members = members,
                MemberCount = members.Count,
                GroundID = team.GroundID,
                GroundName = team.Ground?.Name,
                team.CreatedDate
            });
        }

        // PUT: api/TeamAssignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeamAssignment(int id, TeamAssignment teamAssignment)
        {
            if (id != teamAssignment.TeamAssignmentID)
            {
                return BadRequest();
            }

            _context.Entry(teamAssignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamAssignmentExists(id))
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

        // PUT: api/TeamAssignments/5/AssignGround
        [HttpPut("{id}/AssignGround")]
        public async Task<IActionResult> AssignGround(int id, [FromBody] GroundAssignmentRequest request)
        {
            var team = await _context.TeamAssignments.FindAsync(id);
            
            if (team == null || team.IsDeleted)
            {
                return NotFound();
            }

            team.GroundID = request.GroundID;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamAssignmentExists(id))
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

        // POST: api/TeamAssignments
        [HttpPost]
        public async Task<ActionResult<TeamAssignment>> PostTeamAssignment(TeamAssignment teamAssignment)
        {
            teamAssignment.CreatedDate = DateTime.Now;
            teamAssignment.IsDeleted = false;

            _context.TeamAssignments.Add(teamAssignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeamAssignment", new { id = teamAssignment.TeamAssignmentID }, teamAssignment);
        }

        // DELETE: api/TeamAssignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamAssignment(int id)
        {
            var teamAssignment = await _context.TeamAssignments.FindAsync(id);
            if (teamAssignment == null)
            {
                return NotFound();
            }

            // Soft delete
            teamAssignment.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeamAssignmentExists(int id)
        {
            return _context.TeamAssignments.Any(e => e.TeamAssignmentID == id);
        }
    }

    public class GroundAssignmentRequest
    {
        public int? GroundID { get; set; }
    }
}
