using Microsoft.Extensions.Logging;
using POSLauncher.Maui.Services;
using POSLauncher.Maui.Views;
using POSLauncher.Maui.ViewModels;

namespace POSLauncher.Maui;

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

        // Register services
        builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();
        
#if WINDOWS
        builder.Services.AddSingleton<IServiceManager, WindowsServiceManager>();
        builder.Services.AddSingleton<IStartupManager, WindowsStartupManager>();
        builder.Services.AddSingleton<IApplicationLauncher, WindowsApplicationLauncher>();
#elif ANDROID
        builder.Services.AddSingleton<IServiceManager, AndroidServiceManager>();
        builder.Services.AddSingleton<IStartupManager, AndroidStartupManager>();
        builder.Services.AddSingleton<IApplicationLauncher, AndroidApplicationLauncher>();
#elif IOS
        builder.Services.AddSingleton<IServiceManager, IOSServiceManager>();
        builder.Services.AddSingleton<IStartupManager, IOSStartupManager>();
        builder.Services.AddSingleton<IApplicationLauncher, IOSApplicationLauncher>();
#elif MACCATALYST
        builder.Services.AddSingleton<IServiceManager, MacServiceManager>();
        builder.Services.AddSingleton<IStartupManager, MacStartupManager>();
        builder.Services.AddSingleton<IApplicationLauncher, MacApplicationLauncher>();
#endif

        // Register ViewModels
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<ConfigurationPageViewModel>();

        // Register Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<ConfigurationPage>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
        });
#endif

        return builder.Build();
    }
}