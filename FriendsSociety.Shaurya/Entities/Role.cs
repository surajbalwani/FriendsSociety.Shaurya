namespace FriendsSociety.Shaurya.Entities
{
    public class Role
    {
        public int RoleID { get; set; }
        public required string Name { get; set; }
        public string? Permissions { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
