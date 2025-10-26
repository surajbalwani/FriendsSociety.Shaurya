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
    public class ParticipantsController : ControllerBase
    {
        private readonly DataContext _context;

        public ParticipantsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetParticipants()
        {
            var participants = await _context.Participants
                .Include(p => p.Organization)
                .Include(p => p.AbilityType)
                .Where(p => !p.IsDeleted)
                .Select(p => new
                {
                    p.ParticipantID,
                    p.Name,
                    p.Age,
                    p.Gender,
                    p.BloodGroup,
                    p.OrganizationID,
                    OrganizationName = p.Organization != null ? p.Organization.Name : "Unknown",
                    p.AbilityTypeID,
                    AbilityTypeName = p.AbilityType != null ? p.AbilityType.Name : "Unknown",
                    p.Contact,
                    p.EmergencyContact,
                    p.Address,
                    p.MedicalNotes,
                    p.IsDeleted,
                    p.CreatedDate,
                    p.UpdatedDate
                })
                .ToListAsync();

            return Ok(participants);
        }

        // GET: api/Participants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetParticipant(int id)
        {
            var participant = await _context.Participants
                .Include(p => p.Organization)
                .Include(p => p.AbilityType)
                .Where(p => p.ParticipantID == id && !p.IsDeleted)
                .Select(p => new
                {
                    p.ParticipantID,
                    p.Name,
                    p.Age,
                    p.Gender,
                    p.BloodGroup,
                    p.OrganizationID,
                    OrganizationName = p.Organization != null ? p.Organization.Name : "Unknown",
                    p.AbilityTypeID,
                    AbilityTypeName = p.AbilityType != null ? p.AbilityType.Name : "Unknown",
                    p.Contact,
                    p.EmergencyContact,
                    p.Address,
                    p.MedicalNotes,
                    p.IsDeleted,
                    p.CreatedDate,
                    p.UpdatedDate
                })
                .FirstOrDefaultAsync();

            if (participant == null)
            {
                return NotFound();
            }

            return Ok(participant);
        }

        // PUT: api/Participants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipant(int id, ParticipantUpdateDto participantDto)
        {
            var participant = await _context.Participants.FindAsync(id);
            
            if (participant == null)
            {
                return NotFound();
            }

            participant.Name = participantDto.Name;
            participant.Age = participantDto.Age;
            participant.Gender = participantDto.Gender;
            participant.BloodGroup = participantDto.BloodGroup;
            participant.OrganizationID = participantDto.OrganizationID;
            participant.AbilityTypeID = participantDto.AbilityTypeID;
            participant.Contact = participantDto.Contact;
            participant.EmergencyContact = participantDto.EmergencyContact;
            participant.Address = participantDto.Address;
            participant.MedicalNotes = participantDto.MedicalNotes;
            participant.UpdatedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
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

        // POST: api/Participants
        [HttpPost]
        public async Task<ActionResult<Participant>> PostParticipant(ParticipantCreateDto participantDto)
        {
            var participant = new Participant
            {
                Name = participantDto.Name,
                Age = participantDto.Age,
                Gender = participantDto.Gender,
                BloodGroup = participantDto.BloodGroup,
                OrganizationID = participantDto.OrganizationID,
                AbilityTypeID = participantDto.AbilityTypeID,
                Contact = participantDto.Contact,
                EmergencyContact = participantDto.EmergencyContact,
                Address = participantDto.Address,
                MedicalNotes = participantDto.MedicalNotes,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParticipant", new { id = participant.ParticipantID }, participant);
        }

        // DELETE: api/Participants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            var participant = await _context.Participants.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            // Soft delete
            participant.IsDeleted = true;
            participant.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.ParticipantID == id && !e.IsDeleted);
        }
    }

    // DTOs for Participant operations
    public class ParticipantCreateDto
    {
        public required string Name { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public int OrganizationID { get; set; }
        public int AbilityTypeID { get; set; }
        public string? Contact { get; set; }
        public string? EmergencyContact { get; set; }
        public string? Address { get; set; }
        public string? MedicalNotes { get; set; }
    }

    public class ParticipantUpdateDto
    {
        public required string Name { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public int OrganizationID { get; set; }
        public int AbilityTypeID { get; set; }
        public string? Contact { get; set; }
        public string? EmergencyContact { get; set; }
        public string? Address { get; set; }
        public string? MedicalNotes { get; set; }
    }
}
