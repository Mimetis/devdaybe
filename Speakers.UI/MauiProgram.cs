using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Speakers.UI.Services;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<ISettingServices, SettingServices>();
            builder.Services.AddSingleton<ISettingServices, SettingServices>();

            return builder.Build();
        }
    }
}