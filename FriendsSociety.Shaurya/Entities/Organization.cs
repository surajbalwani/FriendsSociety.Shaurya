namespace FriendsSociety.Shaurya.Entities
{
    public class Organization
    {
        public int OrganizationID { get; set; }
        public required string Name { get; set; }
        public string? Contact { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
