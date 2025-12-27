using Microsoft.Extensions.Logging;
using EmptySlot.Mobile.Services;
using EmptySlot.Mobile.Pages;
using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register pages and view models
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<BookingPage>();
        builder.Services.AddTransient<BookingViewModel>();
        builder.Services.AddTransient<AppointmentsPage>();
        builder.Services.AddTransient<AppointmentsViewModel>();
        builder.Services.AddTransient<MapPage>();
        builder.Services.AddTransient<MapViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
