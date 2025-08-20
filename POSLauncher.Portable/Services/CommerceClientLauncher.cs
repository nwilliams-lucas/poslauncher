using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace POSLauncher.Services
{
    public class CommerceClientLauncher
    {
        private readonly ConfigurationManager _configManager;

        public CommerceClientLauncher(ConfigurationManager configManager)
        {
            _configManager = configManager;
        }

        public async Task<bool> LaunchCommerceClient()
        {
            try
            {
                var config = _configManager.GetConfiguration();
                
                if (string.IsNullOrEmpty(config.CommerceClientPath) || !File.Exists(config.CommerceClientPath))
                {
                    await TryFindCommerceClientFromDesktop();
                    config = _configManager.GetConfiguration();
                }

                if (string.IsNullOrEmpty(config.CommerceClientPath) || !File.Exists(config.CommerceClientPath))
                {
                    return false;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = config.CommerceClientPath,
                    Arguments = config.CommerceClientArguments,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                var process = Process.Start(startInfo);
                return process != null && !process.HasExited;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to launch Commerce_Client: {ex.Message}");
                return false;
            }
        }

        private async Task TryFindCommerceClientFromDesktop()
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                var userDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                
                var shortcutPaths = new[]
                {
                    Path.Combine(desktopPath, "Commerce_Client.lnk"),
                    Path.Combine(userDesktopPath, "Commerce_Client.lnk"),
                    Path.Combine(desktopPath, "Commerce Client.lnk"),
                    Path.Combine(userDesktopPath, "Commerce Client.lnk")
                };

                foreach (var shortcutPath in shortcutPaths)
                {
                    if (File.Exists(shortcutPath))
                    {
                        var shortcutInfo = await ParseShortcut(shortcutPath);
                        if (shortcutInfo != null)
                        {
                            _configManager.UpdateCommerceClientSettings(shortcutInfo.TargetPath, shortcutInfo.Arguments);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to find Commerce_Client from desktop: {ex.Message}");
            }
        }

        private async Task<ShortcutInfo> ParseShortcut(string shortcutPath)
        {
            try
            {
                // Simplified approach without COM - will be handled properly when running on Windows
                // This build is for testing the UI and basic functionality
                Debug.WriteLine($"Shortcut parsing disabled in portable build: {shortcutPath}");
                return null;
            }
            catch
            {
                return null;
            }
        }

        private class ShortcutInfo
        {
            public string TargetPath { get; set; }
            public string Arguments { get; set; }
        }
    }
}