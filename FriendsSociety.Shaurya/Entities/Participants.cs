using System.ComponentModel.DataAnnotations;

namespace FriendsSociety.Shaurya.Entities
{
    public class Participants
    {
        [Key]
        public int ParticipantId { get; set; }
        public required string Name { get; set; }
        public int Age { get; set; }

        // [To-Do] we have to create a new enum for ability type to store options like specially abled with eyes, specially abled with hands, etc.
        public string? AbilityType { get; set; }
        public string? OrganizationName { get; set; }
        public string? Contact { get; set; }
        public string? Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}
