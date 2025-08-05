using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FriendsSociety.Shaurya.Data
{
    public static class ModelSeeder
    {
        // Service-based runtime seeding without explicit identity values
        public static async Task SeedDemoDataAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

            // === Roles ===
            List<Role> rolesList = new List<Role>
            {
                new Role { Name = "Participant", Permissions = "ViewActivities" },
                new Role { Name = "Volunteer", Permissions = "ManageActivities,HelpParticipants" }
            };
            foreach (var role in rolesList)
            {
                if (!string.IsNullOrEmpty(role.Name) && !await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(role);
                }
            }

            // === Ability Types ===
            if (!context.AbilityTypes.Any())
            {
                var abilityTypes = new[]
                {
                    new AbilityType { Name = "Hearing Impairment", Description = "Partial or total inability to hear", IsDeleted = false },
                    new AbilityType { Name = "Visual Impairment", Description = "Partial or total inability to see", IsDeleted = false },
                    new AbilityType { Name = "Mobility Impairment", Description = "Difficulty walking or moving", IsDeleted = false }
                };
                context.AbilityTypes.AddRange(abilityTypes);
                await context.SaveChangesAsync();

                // Get generated IDs
                var abilityType1 = abilityTypes[0].AbilityTypeID;
                var abilityType2 = abilityTypes[1].AbilityTypeID;

                // === Organizations ===
                if (!context.Organizations.Any())
                {
                    var org = new Organization { Name = "Hope Foundation", Contact = "hope@example.org", IsDeleted = false };
                    context.Organizations.Add(org);
                    await context.SaveChangesAsync();

                    // === Users ===
                    if (!context.Users.Any())
                    {
                        context.Users.AddRange(
                            new User
                            {
                                UserName = "Arjun Mehta",
                                Age = 24,
                                AbilityTypeID = abilityType1,
                                OrganizationID = org.OrganizationID,
                                Contact = "arjun@example.com",
                                IsDeleted = false
                            },
                            new User
                            {
                                UserName = "Nikita Shah",
                                Age = 30,
                                AbilityTypeID = abilityType2,
                                OrganizationID = org.OrganizationID,
                                Contact = "nikita@example.com",
                                IsDeleted = false
                            }
                        );
                        await context.SaveChangesAsync();
                    }
                }

                // === Tournaments ===
                if (!context.Tournaments.Any())
                {
                    var tournaments = new[]
                    {
                        new Tournament 
                        { 
                            Name = "Spring Adaptive Sports Championship 2025",
                            Description = "Annual championship featuring various adaptive sports for athletes with different abilities",
                            StartDate = new DateTime(2025, 5, 1, 8, 0, 0),
                            EndDate = new DateTime(2025, 5, 3, 18, 0, 0),
                            Location = "Central Sports Complex",
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Tournament 
                        { 
                            Name = "Community Inclusion Games",
                            Description = "Monthly community games promoting inclusion and participation",
                            StartDate = new DateTime(2025, 6, 15, 9, 0, 0),
                            EndDate = new DateTime(2025, 6, 15, 17, 0, 0),
                            Location = "Community Recreation Center",
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Tournament 
                        { 
                            Name = "Inter-Organization Challenge",
                            Description = "Competition between different organizations",
                            StartDate = new DateTime(2025, 7, 20, 10, 0, 0),
                            EndDate = new DateTime(2025, 7, 22, 16, 0, 0),
                            Location = "Multi-Purpose Sports Facility",
                            IsActive = false,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        }
                    };
                    context.Tournaments.AddRange(tournaments);
                    await context.SaveChangesAsync();

                    // === Activities ===
                    if (!context.Activities.Any())
                    {
                        var activities = new[]
                        {
                            new Activity 
                            { 
                                Name = "Wheelchair Basketball", 
                                Rules = "Standard 5v5 rules apply with adaptive equipment", 
                                IsDeleted = false,
                                TournamentID = tournaments[0].TournamentID  // Assign to Spring Championship
                            },
                            new Activity 
                            { 
                                Name = "Blind Running", 
                                Rules = "Tethered guide required, 100m and 400m events", 
                                IsDeleted = false,
                                TournamentID = tournaments[0].TournamentID  // Assign to Spring Championship
                            },
                            new Activity 
                            { 
                                Name = "Sitting Volleyball", 
                                Rules = "Modified volleyball rules for seated players", 
                                IsDeleted = false,
                                TournamentID = tournaments[1].TournamentID  // Assign to Community Games
                            },
                            new Activity 
                            { 
                                Name = "Boccia Competition", 
                                Rules = "Paralympic boccia rules and regulations", 
                                IsDeleted = false,
                                TournamentID = tournaments[1].TournamentID  // Assign to Community Games
                            },
                            new Activity 
                            { 
                                Name = "Swimming Relay", 
                                Rules = "Mixed ability relay teams, 4x50m freestyle", 
                                IsDeleted = false,
                                TournamentID = tournaments[2].TournamentID  // Assign to Inter-Organization Challenge
                            },
                            new Activity 
                            { 
                                Name = "Table Tennis Singles", 
                                Rules = "Standard table tennis rules with classifications", 
                                IsDeleted = false
                                // No tournament assigned - standalone activity
                            }
                        };
                        context.Activities.AddRange(activities);
                        await context.SaveChangesAsync();

                        // === ActivityCategory (ExclusionType) ===
                        if (!context.ActivityCategories.Any())
                        {
                            context.ActivityCategories.AddRange(
                                new ActivityCategory
                                {
                                    ActivityID = activities[1].ActivityID, // Blind Running
                                    AbilityTypeID = abilityType2 // Visual Impairment
                                },
                                new ActivityCategory
                                {
                                    ActivityID = activities[0].ActivityID, // Wheelchair Basketball
                                    AbilityTypeID = abilityType1 // Mobility Impairment (changed from Hearing)
                                },
                                new ActivityCategory
                                {
                                    ActivityID = activities[2].ActivityID, // Sitting Volleyball
                                    AbilityTypeID = abilityType1 // Mobility Impairment
                                }
                            );
                            await context.SaveChangesAsync();
                        }

                        // === Grounds ===
                        if (!context.Grounds.Any())
                        {
                            var grounds = new[]
                            {
                                new Ground { Name = "Main Arena", Location = "City Sports Complex" },
                                new Ground { Name = "Open Ground", Location = "Community Park" },
                                new Ground { Name = "Swimming Pool", Location = "Aquatic Center" },
                                new Ground { Name = "Indoor Court A", Location = "Recreation Center" },
                                new Ground { Name = "Indoor Court B", Location = "Recreation Center" }
                            };
                            context.Grounds.AddRange(grounds);
                            await context.SaveChangesAsync();

                            // === Ground Allocation ===
                            if (!context.GroundAllocations.Any())
                            {
                                context.GroundAllocations.AddRange(
                                    // Spring Championship allocations
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[0].GroundID, // Main Arena
                                        ActivityID = activities[0].ActivityID, // Wheelchair Basketball
                                        StartTime = new DateTime(2025, 5, 1, 10, 0, 0),
                                        EndTime = new DateTime(2025, 5, 1, 12, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[1].GroundID, // Open Ground
                                        ActivityID = activities[1].ActivityID, // Blind Running
                                        StartTime = new DateTime(2025, 5, 1, 13, 0, 0),
                                        EndTime = new DateTime(2025, 5, 1, 14, 30, 0)
                                    },
                                    // Community Games allocations
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[3].GroundID, // Indoor Court A
                                        ActivityID = activities[2].ActivityID, // Sitting Volleyball
                                        StartTime = new DateTime(2025, 6, 15, 9, 0, 0),
                                        EndTime = new DateTime(2025, 6, 15, 11, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[4].GroundID, // Indoor Court B
                                        ActivityID = activities[3].ActivityID, // Boccia Competition
                                        StartTime = new DateTime(2025, 6, 15, 11, 30, 0),
                                        EndTime = new DateTime(2025, 6, 15, 13, 0, 0)
                                    },
                                    // Inter-Organization Challenge allocations
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[2].GroundID, // Swimming Pool
                                        ActivityID = activities[4].ActivityID, // Swimming Relay
                                        StartTime = new DateTime(2025, 7, 20, 14, 0, 0),
                                        EndTime = new DateTime(2025, 7, 20, 16, 0, 0)
                                    },
                                    // Standalone activity allocation
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[4].GroundID, // Indoor Court B
                                        ActivityID = activities[5].ActivityID, // Table Tennis Singles
                                        StartTime = new DateTime(2025, 8, 10, 10, 0, 0),
                                        EndTime = new DateTime(2025, 8, 10, 12, 0, 0)
                                    }
                                );
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }
    }
}