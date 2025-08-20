namespace POSLauncher.Maui.Services
{
    public interface IConfigurationService
    {
        Task<Configuration> GetConfigurationAsync();
        Task UpdateCommerceClientSettingsAsync(string path, string arguments);
        Task SaveConfigurationAsync(Configuration configuration);
    }

    public class Configuration
    {
        public string CommerceClientPath { get; set; } = string.Empty;
        public string CommerceClientArguments { get; set; } = string.Empty;
        public bool AutoStartOnBoot { get; set; } = true;
        public int ServiceStartTimeoutSeconds { get; set; } = 30;
        public bool MinimizeOnComplete { get; set; } = true;
        public bool ShowStatusUpdates { get; set; } = true;
    }
}