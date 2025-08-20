using POSLauncher.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace POSLauncher
{
    public partial class MainWindow : Window
    {
        private readonly ServiceManager _serviceManager;
        private readonly CommerceClientLauncher _commerceLauncher;
        private readonly ConfigurationManager _configManager;
        private readonly StartupManager _startupManager;

        public MainWindow()
        {
            InitializeComponent();
            
            _configManager = new ConfigurationManager();
            _serviceManager = new ServiceManager();
            _commerceLauncher = new CommerceClientLauncher(_configManager);
            _startupManager = new StartupManager();
            
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await StartLaunchSequence();
        }

        private async Task StartLaunchSequence()
        {
            UpdateStatus("Starting launch sequence...");
            RetryButton.IsEnabled = false;

            try
            {
                await CheckAndConfigureStartup();
                await CheckAndStartPostgreSQL();
                await CheckAndStartFixedService();
                await LaunchCommerceClient();
                
                UpdateStatus("Launch sequence completed successfully!");
                RetryButton.IsEnabled = true;
                
                await Task.Delay(5000);
                if (IsVisible)
                {
                    WindowState = WindowState.Minimized;
                    ShowInTaskbar = false;
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Launch sequence failed: {ex.Message}");
                RetryButton.IsEnabled = true;
            }
        }

        private async Task CheckAndConfigureStartup()
        {
            UpdateStatus("Configuring startup registration...");
            
            try
            {
                var isRegistered = _startupManager.IsSetToStartup();
                if (isRegistered)
                {
                    UpdateStatusBlock(StartupStatus, "Startup Registration: Already configured", true);
                }
                else
                {
                    _startupManager.AddToStartup();
                    var success = _startupManager.IsSetToStartup();
                    UpdateStatusBlock(StartupStatus, 
                        success ? "Startup Registration: Successfully configured" : "Startup Registration: Failed to configure",
                        success);
                }
            }
            catch (Exception ex)
            {
                UpdateStatusBlock(StartupStatus, $"Startup Registration: Error - {ex.Message}", false);
            }
        }

        private async Task CheckAndStartPostgreSQL()
        {
            UpdateStatus("Checking PostgreSQL service...");
            UpdateStatusBlock(PostgreSQLStatus, "PostgreSQL Database: Checking status...", null);

            var status = await _serviceManager.CheckPostgreSQLService();
            
            if (status.IsRunning)
            {
                UpdateStatusBlock(PostgreSQLStatus, "PostgreSQL Database: Running", true);
                return;
            }

            UpdateStatusBlock(PostgreSQLStatus, "PostgreSQL Database: Starting...", null);
            UpdateStatus("Starting PostgreSQL service...");

            var started = await _serviceManager.StartPostgreSQLService();
            
            if (started)
            {
                UpdateStatusBlock(PostgreSQLStatus, "PostgreSQL Database: Successfully started", true);
            }
            else
            {
                var errorMsg = string.IsNullOrEmpty(status.ErrorMessage) 
                    ? "Failed to start" 
                    : status.ErrorMessage;
                UpdateStatusBlock(PostgreSQLStatus, $"PostgreSQL Database: {errorMsg}", false);
                throw new Exception($"Failed to start PostgreSQL: {errorMsg}");
            }
        }

        private async Task CheckAndStartFixedService()
        {
            UpdateStatus("Checking JMC FIXED service...");
            UpdateStatusBlock(FixedServiceStatus, "JMC FIXED Service: Checking status...", null);

            var status = await _serviceManager.CheckFixedService();
            
            if (status.IsRunning)
            {
                UpdateStatusBlock(FixedServiceStatus, "JMC FIXED Service: Running", true);
                return;
            }

            UpdateStatusBlock(FixedServiceStatus, "JMC FIXED Service: Starting...", null);
            UpdateStatus("Starting JMC FIXED service...");

            var started = await _serviceManager.StartFixedService();
            
            if (started)
            {
                UpdateStatusBlock(FixedServiceStatus, "JMC FIXED Service: Successfully started", true);
            }
            else
            {
                var errorMsg = string.IsNullOrEmpty(status.ErrorMessage) 
                    ? "Failed to start" 
                    : status.ErrorMessage;
                UpdateStatusBlock(FixedServiceStatus, $"JMC FIXED Service: {errorMsg}", false);
                throw new Exception($"Failed to start JMC FIXED service: {errorMsg}");
            }
        }

        private async Task LaunchCommerceClient()
        {
            UpdateStatus("Launching Commerce_Client application...");
            UpdateStatusBlock(CommerceClientStatus, "Commerce_Client: Launching...", null);

            var launched = await _commerceLauncher.LaunchCommerceClient();
            
            if (launched)
            {
                UpdateStatusBlock(CommerceClientStatus, "Commerce_Client: Successfully launched", true);
            }
            else
            {
                UpdateStatusBlock(CommerceClientStatus, "Commerce_Client: Failed to launch - check configuration", false);
                throw new Exception("Failed to launch Commerce_Client application");
            }
        }

        private void UpdateStatus(string message)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = message;
            });
        }

        private void UpdateStatusBlock(System.Windows.Controls.TextBlock textBlock, string message, bool? success)
        {
            Dispatcher.Invoke(() =>
            {
                textBlock.Text = message;
                
                if (success.HasValue)
                {
                    textBlock.Style = success.Value 
                        ? (Style)FindResource("SuccessStyle") 
                        : (Style)FindResource("ErrorStyle");
                }
                else
                {
                    textBlock.Style = (Style)FindResource("InProgressStyle");
                }
            });
        }

        private async void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            await StartLaunchSequence();
        }

        private void ConfigureButton_Click(object sender, RoutedEventArgs e)
        {
            var configWindow = new ConfigurationWindow(_configManager);
            configWindow.Owner = this;
            configWindow.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}