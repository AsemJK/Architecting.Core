using Microsoft.EntityFrameworkCore;
using myapi_minimals.infra.Models;
using myapi_minimals.infra.Models.Identity;

namespace myapi_minimals.infra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<ToDo> ToDos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
