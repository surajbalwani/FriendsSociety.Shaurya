﻿using System;
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
    public class GroundAllocationsController : ControllerBase
    {
        private readonly DataContext _context;

        public GroundAllocationsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/GroundAllocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroundAllocation>>> GetGroundAllocations()
        {
            return await _context.GroundAllocations.ToListAsync();
        }

        // GET: api/GroundAllocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroundAllocation>> GetGroundAllocation(int id)
        {
            var groundAllocation = await _context.GroundAllocations.FindAsync(id);

            if (groundAllocation == null)
            {
                return NotFound();
            }

            return groundAllocation;
        }

        // PUT: api/GroundAllocations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroundAllocation(int id, GroundAllocation groundAllocation)
        {
            if (id != groundAllocation.GroundAllocationID)
            {
                return BadRequest();
            }

            _context.Entry(groundAllocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroundAllocationExists(id))
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

        // POST: api/GroundAllocations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GroundAllocation>> PostGroundAllocation(GroundAllocation groundAllocation)
        {
            _context.GroundAllocations.Add(groundAllocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroundAllocation", new { id = groundAllocation.GroundAllocationID }, groundAllocation);
        }

        // DELETE: api/GroundAllocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroundAllocation(int id)
        {
            var groundAllocation = await _context.GroundAllocations.FindAsync(id);
            if (groundAllocation == null)
            {
                return NotFound();
            }

            _context.GroundAllocations.Remove(groundAllocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroundAllocationExists(int id)
        {
            return _context.GroundAllocations.Any(e => e.GroundAllocationID == id);
        }
    }
}
