using Microsoft.EntityFrameworkCore;

namespace Project_Razgrom_v_9._184
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Rolls> Rolls { get; set; }
        public DbSet<Linker> Linker { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Banners> Banners { get; set; }

    }
}
