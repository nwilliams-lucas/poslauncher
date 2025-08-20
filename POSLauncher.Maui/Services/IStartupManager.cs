namespace POSLauncher.Maui.Services
{
    public interface IStartupManager
    {
        Task<bool> IsSetToStartupAsync();
        Task<bool> AddToStartupAsync();
        Task<bool> RemoveFromStartupAsync();
        Task EnsureStartupRegistrationAsync();
    }
}