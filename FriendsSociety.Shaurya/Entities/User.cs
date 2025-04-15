using System.Data;

namespace FriendsSociety.Shaurya.Entities
{
    public class User
    {
        public int UserID { get; set; }

        public int RoleID { get; set; }
        public Role? Role { get; set; }

        public required string Name { get; set; }

        public int Age { get; set; }

        public int AbilityTypeID { get; set; }
        public AbilityType? AbilityType { get; set; }

        public int OrganizationID { get; set; }
        public Organization? Organization { get; set; }

        public string? Contact { get; set; }

        public bool IsDeleted { get; set; }
    }
}
