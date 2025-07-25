using Microsoft.AspNetCore.Identity;

namespace FriendsSociety.Shaurya.Entities
{
    public class Role : IdentityRole
    {
        public int RoleID { get; set; }
        public string? Permissions { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
