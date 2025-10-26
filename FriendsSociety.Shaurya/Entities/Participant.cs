namespace FriendsSociety.Shaurya.Entities
{
    public class Participant
    {
        public int ParticipantID { get; set; }

        public required string Name { get; set; }

        public int Age { get; set; }

        public string? Gender { get; set; }

        public string? BloodGroup { get; set; }

        public int OrganizationID { get; set; }
        public Organization? Organization { get; set; }

        public int AbilityTypeID { get; set; }
        public AbilityType? AbilityType { get; set; }

        public string? Contact { get; set; }

        public string? EmergencyContact { get; set; }

        public string? Address { get; set; }

        public string? MedicalNotes { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }
    }
}
