namespace FriendsSociety.Shaurya.Entities
{
    public class ParticipantGame
    {
        public int ParticipantGameID { get; set; }
        
        public int ParticipantID { get; set; }
        public Participant? Participant { get; set; }
        
        public int GameID { get; set; }
        public Game? Game { get; set; }
        
        public DateTime RegisteredDate { get; set; } = DateTime.Now;
        
        public bool IsDeleted { get; set; }
    }
}
