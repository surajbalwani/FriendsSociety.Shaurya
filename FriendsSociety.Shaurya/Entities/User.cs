using Microsoft.AspNetCore.Identity;

namespace FriendsSociety.Shaurya.Entities
{
    public class User : IdentityUser
    {
        public int UserID { get; set; }
        public Role? Role { get; set; }

        public int Age { get; set; }

        public int AbilityTypeID { get; set; }
        public AbilityType? AbilityType { get; set; }

        public int OrganizationID { get; set; }
        public Organization? Organization { get; set; }

        public string? Contact { get; set; }

        public bool IsDeleted { get; set; }
    }
}
