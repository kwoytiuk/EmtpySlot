using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace EmptySlot.Mobile;

public class MauiWinUIApplicationImpl : MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() =>
        MauiProgram.CreateMauiApp();
}