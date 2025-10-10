using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace identity.server
{
    public static class IdentityModule
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityServer()
                    .AddInMemoryClients(new[]
                    {
                        new Client
                        {
                            ClientId = "client",
                            ClientSecrets = { new Secret("secret".Sha256()) },
                            AllowedGrantTypes = GrantTypes.Code, // Explicitly allow Authorization Code Flow
                            RedirectUris = { "https://localhost:7161/signin-oidc" }, // Match the client's redirect URI
                            PostLogoutRedirectUris = { "https://localhost:7161/signout-callback-oidc" }, // Optional
                            AllowedScopes = { "api1", IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile },
                            RequirePkce = true, // Enable PKCE
                            AllowPlainTextPkce = false // Enforce S256 for security
                        }
                    })
                    .AddInMemoryApiScopes(new[] { new ApiScope("api1", "My API") })
                    .AddInMemoryIdentityResources(new[] { new IdentityResources.OpenId() })
                    .AddTestUsers(new List<TestUser>
                    {
                        new TestUser
                        {
                            SubjectId = "1",
                            Username = "asem",
                            Password = "password"
                        }
                    })
                    .AddDeveloperSigningCredential(); // For development only
            return services;
        }
    }
}
