using FriendsSociety.Shaurya.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendsSociety.Shaurya.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
