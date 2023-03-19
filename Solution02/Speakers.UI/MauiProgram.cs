using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Speakers.UI.Services;
using Speakers.UI.ViewModels;
using Speakers.UI.Views;
using System.IO;
using Microsoft.Identity.Client;

namespace Speakers.UI {
    public static class MauiProgram {
        public static MauiApp CreateMauiApp() {
            var builder = MauiApp.CreateBuilder();

            var config = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {

                            // Speaker API endpoints
                            {"SpeakerApi" , "https://localhost:7170"},
                            {"SpeakersEndpoint", "api/Speakers"},
                            // Azure AD Authentication
                            {"ClientId", "7c66175f-d9f2-47f2-90bf-bc7ce36ef91d" },
                            {"Scopes" , "api://2118f8b6-2847-490c-9983-2b1f0f6fc9ef/access_as_user" },
                            {"TenantId", "common" },
                            // SQLite file path
                            {"SqliteFilePath", Path.Combine(FileSystem.AppDataDirectory, "devdaybelog.db")},
                            // Sync endpoint
                            {"SyncEndpoint", "api/sync"},

                        })
                        .Build();

            builder.Configuration.AddConfiguration(config);

            builder
                .UseMauiApp<App>()
                .ConfigureLifecycleEvents(events => {
#if ANDROID
                    events.AddAndroid(platform => {
                        platform.OnActivityResult((activity, rc, result, data) => {
                            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(rc, result, data);
                        });
                    });
#endif
                })
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("ionicons.ttf", "ionicons");
                });


            builder.Services.AddHttpClient("api", c => {
                var apiUriString = builder.Configuration["SpeakerApi"];

                if (DeviceInfo.Platform == DevicePlatform.Android && apiUriString.Contains("localhost")) {
                    // Android specific uri to reach localhost
                    apiUriString = apiUriString.Replace("localhost", "10.0.2.2");

                    // Check if we are trying to reach a IIS Express.
                    // IIS Express does not allow any request other than localhost
                    // So far,hacking the Host-Content header to mimic localhost call
                    c.DefaultRequestHeaders.Host = $"localhost:{new Uri(apiUriString).Port}";
                }
                c.BaseAddress = new Uri(apiUriString);

            }).ConfigurePrimaryHttpMessageHandler(() => HttpsClientHandlerService.GetPlatformMessageHandler());

            builder.Services.AddDbContext<SpeakersContext>(options =>
                options.UseSqlite($"Filename={builder.Configuration["SqliteFilePath"]}"));

            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<SpeakersViewModel>();
            builder.Services.AddSingleton<SpeakerEditViewModel>();
            builder.Services.AddSingleton<SpeakersPage>();
            builder.Services.AddSingleton<SpeakerEdit>();


            return builder.Build();
        }
    }
}