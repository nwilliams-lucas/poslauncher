using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace POSLauncher.Services
{
    public class StartupManager
    {
        private const string StartupKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string ApplicationName = "POSLauncher";

        public bool IsSetToStartup()
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

        public bool AddToStartup()
        {
            try
            {
                var executablePath = Path.Combine(AppContext.BaseDirectory, "POSLauncher.exe");

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

        public bool RemoveFromStartup()
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

        public void EnsureStartupRegistration()
        {
            if (!IsSetToStartup())
            {
                AddToStartup();
            }
        }
    }
}