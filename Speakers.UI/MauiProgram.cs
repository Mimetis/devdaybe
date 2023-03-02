using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Speakers.UI.Services;
using Speakers.UI.ViewModels;
using Speakers.UI.Views;

namespace Speakers.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //#if DEBUG
            //		builder.Logging.AddDebug();
            //#endif

            //builder.Services.AddHttpClient("api", c =>
            //{
            //    c.BaseAddress = new Uri("https://localhost:7170");
            //});

            //builder.Services.AddHttpClient("localhost_android", c =>
            //{
            //    c.BaseAddress = new Uri("https://10.0.2.2:7170");

            //    // Check if we are trying to reach a IIS Express.
            //    // IIS Express does not allow any request other than localhost
            //    // So far,hacking the Host-Content header to mimic localhost call

            //    c.DefaultRequestHeaders.Host = $"localhost:7170";

            //}).ConfigurePrimaryHttpMessageHandler(() =>
            //{
            //    HttpsClientHandlerService handler = new();
            //    return handler.GetPlatformMessageHandler();
            //});

            builder.Services.AddHttpClient("localhost_android", c =>
            {
                c.BaseAddress = new Uri("https://devdaybe.servicebus.windows.net/localhost-https");

                //c.DefaultRequestHeaders.Host = $"localhost:7170";
            });

            builder.Services.AddSingleton<SpeakersViewModel>();
            builder.Services.AddTransient<SpeakersPage>();

            builder.Services.AddSingleton<ISettingServices, SettingServices>();

            return builder.Build();
        }
    }
}