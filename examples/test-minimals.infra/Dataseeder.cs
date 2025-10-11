using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using test_minimals.infra.Data;
namespace test_minimals.infra
{
    public static class Dataseeder
    {
        public static void Seed(IApplicationBuilder app)
        {
            // Seed initial data if necessary
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }
    }
}
