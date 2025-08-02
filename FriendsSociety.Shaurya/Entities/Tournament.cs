namespace FriendsSociety.Shaurya.Entities
{
    public class Tournament
    {
        public int TournamentID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property - A tournament can have multiple activities/games
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}