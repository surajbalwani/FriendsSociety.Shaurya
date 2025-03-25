using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FriendsSociety.Shaurya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ApplicationUserController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var applicationUsers = await _dataContext.ApplicationUsers.ToListAsync();

            return Ok(applicationUsers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var applicationUser = await _dataContext.ApplicationUsers.FindAsync(id);
            if (applicationUser == null)
                return NotFound("User not found");

            return Ok(applicationUser);
        }

        [HttpPost]
        public async Task<ActionResult<List<ApplicationUser>>> AddUser(ApplicationUser user)
        {
            var applicationUser = await _dataContext.ApplicationUsers.AddAsync(user);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.ApplicationUsers.ToListAsync());
        }

        [HttpPut]
        public async Task<ActionResult<List<ApplicationUser>>> UpdateUser(ApplicationUser user)
        {
            var dbApplicationUser = await _dataContext.ApplicationUsers.FindAsync(user.Id);

            if(dbApplicationUser is null)
                return NotFound("User not found");

            dbApplicationUser.Name = user.Name;
            dbApplicationUser.FirstName = user.FirstName;
            dbApplicationUser.LastName = user.LastName;
            dbApplicationUser.Place = user.Place;
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.ApplicationUsers.ToListAsync());
        }

        [HttpDelete]
        public async Task<ActionResult<List<ApplicationUser>>> DeleteUser(int id)
        {
            var dbApplicationUser = await _dataContext.ApplicationUsers.FindAsync(id);

            if (dbApplicationUser is null)
                return NotFound("User not found");

            _dataContext.ApplicationUsers.Remove(dbApplicationUser);
            await _dataContext.SaveChangesAsync();

            return Ok(await _dataContext.ApplicationUsers.ToListAsync());
        }
    }
}
