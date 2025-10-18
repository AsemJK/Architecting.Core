using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using myapi_minimals.infra.Data;
using myapi_minimals.infra.Models.Identity;
namespace myapi_minimals.infra
{
    public static class DataSeeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            // Seed initial data if necessary
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.MigrateAsync();
            }
            var users = context.Users.ToList();
            if (users.Count == 0)
            {
                // Seed default users
                PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
                context.Users.Add(new myapi_minimals.infra.Models.Identity.User
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    Email = "asemalsaiyadi@gmail.com",
                    PasswordHash = passwordHasher.HashPassword(null, "Admin123$"),
                    IsActive = true
                });
                context.SaveChanges();
            }
        }
    }
}
