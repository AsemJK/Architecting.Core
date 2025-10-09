using Microsoft.EntityFrameworkCore;
using test_minimals.infra.Models;

namespace test_minimals.infra.Data
{
    internal class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
