using System.Text.Json;

namespace POSLauncher.Maui.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configPath;
        private Configuration? _configuration;

        public ConfigurationService()
        {
            var appDataPath = FileSystem.Current.AppDataDirectory;
            _configPath = Path.Combine(appDataPath, "config.json");
        }

        public async Task<Configuration> GetConfigurationAsync()
        {
            if (_configuration != null)
                return _configuration;

            try
            {
                if (File.Exists(_configPath))
                {
                    var json = await File.ReadAllTextAsync(_configPath);
                    _configuration = JsonSerializer.Deserialize<Configuration>(json) ?? new Configuration();
                }
                else
                {
                    _configuration = new Configuration();
                    await SaveConfigurationAsync(_configuration);
                }
            }
            catch
            {
                _configuration = new Configuration();
            }

            return _configuration;
        }

        public async Task UpdateCommerceClientSettingsAsync(string path, string arguments)
        {
            var config = await GetConfigurationAsync();
            config.CommerceClientPath = path;
            config.CommerceClientArguments = arguments;
            await SaveConfigurationAsync(config);
        }

        public async Task SaveConfigurationAsync(Configuration configuration)
        {
            try
            {
                _configuration = configuration;
                var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                await File.WriteAllTextAsync(_configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save configuration: {ex.Message}");
            }
        }
    }
}