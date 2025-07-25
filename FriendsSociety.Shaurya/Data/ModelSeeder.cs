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

                // === Activities ===
                if (!context.Activities.Any())
                {
                    var activities = new[]
                    {
                        new Activity { Name = "Wheelchair Basketball", Rules = "Standard 5v5 rules apply", IsDeleted = false },
                        new Activity { Name = "Blind Running", Rules = "Tethered guide required", IsDeleted = false }
                    };
                    context.Activities.AddRange(activities);
                    await context.SaveChangesAsync();

                    // === ActivityCategory (ExclusionType) ===
                    if (!context.ActivityCategories.Any())
                    {
                        context.ActivityCategories.Add(
                            new ActivityCategory
                            {
                                ActivityID = activities[1].ActivityID,
                                AbilityTypeID = abilityType2
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
                            new Ground { Name = "Open Ground", Location = "Community Park" }
                        };
                        context.Grounds.AddRange(grounds);
                        await context.SaveChangesAsync();

                        // === Ground Allocation ===
                        if (!context.GroundAllocations.Any())
                        {
                            context.GroundAllocations.AddRange(
                                new GroundAllocation
                                {
                                    GroundID = grounds[0].GroundID,
                                    ActivityID = activities[0].ActivityID,
                                    StartTime = new DateTime(2025, 05, 01, 10, 0, 0),
                                    EndTime = new DateTime(2025, 05, 01, 12, 0, 0)
                                },
                                new GroundAllocation
                                {
                                    GroundID = grounds[1].GroundID,
                                    ActivityID = activities[1].ActivityID,
                                    StartTime = new DateTime(2025, 05, 01, 13, 0, 0),
                                    EndTime = new DateTime(2025, 05, 01, 14, 30, 0)
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