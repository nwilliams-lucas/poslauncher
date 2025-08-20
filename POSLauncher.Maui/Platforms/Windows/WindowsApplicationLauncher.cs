#if WINDOWS
using System.Diagnostics;
using POSLauncher.Maui.Services;

namespace POSLauncher.Maui.Services
{
    public class WindowsApplicationLauncher : IApplicationLauncher
    {
        private readonly IConfigurationService _configService;

        public WindowsApplicationLauncher(IConfigurationService configService)
        {
            _configService = configService;
        }

        public async Task<bool> LaunchCommerceClientAsync()
        {
            try
            {
                var config = await _configService.GetConfigurationAsync();
                
                if (string.IsNullOrEmpty(config.CommerceClientPath) || !File.Exists(config.CommerceClientPath))
                {
                    await FindCommerceClientFromDesktopAsync();
                    config = await _configService.GetConfigurationAsync();
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
                System.Diagnostics.Debug.WriteLine($"Failed to launch Commerce_Client: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> FindCommerceClientFromDesktopAsync()
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
                        var shortcutInfo = await ParseShortcutAsync(shortcutPath);
                        if (shortcutInfo != null)
                        {
                            await _configService.UpdateCommerceClientSettingsAsync(shortcutInfo.TargetPath, shortcutInfo.Arguments);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to find Commerce_Client from desktop: {ex.Message}");
                return false;
            }
        }

        private async Task<ShortcutInfo?> ParseShortcutAsync(string shortcutPath)
        {
            try
            {
                // For simplicity, we'll use a basic approach to parse shortcuts
                // In a production environment, you might want to use a more robust library
                var shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
                var shortcut = shell?.GetType().InvokeMember("CreateShortcut", 
                    System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { shortcutPath });
                
                if (shortcut != null)
                {
                    var targetPath = shortcut.GetType().InvokeMember("TargetPath", 
                        System.Reflection.BindingFlags.GetProperty, null, shortcut, null)?.ToString();
                    var arguments = shortcut.GetType().InvokeMember("Arguments", 
                        System.Reflection.BindingFlags.GetProperty, null, shortcut, null)?.ToString();
                    
                    return new ShortcutInfo
                    {
                        TargetPath = targetPath ?? string.Empty,
                        Arguments = arguments ?? string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to parse shortcut: {ex.Message}");
            }
            return null;
        }

        private class ShortcutInfo
        {
            public string TargetPath { get; set; } = string.Empty;
            public string Arguments { get; set; } = string.Empty;
        }
    }
}
#endif