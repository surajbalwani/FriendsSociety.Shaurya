namespace FriendsSociety.Shaurya.Entities
{
    public class Ground
    {
        public int GroundID { get; set; }
        public required string Name { get; set; }
        public string? Location { get; set; }
        public ICollection<GroundAllocation> GroundAllocations { get; set; } = new List<GroundAllocation>();
    }
}
