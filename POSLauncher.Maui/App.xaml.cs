using POSLauncher.Maui.Views;

namespace POSLauncher.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}