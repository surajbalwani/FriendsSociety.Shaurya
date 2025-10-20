using FriendsSociety.Shaurya.Entities;
using FriendsSociety.Shaurya.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FriendsSociety.Shaurya.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public AccountController(UserManager<User> userManager,
                                 SignInManager<User> signInManager,
                                 IConfiguration config,
                                 DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            // Validate required custom properties
            if (model.AbilityTypeID <= 0)
                return BadRequest("AbilityTypeID is required and must be a positive integer.");

            var abilityExists = await _context.AbilityTypes.FindAsync(model.AbilityTypeID);
            if (abilityExists == null)
                return BadRequest("Invalid AbilityTypeID.");

            if (model.OrganizationID <= 0)
                return BadRequest("OrganizationID is required and must be a positive integer.");

            var orgExists = await _context.Organizations.FindAsync(model.OrganizationID);
            if (orgExists == null)
                return BadRequest("Invalid OrganizationID.");

            if (model.Age <= 0)
                return BadRequest("Age must be a positive integer.");

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                AbilityTypeID = model.AbilityTypeID,
                OrganizationID = model.OrganizationID,
                Age = model.Age,
                Contact = model.Contact,
                IsDeleted = false,
                EmailConfirmed = false
            };

            // If no password provided, create user without password (requires password set via reset flow).
            IdentityResult result;
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                result = await _userManager.CreateAsync(user);
            }
            else
            {
                result = await _userManager.CreateAsync(user, model.Password);
            }

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "Participant"); // Assign default role
            return Ok("Registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var token = JwtTokenHelper.GenerateJwtToken(user, roles, _config);

            return Ok(new { token });
        }
    }
    public class RegisterDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public int AbilityTypeID { get; set; }
        public int OrganizationID { get; set; }
        public int Age { get; set; }
        public string? Contact { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
