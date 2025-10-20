using FriendsSociety.Shaurya.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
            var logger = Log.ForContext(typeof(ModelSeeder));

            try
            {
                // Test database connection first
                await context.Database.CanConnectAsync();
                logger.Information("Database connection verified successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Cannot connect to database. Skipping data seeding.");
                return;
            }

            try
            {
                // === Roles ===
                var roles = new List<(string Name, string Permissions)>
                {
                    ("Participant", "ViewActivities"),
                    ("Volunteer", "ManageActivities,HelpParticipants")
                };

                foreach (var roleInfo in roles)
                {
                    if (!await roleManager.RoleExistsAsync(roleInfo.Name))
                    {
                        var role = new Role
                        {
                            Name = roleInfo.Name,
                            NormalizedName = roleInfo.Name.ToUpperInvariant(),
                            Permissions = roleInfo.Permissions
                        };
                        var result = await roleManager.CreateAsync(role);
                        
                        if (!result.Succeeded)
                        {
                            logger.Error("Failed to create role {RoleName}: {Errors}", 
                                roleInfo.Name, 
                                string.Join(", ", result.Errors.Select(e => e.Description)));
                            throw new Exception($"Failed to create role {roleInfo.Name}");
                        }
                        
                        logger.Information("Created role {RoleName} successfully", roleInfo.Name);
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
                            Name = "Summer Paralympic Games 2025", 
                            Description = "Annual summer tournament featuring adaptive sports for athletes with various abilities",
                            StartDate = new DateTime(2025, 07, 15, 9, 0, 0),
                            EndDate = new DateTime(2025, 07, 25, 18, 0, 0),
                            Location = "Central Sports Complex",
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Tournament 
                        { 
                            Name = "Unity Sports Festival", 
                            Description = "A celebration of inclusive sports bringing together athletes of all abilities",
                            StartDate = new DateTime(2025, 09, 10, 8, 0, 0),
                            EndDate = new DateTime(2025, 09, 12, 17, 0, 0),
                            Location = "Community Sports Hub",
                            IsActive = true,
                            IsDeleted = false,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Tournament 
                        { 
                            Name = "Winter Challenge Cup", 
                            Description = "Indoor winter sports competition focusing on team-based activities",
                            StartDate = new DateTime(2025, 12, 05, 10, 0, 0),
                            EndDate = new DateTime(2025, 12, 08, 16, 0, 0),
                            Location = "Indoor Arena Complex",
                            IsActive = false, // Not yet started
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
                            // Activities for Summer Paralympic Games 2025
                            new Activity { Name = "Wheelchair Basketball", Rules = "Standard 5v5 rules apply", IsDeleted = false, TournamentID = tournaments[0].TournamentID },
                            new Activity { Name = "Blind Running", Rules = "Tethered guide required", IsDeleted = false, TournamentID = tournaments[0].TournamentID },
                            new Activity { Name = "Wheelchair Rugby", Rules = "Full contact sport played in wheelchairs", IsDeleted = false, TournamentID = tournaments[0].TournamentID },
                            new Activity { Name = "Goalball", Rules = "Ball sport designed for visually impaired athletes", IsDeleted = false, TournamentID = tournaments[0].TournamentID },
                            
                            // Activities for Unity Sports Festival
                            new Activity { Name = "Adaptive Swimming", Rules = "Modified swimming rules for different abilities", IsDeleted = false, TournamentID = tournaments[1].TournamentID },
                            new Activity { Name = "Seated Volleyball", Rules = "Volleyball played from seated position", IsDeleted = false, TournamentID = tournaments[1].TournamentID },
                            new Activity { Name = "Para Table Tennis", Rules = "Table tennis for players with physical impairments", IsDeleted = false, TournamentID = tournaments[1].TournamentID },
                            
                            // Activities for Winter Challenge Cup
                            new Activity { Name = "Wheelchair Handball", Rules = "Indoor handball adapted for wheelchair users", IsDeleted = false, TournamentID = tournaments[2].TournamentID },
                            new Activity { Name = "Blind Football", Rules = "5-a-side football for visually impaired players", IsDeleted = false, TournamentID = tournaments[2].TournamentID },
                            
                            // Standalone activities (not part of any tournament)
                            new Activity { Name = "Community Yoga", Rules = "Adaptive yoga for all abilities", IsDeleted = false },
                            new Activity { Name = "Therapeutic Swimming", Rules = "Individual swimming therapy sessions", IsDeleted = false }
                        };
                        context.Activities.AddRange(activities);
                        await context.SaveChangesAsync();

                        // === ActivityCategory (ExclusionType) ===
                        if (!context.ActivityCategories.Any())
                        {
                            context.ActivityCategories.AddRange(
                                // Blind Running is for Visual Impairment
                                new ActivityCategory
                                {
                                    ActivityID = activities[1].ActivityID, // Blind Running
                                    AbilityTypeID = abilityType2 // Visual Impairment
                                },
                                // Goalball is also for Visual Impairment
                                new ActivityCategory
                                {
                                    ActivityID = activities[3].ActivityID, // Goalball
                                    AbilityTypeID = abilityType2 // Visual Impairment
                                },
                                // Wheelchair Basketball is for Mobility Impairment
                                new ActivityCategory
                                {
                                    ActivityID = activities[0].ActivityID, // Wheelchair Basketball
                                    AbilityTypeID = abilityTypes[2].AbilityTypeID // Mobility Impairment
                                },
                                // Blind Football is for Visual Impairment
                                new ActivityCategory
                                {
                                    ActivityID = activities[8].ActivityID, // Blind Football
                                    AbilityTypeID = abilityType2 // Visual Impairment
                                }
                            );
                            await context.SaveChangesAsync();
                        }

                        // === Grounds ===
                        if (!context.Grounds.Any())
                        {
                            var grounds = new[]
                            {
                                new Ground { Name = "Main Arena", Location = "Central Sports Complex" },
                                new Ground { Name = "Open Ground", Location = "Community Park" },
                                new Ground { Name = "Swimming Pool", Location = "Aquatic Center" },
                                new Ground { Name = "Indoor Court A", Location = "Indoor Arena Complex" },
                                new Ground { Name = "Indoor Court B", Location = "Indoor Arena Complex" },
                                new Ground { Name = "Track Field", Location = "Athletics Stadium" }
                            };
                            context.Grounds.AddRange(grounds);
                            await context.SaveChangesAsync();

                            // === Ground Allocation ===
                            if (!context.GroundAllocations.Any())
                            {
                                context.GroundAllocations.AddRange(
                                    // Summer Paralympic Games allocations
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[0].GroundID, // Main Arena
                                        ActivityID = activities[0].ActivityID, // Wheelchair Basketball
                                        StartTime = new DateTime(2025, 07, 16, 10, 0, 0),
                                        EndTime = new DateTime(2025, 07, 16, 12, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[5].GroundID, // Track Field
                                        ActivityID = activities[1].ActivityID, // Blind Running
                                        StartTime = new DateTime(2025, 07, 17, 9, 0, 0),
                                        EndTime = new DateTime(2025, 07, 17, 11, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[0].GroundID, // Main Arena
                                        ActivityID = activities[2].ActivityID, // Wheelchair Rugby
                                        StartTime = new DateTime(2025, 07, 18, 14, 0, 0),
                                        EndTime = new DateTime(2025, 07, 18, 16, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[3].GroundID, // Indoor Court A
                                        ActivityID = activities[3].ActivityID, // Goalball
                                        StartTime = new DateTime(2025, 07, 19, 11, 0, 0),
                                        EndTime = new DateTime(2025, 07, 19, 13, 0, 0)
                                    },

                                    // Unity Sports Festival allocations
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[2].GroundID, // Swimming Pool
                                        ActivityID = activities[4].ActivityID, // Adaptive Swimming
                                        StartTime = new DateTime(2025, 09, 10, 8, 0, 0),
                                        EndTime = new DateTime(2025, 09, 10, 10, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[3].GroundID, // Indoor Court A
                                        ActivityID = activities[5].ActivityID, // Seated Volleyball
                                        StartTime = new DateTime(2025, 09, 11, 10, 0, 0),
                                        EndTime = new DateTime(2025, 09, 11, 12, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[4].GroundID, // Indoor Court B
                                        ActivityID = activities[6].ActivityID, // Para Table Tennis
                                        StartTime = new DateTime(2025, 09, 12, 13, 0, 0),
                                        EndTime = new DateTime(2025, 09, 12, 15, 0, 0)
                                    },

                                    // Winter Challenge Cup allocations
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[3].GroundID, // Indoor Court A
                                        ActivityID = activities[7].ActivityID, // Wheelchair Handball
                                        StartTime = new DateTime(2025, 12, 05, 10, 0, 0),
                                        EndTime = new DateTime(2025, 12, 05, 12, 0, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[4].GroundID, // Indoor Court B
                                        ActivityID = activities[8].ActivityID, // Blind Football
                                        StartTime = new DateTime(2025, 12, 06, 14, 0, 0),
                                        EndTime = new DateTime(2025, 12, 06, 16, 0, 0)
                                    },

                                    // Standalone activities allocations
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[1].GroundID, // Open Ground
                                        ActivityID = activities[9].ActivityID, // Community Yoga
                                        StartTime = new DateTime(2025, 08, 15, 7, 0, 0),
                                        EndTime = new DateTime(2025, 08, 15, 8, 30, 0)
                                    },
                                    new GroundAllocation
                                    {
                                        GroundID = grounds[2].GroundID, // Swimming Pool
                                        ActivityID = activities[10].ActivityID, // Therapeutic Swimming
                                        StartTime = new DateTime(2025, 08, 20, 16, 0, 0),
                                        EndTime = new DateTime(2025, 08, 20, 17, 0, 0)
                                    }
                                );
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred during data seeding. Some data may not have been seeded properly.");
                throw; // Re-throw to be caught by the caller
            }
        }
    }
}