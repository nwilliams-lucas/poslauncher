namespace POSLauncher.Maui.Services
{
    public interface IApplicationLauncher
    {
        Task<bool> LaunchCommerceClientAsync();
        Task<bool> FindCommerceClientFromDesktopAsync();
    }
}