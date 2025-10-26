using FriendsSociety.Shaurya.Configuration;
using FriendsSociety.Shaurya.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FriendsSociety.Shaurya.Data
{
    public class DataContext : IdentityDbContext<User, Role, string>
    {
        private readonly bool _shouldSeed;

        public DataContext(DbContextOptions<DataContext> options, IOptions<DatabaseSettings> dbSettings) : base(options)
        {
            _shouldSeed = dbSettings.Value.SeedDemoData;
        }

        override public DbSet<User> Users { get; set; }
        override public DbSet<Role> Roles { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AbilityType> AbilityTypes { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Ground> Grounds { get; set; }
        public DbSet<ActivityCategory> ActivityCategories { get; set; }
        public DbSet<GroundAllocation> GroundAllocations { get; set; }
        public DbSet<TeamAssignment> TeamAssignments { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User → AbilityType
            modelBuilder.Entity<User>()
                .HasOne(u => u.AbilityType)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AbilityTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Organization
            modelBuilder.Entity<User>()
                .HasOne(u => u.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(u => u.OrganizationID)
                .OnDelete(DeleteBehavior.Restrict);

            // Participant → Organization
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.Organization)
                .WithMany()
                .HasForeignKey(p => p.OrganizationID)
                .OnDelete(DeleteBehavior.Restrict);

            // Participant → AbilityType
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.AbilityType)
                .WithMany()
                .HasForeignKey(p => p.AbilityTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Tournament → Activity (One-to-Many)
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.Tournament)
                .WithMany(t => t.Activities)
                .HasForeignKey(a => a.TournamentID)
                .OnDelete(DeleteBehavior.SetNull);

            // ActivityCategory → Activity & AbilityType
            modelBuilder.Entity<ActivityCategory>()
                .HasOne(ac => ac.Activity)
                .WithMany(a => a.ActivityCategories)
                .HasForeignKey(ac => ac.ActivityID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ActivityCategory>()
                .HasOne(ac => ac.AbilityType)
                .WithMany(at => at.ActivityCategories)
                .HasForeignKey(ac => ac.AbilityTypeID)
                .OnDelete(DeleteBehavior.Cascade);

            // GroundAllocation → Activity & Ground
            modelBuilder.Entity<GroundAllocation>()
                .HasOne(ga => ga.Activity)
                .WithMany(a => a.GroundAllocations)
                .HasForeignKey(ga => ga.ActivityID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroundAllocation>()
                .HasOne(ga => ga.Ground)
                .WithMany(g => g.GroundAllocations)
                .HasForeignKey(ga => ga.GroundID)
                .OnDelete(DeleteBehavior.Cascade);

            // TeamAssignment → Leader (Volunteer) & Ground
            modelBuilder.Entity<TeamAssignment>()
                .HasOne(ta => ta.Leader)
                .WithMany()
                .HasForeignKey(ta => ta.LeaderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamAssignment>()
                .HasOne(ta => ta.Ground)
                .WithMany()
                .HasForeignKey(ta => ta.GroundID)
                .OnDelete(DeleteBehavior.SetNull);

            if (_shouldSeed)
            {
                // ModelSeeder.Seed(modelBuilder); // Removed: now using service-based seeding only
            }
        }
    }
}
