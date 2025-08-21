using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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
                Console.WriteLine("Starting Commerce Client auto-discovery...");
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                var userDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                
                Console.WriteLine($"Checking common desktop: {desktopPath}");
                Console.WriteLine($"Checking user desktop: {userDesktopPath}");
                
                var shortcutPaths = new[]
                {
                    Path.Combine(desktopPath, "Commerce-Client.lnk"),
                    Path.Combine(userDesktopPath, "Commerce-Client.lnk"),
                    Path.Combine(desktopPath, "Commerce_Client.lnk"),
                    Path.Combine(userDesktopPath, "Commerce_Client.lnk"),
                    Path.Combine(desktopPath, "Commerce Client.lnk"),
                    Path.Combine(userDesktopPath, "Commerce Client.lnk")
                };

                foreach (var shortcutPath in shortcutPaths)
                {
                    Console.WriteLine($"Checking for shortcut: {shortcutPath}");
                    if (File.Exists(shortcutPath))
                    {
                        Console.WriteLine($"Found shortcut: {shortcutPath}");
                        var shortcutInfo = await ParseShortcut(shortcutPath);
                        if (shortcutInfo != null)
                        {
                            Console.WriteLine($"Successfully parsed shortcut - Target: {shortcutInfo.TargetPath}, Args: {shortcutInfo.Arguments}");
                            _configManager.UpdateCommerceClientSettings(shortcutInfo.TargetPath, shortcutInfo.Arguments);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Failed to parse shortcut");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Shortcut not found");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to find Commerce Client from desktop: {ex.Message}");
                Debug.WriteLine($"Failed to find Commerce_Client from desktop: {ex.Message}");
            }
        }

        private Task<ShortcutInfo> ParseShortcut(string shortcutPath)
        {
            try
            {
                Debug.WriteLine($"Attempting to parse shortcut: {shortcutPath}");
                Console.WriteLine($"Attempting to parse shortcut: {shortcutPath}");
                
                // Use simpler approach for now - just look for Commerce-Client executable
                // This is a fallback approach that works without complex COM interop
                var possiblePaths = new[]
                {
                    @"C:\Program Files\Commerce\Commerce-Client.exe",
                    @"C:\Program Files (x86)\Commerce\Commerce-Client.exe",
                    @"C:\Commerce\Commerce-Client.exe",
                    @"C:\Program Files\Commerce-Client\Commerce-Client.exe",
                    @"C:\Program Files (x86)\Commerce-Client\Commerce-Client.exe"
                };

                foreach (var path in possiblePaths)
                {
                    Console.WriteLine($"Checking possible path: {path}");
                    if (File.Exists(path))
                    {
                        Console.WriteLine($"Found Commerce-Client at: {path}");
                        return Task.FromResult(new ShortcutInfo
                        {
                            TargetPath = path,
                            Arguments = "" // Default empty arguments
                        });
                    }
                }
                
                Debug.WriteLine("Failed to find Commerce-Client executable");
                Console.WriteLine("Failed to find Commerce-Client executable in standard locations");
                return Task.FromResult<ShortcutInfo>(null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception parsing shortcut: {ex.Message}");
                Console.WriteLine($"Exception parsing shortcut: {ex.Message}");
                return Task.FromResult<ShortcutInfo>(null);
            }
        }

        private class ShortcutInfo
        {
            public string TargetPath { get; set; }
            public string Arguments { get; set; }
        }
    }
}