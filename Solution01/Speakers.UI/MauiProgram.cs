﻿using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Speakers.UI.Services;
using Speakers.UI.ViewModels;
using Speakers.UI.Views;
using System.IO;

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
                    // SQLite file path & Sync endpoint
                    {"SqliteFilePath", Path.Combine(FileSystem.AppDataDirectory, "devdayblog.db")},
                    {"SyncEndpoint", "api/sync"},

                })
                .Build();

            builder.Configuration.AddConfiguration(config);

            builder
                .UseMauiApp<App>()
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
                c.Timeout = TimeSpan.FromSeconds(2);

            }).ConfigurePrimaryHttpMessageHandler(() => HttpsClientHandlerService.GetPlatformMessageHandler());

            builder.Services.AddDbContext<SpeakersContext>(options =>
                options.UseSqlite($"Filename={builder.Configuration["SqliteFilePath"]}"));

            builder.Services.AddSingleton<SpeakersViewModel>();
            builder.Services.AddSingleton<SpeakerEditViewModel>();
            builder.Services.AddSingleton<SpeakersPage>();
            builder.Services.AddSingleton<SpeakerEdit>();


            return builder.Build();
        }
    }
}