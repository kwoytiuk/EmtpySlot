using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace EmptySlot.Mobile;

public partial class MauiWinUIApplicationImpl : MauiWinUIApplication
{
    public MauiWinUIApplicationImpl()
    {
        this.InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() =>
        MauiProgram.CreateMauiApp();
}