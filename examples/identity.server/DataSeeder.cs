using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace identity.server
{
    public static class DataSeeder
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                var client = new Client
                {
                    ClientId = "test-minimals-client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "test-minimals-api" }
                };

                context.Clients.Add(client.ToEntity());
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                context.ApiScopes.Add(new ApiScope("test-minimals-api").ToEntity());
                context.SaveChanges();
            }
        }
    }
}
