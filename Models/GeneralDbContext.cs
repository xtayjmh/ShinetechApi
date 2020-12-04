using API.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class GeneralDbContext : DbContext
    {
        public GeneralDbContext(DbContextOptions<GeneralDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Parameter> Parameter { get; set; }
        /// <summary>
        /// 部门表
        /// </summary>
        public DbSet<Department> Departments { get; set; }
        public DbSet<LoginTracking> LoginTrackings { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}