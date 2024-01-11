using MonitoringComService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;


namespace MonitoringComService.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ILogger<AppDbContext> _logger;

        public DbSet<Device> Devices { get; set; }
        public DbSet<Measurement> Measurements { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> dbContextOption, ILogger<AppDbContext> logger) : base(dbContextOption)
        {
            _logger = logger;
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if(databaseCreator != null)
                {
                    //create database if it cannot connect
                    if(!databaseCreator.CanConnect()) databaseCreator.Create();
                    //create tables if no tables exist
                    if(!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch(Exception e)
            {
                _logger.LogError("--> "+e.Message);
            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>()
                .HasMany(d => d.Measurements)
                .WithOne(m => m.Device)
                .HasForeignKey(m => m.DeviceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}