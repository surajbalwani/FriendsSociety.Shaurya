namespace FriendsSociety.Shaurya.Entities
{
    public class TeamAssignment
    {
        public int TeamAssignmentID { get; set; }

        public required string TeamName { get; set; }

        public int LeaderID { get; set; }
        public Leader? Leader { get; set; }

        public string? MemberIDs { get; set; } // Comma-separated user IDs

        public int? GroundID { get; set; }
        public Ground? Ground { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
        public string Name { get; set; }
    }
}
