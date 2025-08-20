#if WINDOWS
using Microsoft.Win32;
using System.Reflection;

namespace POSLauncher.Maui.Services
{
    public class WindowsStartupManager : IStartupManager
    {
        private const string StartupKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ApplicationName = "POSLauncher";

        public async Task<bool> IsSetToStartupAsync()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(StartupKeyPath, false))
                {
                    return key?.GetValue(ApplicationName) != null;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddToStartupAsync()
        {
            try
            {
                var executablePath = Assembly.GetExecutingAssembly().Location;
                if (executablePath.EndsWith(".dll"))
                {
                    executablePath = executablePath.Replace(".dll", ".exe");
                }

                using (var key = Registry.LocalMachine.OpenSubKey(StartupKeyPath, true))
                {
                    key?.SetValue(ApplicationName, $"\"{executablePath}\"");
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to add to startup: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFromStartupAsync()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(StartupKeyPath, true))
                {
                    key?.DeleteValue(ApplicationName, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to remove from startup: {ex.Message}");
                return false;
            }
        }

        public async Task EnsureStartupRegistrationAsync()
        {
            if (!await IsSetToStartupAsync())
            {
                await AddToStartupAsync();
            }
        }
    }
}
#endif