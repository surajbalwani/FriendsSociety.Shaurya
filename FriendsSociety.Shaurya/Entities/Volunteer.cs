namespace FriendsSociety.Shaurya.Entities
{
    public class Volunteer
    {
        public int VolunteerID { get; set; }

        public required string Name { get; set; }

        public string? Contact { get; set; }

        public string? WhatsAppNo { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
