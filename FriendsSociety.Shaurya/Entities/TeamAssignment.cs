namespace FriendsSociety.Shaurya.Entities
{
    public class TeamAssignment
    {
        public int TeamAssignmentID { get; set; }

        public required string TeamName { get; set; }

        public int LeaderID { get; set; }
        public Volunteer? Leader { get; set; }

        public string? MemberIDs { get; set; } // Comma-separated volunteer IDs

        public int? GroundID { get; set; }
        public Ground? Ground { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; }
    }
}
