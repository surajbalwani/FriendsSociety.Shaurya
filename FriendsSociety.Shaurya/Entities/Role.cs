using Microsoft.AspNetCore.Identity;

namespace FriendsSociety.Shaurya.Entities
{
    public class Role : IdentityRole
    {
        public string? Permissions { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
