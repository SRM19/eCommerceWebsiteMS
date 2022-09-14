using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Foody.Services.Identity.Utils
{
    public static class Constants
    {
        public const string Admin = "Admin";

        public const string Customer = "Customer";

        //new identity resources
        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
        };

        //create new api scope to assign to client and perform operation accordingly

        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            new ApiScope("foody","Foody Server"),
            new ApiScope("read","Read your data."),
            new ApiScope("write","Write your data."),
            new ApiScope("delete","Delete your data."),
        };

        //create new client: generic and specific, profile is inbuilt scope
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                ClientId = "client",
                ClientSecrets = {new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = {"read","write","profile"}
            },
            //this client will be used in the application
            new Client
            {
                ClientId = "foody",
                ClientSecrets = {new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = {"https://localhost:44392/signin-oidc"},
                PostLogoutRedirectUris = {"https://localhost:44392/signout-callback-oidc"},
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "foody"
                }
            }
        };

    }
}
