using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Services {
    public class AuthenticationService {
        private IConfiguration configuration;
        private string clientId;
        private string[] scopes;
        private string tenantId;

        public IPublicClientApplication IdentityClient { get; private set; }


        public AuthenticationService(IConfiguration configuration) {
            this.configuration = configuration;
            this.clientId = configuration["ClientId"];
            this.scopes = new[] { configuration["Scopes"] };
            this.tenantId = configuration["TenantId"];
        }

        public async Task<AuthenticationToken> GetAuthenticationTokenAsync() {
            if (IdentityClient == null) {
#if ANDROID
                IdentityClient = PublicClientApplicationBuilder
                    .Create(this.clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, this.tenantId)
                    .WithRedirectUri($"msal{this.clientId}://auth")
                    .WithParentActivityOrWindow(() => Platform.CurrentActivity)
                    .Build();
#elif IOS
                this.IdentityClient = PublicClientApplicationBuilder
                    .Create(this.clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, this.tenantId)
                    .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                    .WithRedirectUri($"msal{this.clientId}://auth")
                    .Build();
#else
                IdentityClient = PublicClientApplicationBuilder
                    .Create(this.clientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, this.tenantId)
                    .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                    .Build();
#endif
            }

            var accounts = await IdentityClient.GetAccountsAsync();
            AuthenticationResult result = null;
            bool tryInteractiveLogin = false;

            try {
                result = await IdentityClient
                    .AcquireTokenSilent(this.scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException) {
                tryInteractiveLogin = true;
            }
            catch (Exception ex) {
                Debug.WriteLine($"MSAL Silent Error: {ex.Message}");
            }

            if (tryInteractiveLogin) {
                try {
                    result = await IdentityClient
                        .AcquireTokenInteractive(this.scopes)
                        .ExecuteAsync();
                }
                catch (Exception ex) {
                    Debug.WriteLine($"MSAL Interactive Error: {ex.Message}");
                }
            }

            return new AuthenticationToken {
                DisplayName = result?.Account?.Username ?? "",
                ExpiresOn = result?.ExpiresOn ?? DateTimeOffset.MinValue,
                Token = result?.AccessToken ?? "",
                UserId = result?.Account?.Username ?? ""
            };
        }
    }

    public class AuthenticationToken {
        public string DisplayName { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
