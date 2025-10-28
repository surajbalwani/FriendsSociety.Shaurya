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
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
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
                    new AbilityType { Name = "Physically challenged by leg", Description = "Physical disability affecting leg movement", IsDeleted = false },
                    new AbilityType { Name = "Physically challenged by Hand", Description = "Physical disability affecting hand/arm movement", IsDeleted = false },
                    new AbilityType { Name = "Intellectually challenged", Description = "Intellectual or cognitive disability", IsDeleted = false },
                    new AbilityType { Name = "DEAF AND DUMB", Description = "Deaf and speech impaired", IsDeleted = false },
                    new AbilityType { Name = "VISUALLY IMPAIRED & PARTIALLY VISUALLY IMPAIRED", Description = "Visual impairment or partial blindness", IsDeleted = false },
                    new AbilityType { Name = "LEARNING DISABILITY", Description = "Learning difficulties and disabilities", IsDeleted = false }
                };
                context.AbilityTypes.AddRange(abilityTypes);
                await context.SaveChangesAsync();
                logger.Information("Seeded {Count} ability types successfully", abilityTypes.Length);

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
                        // Use UserManager to create identity users and assign roles
                        var pwd = "P@ssW0rd123!"; // demo password that should satisfy default password policy

                        var user1 = new User
                        {
                            UserName = "arjun@example.com",
                            Email = "arjun@example.com",
                            Age = 24,
                            AbilityTypeID = abilityType1,
                            OrganizationID = org.OrganizationID,
                            Contact = "arjun@example.com",
                            IsDeleted = false,
                            EmailConfirmed = true
                        };

                        var user2 = new User
                        {
                            UserName = "nikita@example.com",
                            Email = "nikita@example.com",
                            Age = 30,
                            AbilityTypeID = abilityType2,
                            OrganizationID = org.OrganizationID,
                            Contact = "nikita@example.com",
                            IsDeleted = false,
                            EmailConfirmed = true
                        };

                        var create1 = await userManager.CreateAsync(user1, pwd);
                        if (!create1.Succeeded)
                        {
                            logger.Error("Failed to create user {Email}: {Errors}", user1.Email, string.Join(", ", create1.Errors.Select(e => e.Description)));
                        }
                        else
                        {
                            var addRoleRes = await userManager.AddToRoleAsync(user1, "Volunteer");
                            if (!addRoleRes.Succeeded)
                                logger.Error("Failed to add role Volunteer to {Email}: {Errors}", user1.Email, string.Join(", ", addRoleRes.Errors.Select(e => e.Description)));
                        }

                        var create2 = await userManager.CreateAsync(user2, pwd);
                        if (!create2.Succeeded)
                        {
                            logger.Error("Failed to create user {Email}: {Errors}", user2.Email, string.Join(", ", create2.Errors.Select(e => e.Description)));
                        }
                        else
                        {
                            var addRoleRes2 = await userManager.AddToRoleAsync(user2, "Participant");
                            if (!addRoleRes2.Succeeded)
                                logger.Error("Failed to add role Participant to {Email}: {Errors}", user2.Email, string.Join(", ", addRoleRes2.Errors.Select(e => e.Description)));
                        }

                        // Ensure changes are visible in the DbContext
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

                            // === Volunteers ===
                            if (!context.Volunteers.Any())
                            {
                                var volunteers = new[]
                                {
                                    new Volunteer 
                                    { 
                                        Name = "Rajesh Kumar", 
                                        Age = 28, 
                                        BloodGroup = "O+", 
                                        Contact = "9876543210",
                                        WhatsAppNo = "9876543210",
                                        Email = "rajesh.kumar@example.com",
                                        Address = "123, MG Road, Bangalore",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Priya Sharma", 
                                        Age = 25, 
                                        BloodGroup = "A+", 
                                        Contact = "9123456789",
                                        WhatsAppNo = "9123456789",
                                        Email = "priya.sharma@example.com",
                                        Address = "456, Park Street, Mumbai",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Amit Patel", 
                                        Age = 32, 
                                        BloodGroup = "B+", 
                                        Contact = "9988776655",
                                        WhatsAppNo = "9988776655",
                                        Email = "amit.patel@example.com",
                                        Address = "789, SG Highway, Ahmedabad",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Sneha Reddy", 
                                        Age = 27, 
                                        BloodGroup = "AB+", 
                                        Contact = "9445566778",
                                        WhatsAppNo = "9445566778",
                                        Email = "sneha.reddy@example.com",
                                        Address = "321, Brigade Road, Hyderabad",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Vikram Singh", 
                                        Age = 30, 
                                        BloodGroup = "O-", 
                                        Contact = "9556677889",
                                        WhatsAppNo = "9556677889",
                                        Email = "vikram.singh@example.com",
                                        Address = "654, Connaught Place, Delhi",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Anjali Desai", 
                                        Age = 24, 
                                        BloodGroup = "A-", 
                                        Contact = "9667788990",
                                        WhatsAppNo = "9667788990",
                                        Email = "anjali.desai@example.com",
                                        Address = "987, FC Road, Pune",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Karthik Nair", 
                                        Age = 29, 
                                        BloodGroup = "B-", 
                                        Contact = "9778899001",
                                        WhatsAppNo = "9778899001",
                                        Email = "karthik.nair@example.com",
                                        Address = "147, MG Road, Kochi",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Meera Iyer", 
                                        Age = 26, 
                                        BloodGroup = "AB-", 
                                        Contact = "9889900112",
                                        WhatsAppNo = "9889900112",
                                        Email = "meera.iyer@example.com",
                                        Address = "258, Anna Salai, Chennai",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Arjun Gupta", 
                                        Age = 31, 
                                        BloodGroup = "O+", 
                                        Contact = "9990011223",
                                        WhatsAppNo = "9990011223",
                                        Email = "arjun.gupta@example.com",
                                        Address = "369, Park Road, Kolkata",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    },
                                    new Volunteer 
                                    { 
                                        Name = "Deepika Menon", 
                                        Age = 23, 
                                        BloodGroup = "A+", 
                                        Contact = "9001122334",
                                        WhatsAppNo = "9001122334",
                                        Email = "deepika.menon@example.com",
                                        Address = "741, Beach Road, Visakhapatnam",
                                        IsDeleted = false,
                                        CreatedDate = DateTime.Now
                                    }
                                };
                                context.Volunteers.AddRange(volunteers);
                                await context.SaveChangesAsync();
                                logger.Information("Seeded {Count} volunteers successfully", volunteers.Length);
                            }

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

                            // === Games ===
                            if (!context.Games.Any())
                            {
                                var games = new List<Game>();
                                var gameDefinitions = new[]
                                {
                                    new { Id = 1, Name = "Archery" },
                                    new { Id = 2, Name = "Bocce" },
                                    new { Id = 3, Name = "Carrom" },
                                    new { Id = 4, Name = "Goal The Ball" },
                                    new { Id = 5, Name = "Shotput" },
                                    new { Id = 6, Name = "50m Race" },
                                    new { Id = 7, Name = "100m Race" },
                                    new { Id = 8, Name = "200m Race" }
                                };

                                var ageCategories = new[]
                                {
                                    new { Code = "A", Start = 8, End = 12 },
                                    new { Code = "B", Start = 13, End = 17 },
                                    new { Code = "C", Start = 18, End = 22 },
                                    new { Code = "D", Start = 23, End = 27 }
                                };

                                // Create games for each combination
                                for (int disabilityCode = 1; disabilityCode <= 6; disabilityCode++)
                                {
                                    foreach (var ageCategory in ageCategories)
                                    {
                                        foreach (var gameDef in gameDefinitions)
                                        {
                                            var gameCode = $"{disabilityCode}{ageCategory.Code}{gameDef.Id:D2}";
                                            games.Add(new Game
                                            {
                                                Name = gameDef.Name,
                                                GameCode = gameCode,
                                                GameCodeNumber = gameDef.Id,
                                                DisabilityTypeCode = disabilityCode,
                                                AgeCategory = ageCategory.Code,
                                                AgeRangeStart = ageCategory.Start,
                                                AgeRangeEnd = ageCategory.End,
                                                AbilityTypeID = abilityTypes[disabilityCode - 1].AbilityTypeID,
                                                Description = $"{gameDef.Name} for {abilityTypes[disabilityCode - 1].Name}, Age {ageCategory.Start}-{ageCategory.End}",
                                                Rules = $"Standard rules for {gameDef.Name}",
                                                IsDeleted = false
                                            });
                                        }
                                    }
                                }

                                context.Games.AddRange(games);
                                await context.SaveChangesAsync();
                                logger.Information("Seeded {Count} games successfully", games.Count);
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