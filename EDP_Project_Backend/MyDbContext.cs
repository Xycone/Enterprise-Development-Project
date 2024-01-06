using EDP_Project_Backend.Models;
using Microsoft.EntityFrameworkCore;
namespace EDP_Project_Backend
{
    public class MyDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public MyDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder
        optionsBuilder)
        {
            string? connectionString = _configuration.GetConnectionString(
            "MyConnection");
            if (connectionString != null)
            {
                optionsBuilder.UseMySQL(connectionString);
            }
        }

        // Make sure to add here when new model is created
        public DbSet<Tier> Tiers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Perk> Perks { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
