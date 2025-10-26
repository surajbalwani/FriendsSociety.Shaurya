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
    public class VolunteersController : ControllerBase
    {
        private readonly DataContext _context;

        public VolunteersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Volunteers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVolunteers()
        {
            var volunteers = await _context.Volunteers
                .Where(v => !v.IsDeleted)
                .Select(v => new
                {
                    v.VolunteerID,
                    v.Name,
                    v.Contact,
                    v.WhatsAppNo,
                    v.Email,
                    v.Address,
                    v.IsDeleted,
                    v.CreatedDate,
                    v.UpdatedDate
                })
                .ToListAsync();

            return Ok(volunteers);
        }

        // GET: api/Volunteers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetVolunteer(int id)
        {
            var volunteer = await _context.Volunteers
                .Where(v => v.VolunteerID == id && !v.IsDeleted)
                .Select(v => new
                {
                    v.VolunteerID,
                    v.Name,
                    v.Contact,
                    v.WhatsAppNo,
                    v.Email,
                    v.Address,
                    v.IsDeleted,
                    v.CreatedDate,
                    v.UpdatedDate
                })
                .FirstOrDefaultAsync();

            if (volunteer == null)
            {
                return NotFound();
            }

            return Ok(volunteer);
        }

        // PUT: api/Volunteers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVolunteer(int id, VolunteerUpdateDto volunteerDto)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            
            if (volunteer == null)
            {
                return NotFound();
            }

            volunteer.Name = volunteerDto.Name;
            volunteer.Contact = volunteerDto.Contact;
            volunteer.WhatsAppNo = volunteerDto.WhatsAppNo;
            volunteer.Email = volunteerDto.Email;
            volunteer.Address = volunteerDto.Address;
            volunteer.UpdatedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VolunteerExists(id))
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

        // POST: api/Volunteers
        [HttpPost]
        public async Task<ActionResult<Volunteer>> PostVolunteer(VolunteerCreateDto volunteerDto)
        {
            var volunteer = new Volunteer
            {
                Name = volunteerDto.Name,
                Contact = volunteerDto.Contact,
                WhatsAppNo = volunteerDto.WhatsAppNo,
                Email = volunteerDto.Email,
                Address = volunteerDto.Address,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };

            _context.Volunteers.Add(volunteer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVolunteer", new { id = volunteer.VolunteerID }, volunteer);
        }

        // DELETE: api/Volunteers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVolunteer(int id)
        {
            var volunteer = await _context.Volunteers.FindAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            // Soft delete
            volunteer.IsDeleted = true;
            volunteer.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VolunteerExists(int id)
        {
            return _context.Volunteers.Any(e => e.VolunteerID == id && !e.IsDeleted);
        }
    }

    // DTOs for Volunteer operations
    public class VolunteerCreateDto
    {
        public required string Name { get; set; }
        public string? Contact { get; set; }
        public string? WhatsAppNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class VolunteerUpdateDto
    {
        public required string Name { get; set; }
        public string? Contact { get; set; }
        public string? WhatsAppNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
