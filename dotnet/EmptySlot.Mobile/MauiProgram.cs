using Microsoft.Extensions.Logging;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;
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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Add Blazor Hybrid support
        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Register services
        builder.Services.AddSingleton<ICacheService, CacheService>();
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register pages and view models (still needed for legacy XAML pages if any)
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

        return builder.Build();
    }
}
