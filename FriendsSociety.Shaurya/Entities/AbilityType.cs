namespace FriendsSociety.Shaurya.Entities
{
    public class AbilityType
    {
        public int AbilityTypeID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<ActivityCategory> ActivityCategories { get; set; } = new List<ActivityCategory>();
    }
}
