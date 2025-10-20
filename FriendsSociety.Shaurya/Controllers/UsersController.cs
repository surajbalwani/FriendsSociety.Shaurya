using FriendsSociety.Shaurya.Data;
using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FriendsSociety.Shaurya.Controllers
{
    [Authorize(Roles = "Volunteer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(DataContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // DTO returned to callers including roles
        public class UserDto
        {
            public int UserID { get; set; }
            public string? UserName { get; set; }
            public string? Email { get; set; }
            public int? Age { get; set; }
            public int? AbilityTypeID { get; set; }
            public int? OrganizationID { get; set; }
            public string? Contact { get; set; }
            public bool IsDeleted { get; set; }
            public IList<string> Roles { get; set; } = new List<string>();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var result = new List<UserDto>(users.Count);

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserDto
                {
                    UserID = u.UserID,
                    UserName = u.UserName,
                    Email = u.Email,
                    Age = u.Age,
                    AbilityTypeID = u.AbilityTypeID,
                    OrganizationID = u.OrganizationID,
                    Contact = u.Contact,
                    IsDeleted = u.IsDeleted,
                    Roles = roles.ToList()
                });
            }

            return Ok(result);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var dto = new UserDto
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Email = user.Email,
                Age = user.Age,
                AbilityTypeID = user.AbilityTypeID,
                OrganizationID = user.OrganizationID,
                Contact = user.Contact,
                IsDeleted = user.IsDeleted,
                Roles = roles.ToList()
            };

            return dto;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Get roles (likely none) and return DTO
            var roles = await _userManager.GetRolesAsync(user);
            var dto = new UserDto
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Email = user.Email,
                Age = user.Age,
                AbilityTypeID = user.AbilityTypeID,
                OrganizationID = user.OrganizationID,
                Contact = user.Contact,
                IsDeleted = user.IsDeleted,
                Roles = roles.ToList()
            };

            return CreatedAtAction("GetUser", new { id = user.UserID }, dto);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }
    }
}
