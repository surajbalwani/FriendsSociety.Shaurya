using FriendsSociety.Shaurya.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendsSociety.Shaurya.Data
{
    public static class ModelSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // === Roles ===
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, Name = "Participant", Permissions = "ViewActivities" },
                new Role { RoleID = 2, Name = "Volunteer", Permissions = "ManageActivities,HelpParticipants" }
            );

            // === Ability Types ===
            modelBuilder.Entity<AbilityType>().HasData(
                new AbilityType { AbilityTypeID = 1, Name = "Hearing Impairment", Description = "Partial or total inability to hear", IsDeleted = false },
                new AbilityType { AbilityTypeID = 2, Name = "Visual Impairment", Description = "Partial or total inability to see", IsDeleted = false },
                new AbilityType { AbilityTypeID = 3, Name = "Mobility Impairment", Description = "Difficulty walking or moving", IsDeleted = false }
            );

            // === Organizations ===
            modelBuilder.Entity<Organization>().HasData(
                new Organization { OrganizationID = 1, Name = "Hope Foundation", Contact = "hope@example.org", IsDeleted = false }
            );

            // === Users ===
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    RoleID = 1,
                    Name = "Arjun Mehta",
                    Age = 24,
                    AbilityTypeID = 1,
                    OrganizationID = 1,
                    Contact = "arjun@example.com",
                    IsDeleted = false
                },
                new User
                {
                    UserID = 2,
                    RoleID = 2,
                    Name = "Nikita Shah",
                    Age = 30,
                    AbilityTypeID = 2,
                    OrganizationID = 1,
                    Contact = "nikita@example.com",
                    IsDeleted = false
                }
            );

            // === Activities ===
            modelBuilder.Entity<Activity>().HasData(
                new Activity { ActivityID = 1, Name = "Wheelchair Basketball", Rules = "Standard 5v5 rules apply", IsDeleted = false },
                new Activity { ActivityID = 2, Name = "Blind Running", Rules = "Tethered guide required", IsDeleted = false }
            );

            // === Grounds ===
            modelBuilder.Entity<Ground>().HasData(
                new Ground { GroundID = 1, Name = "Main Arena", Location = "City Sports Complex" },
                new Ground { GroundID = 2, Name = "Open Ground", Location = "Community Park" }
            );

            // === ActivityCategory (ExclusionType) ===
            modelBuilder.Entity<ActivityCategory>().HasData(
                new ActivityCategory
                {
                    ActivityCategoryID = 1,
                    ActivityID = 2,
                    AbilityTypeID = 2
                }
            );

            // === Ground Allocation ===
            modelBuilder.Entity<GroundAllocation>().HasData(
                new GroundAllocation
                {
                    GroundAllocationID = 1,
                    GroundID = 1,
                    ActivityID = 1,
                    StartTime = new DateTime(2025, 05, 01, 10, 0, 0),
                    EndTime = new DateTime(2025, 05, 01, 12, 0, 0)
                },
                new GroundAllocation
                {
                    GroundAllocationID = 2,
                    GroundID = 2,
                    ActivityID = 2,
                    StartTime = new DateTime(2025, 05, 01, 13, 0, 0),
                    EndTime = new DateTime(2025, 05, 01, 14, 30, 0)
                }
            );
        }
    }
}