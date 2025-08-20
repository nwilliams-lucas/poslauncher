namespace POSLauncher.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute("configuration", typeof(Views.ConfigurationPage));
    }
}