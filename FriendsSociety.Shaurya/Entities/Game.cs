namespace FriendsSociety.Shaurya.Entities
{
    public class Game
    {
        public int GameID { get; set; }
        
        public required string Name { get; set; }
        
        public required string GameCode { get; set; } // e.g., "1A02"
        
        public int GameCodeNumber { get; set; } // 01-08 (Game ID part)
        
        public int DisabilityTypeCode { get; set; } // 1-6
        
        public required string AgeCategory { get; set; } // A, B, C, D
        
        public int AgeRangeStart { get; set; }
        
        public int AgeRangeEnd { get; set; }
        
        public int AbilityTypeID { get; set; }
        public AbilityType? AbilityType { get; set; }
        
        public string? Description { get; set; }
        
        public string? Rules { get; set; }
        
        public bool IsDeleted { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedDate { get; set; }
    }
}
