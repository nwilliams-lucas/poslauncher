using System;
using System.IO;
using System.Text.Json;

namespace POSLauncher.Services
{
    public class ConfigurationManager
    {
        private readonly string _configPath;
        private Configuration _configuration;

        public ConfigurationManager()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var configDir = Path.Combine(appDataPath, "POSLauncher");
            Directory.CreateDirectory(configDir);
            _configPath = Path.Combine(configDir, "config.json");
            
            LoadConfiguration();
        }

        public Configuration GetConfiguration()
        {
            return _configuration ?? new Configuration();
        }

        public void UpdateCommerceClientSettings(string path, string arguments)
        {
            _configuration.CommerceClientPath = path;
            _configuration.CommerceClientArguments = arguments;
            SaveConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var json = File.ReadAllText(_configPath);
                    _configuration = JsonSerializer.Deserialize<Configuration>(json) ?? new Configuration();
                }
                else
                {
                    _configuration = new Configuration();
                    SaveConfiguration();
                }
            }
            catch
            {
                _configuration = new Configuration();
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                var json = JsonSerializer.Serialize(_configuration, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save configuration: {ex.Message}");
            }
        }
    }

    public class Configuration
    {
        public string CommerceClientPath { get; set; } = string.Empty;
        public string CommerceClientArguments { get; set; } = string.Empty;
        public bool AutoStartOnBoot { get; set; } = true;
        public int ServiceStartTimeoutSeconds { get; set; } = 30;
    }
}