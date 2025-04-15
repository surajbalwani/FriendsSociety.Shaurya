namespace FriendsSociety.Shaurya.Entities
{
    public class Activity
    {
        public int ActivityID { get; set; }
        public required string Name { get; set; }
        public string? Rules { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ActivityCategory> ActivityCategories { get; set; } = new List<ActivityCategory>();

        public ICollection<GroundAllocation> GroundAllocations { get; set; } = new List<GroundAllocation>();
    }
}
