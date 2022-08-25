using ProjectManagement.Models;

namespace ProjectManagement.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>().Navigation(t => t.Users).AutoInclude();
            modelBuilder.Entity<Team>().Navigation(t => t.Projects).AutoInclude();

            modelBuilder.Entity<User>().Navigation(u => u.InviteRequests).AutoInclude();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<InviteRequest> InviteRequests { get; set; }
    }
}
