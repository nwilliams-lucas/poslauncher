using System;
using System.Windows;

namespace POSLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Console.WriteLine("Application starting...");
                base.OnStartup(e);
                
                Console.WriteLine("Creating MainWindow...");
                var mainWindow = new MainWindow();
                
                Console.WriteLine("Showing MainWindow...");
                mainWindow.Show();
                
                Console.WriteLine("MainWindow shown successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during startup: {ex}");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}