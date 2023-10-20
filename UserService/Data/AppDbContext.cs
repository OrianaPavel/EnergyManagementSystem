using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using UserService.Entities;

namespace UserService.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ILogger<AppDbContext> _logger;

        public DbSet<User> Users { get; set; }

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
    }
}