using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Services
{
    public class AuthenticationService
    {
        private IConfiguration configuration;
        private string clientId;
        private string[] scopes;
        private string tenantId;
        private IPublicClientApplication identityClient;

        public IPublicClientApplication IdentityClient
        {
            get
            {
                if (identityClient != null)
                    return identityClient;
#if ANDROID
                identityClient = PublicClientApplicationBuilder
                    .Create(this.clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, this.tenantId)
                    .WithRedirectUri($"msal{this.clientId}://auth")
                    .WithParentActivityOrWindow(() => Platform.CurrentActivity)
                    .Build();
#elif IOS
                identityClient = PublicClientApplicationBuilder
                    .Create(this.clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, this.tenantId)
                    .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                    .WithRedirectUri($"msal{this.clientId}://auth")
                    .Build();
#else
                identityClient = PublicClientApplicationBuilder
                    .Create(this.clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, this.tenantId)
                    .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                    .Build();
#endif
                return identityClient;
            }
        }



        public AuthenticationService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.clientId = configuration["ClientId"];
            this.scopes = new[] { configuration["Scopes"] };
            this.tenantId = configuration["TenantId"];
        }

        public async Task<bool> LogoutAsync()
        {
            var accounts = await IdentityClient.GetAccountsAsync();

            if (accounts == null)
                return true;

            foreach (var account in accounts)
                await IdentityClient.RemoveAsync(account);


            return true;


        }


        public async Task<AuthenticationToken> GetAuthenticationTokenAsync()
        {
            var accounts = await IdentityClient.GetAccountsAsync();

            AuthenticationResult result = null;
            bool tryInteractiveLogin = false;

            try
            {
                result = await IdentityClient
                    .AcquireTokenSilent(this.scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                tryInteractiveLogin = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MSAL Silent Error: {ex.Message}");
            }

            if (tryInteractiveLogin)
            {
                result = await IdentityClient
                    .AcquireTokenInteractive(this.scopes)
                    .ExecuteAsync();
            }

            return new AuthenticationToken
            {
                DisplayName = result?.Account?.Username ?? "",
                ExpiresOn = result?.ExpiresOn ?? DateTimeOffset.MinValue,
                AccessToken = result?.AccessToken ?? "",
                UserId = result?.Account?.Username ?? ""
            };
        }
    }

    public class AuthenticationToken
    {
        public string DisplayName { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public string AccessToken { get; set; }
        public string UserId { get; set; }
    }
}
