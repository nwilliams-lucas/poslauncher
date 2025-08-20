using POSLauncher.Maui.Services;
using System.Windows.Input;

namespace POSLauncher.Maui.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IServiceManager _serviceManager;
        private readonly IApplicationLauncher _applicationLauncher;
        private readonly IStartupManager _startupManager;
        private readonly IConfigurationService _configService;

        private string _statusMessage = "Initializing...";
        private string _postgresStatus = "PostgreSQL Database: Checking...";
        private string _fixedServiceStatus = "JMC FIXED Service: Checking...";
        private string _commerceClientStatus = "Commerce_Client: Waiting for services...";
        private string _startupStatus = "Startup Registration: Checking...";
        private bool _isRetryEnabled;
        private bool _isLaunching;

        public MainPageViewModel(
            IServiceManager serviceManager,
            IApplicationLauncher applicationLauncher,
            IStartupManager startupManager,
            IConfigurationService configService)
        {
            _serviceManager = serviceManager;
            _applicationLauncher = applicationLauncher;
            _startupManager = startupManager;
            _configService = configService;

            RetryCommand = new Command(async () => await StartLaunchSequenceAsync(), () => _isRetryEnabled);
            ConfigureCommand = new Command(async () => await Shell.Current.GoToAsync("configuration"));
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string PostgresStatus
        {
            get => _postgresStatus;
            set => SetProperty(ref _postgresStatus, value);
        }

        public string FixedServiceStatus
        {
            get => _fixedServiceStatus;
            set => SetProperty(ref _fixedServiceStatus, value);
        }

        public string CommerceClientStatus
        {
            get => _commerceClientStatus;
            set => SetProperty(ref _commerceClientStatus, value);
        }

        public string StartupStatus
        {
            get => _startupStatus;
            set => SetProperty(ref _startupStatus, value);
        }

        public bool IsRetryEnabled
        {
            get => _isRetryEnabled;
            set
            {
                if (SetProperty(ref _isRetryEnabled, value))
                {
                    ((Command)RetryCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsLaunching
        {
            get => _isLaunching;
            set => SetProperty(ref _isLaunching, value);
        }

        public ICommand RetryCommand { get; }
        public ICommand ConfigureCommand { get; }

        public async Task InitializeAsync()
        {
            await StartLaunchSequenceAsync();
        }

        private async Task StartLaunchSequenceAsync()
        {
            StatusMessage = "Starting launch sequence...";
            IsRetryEnabled = false;
            IsLaunching = true;

            try
            {
                await CheckAndConfigureStartupAsync();
                await CheckAndStartPostgreSQLAsync();
                await CheckAndStartFixedServiceAsync();
                await LaunchCommerceClientAsync();

                StatusMessage = "Launch sequence completed successfully!";
                IsRetryEnabled = true;

                // Auto-minimize after 5 seconds on successful completion
                var config = await _configService.GetConfigurationAsync();
                if (config.MinimizeOnComplete)
                {
                    await Task.Delay(5000);
                    // Note: Minimization would need platform-specific implementation
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Launch sequence failed: {ex.Message}";
                IsRetryEnabled = true;
            }
            finally
            {
                IsLaunching = false;
            }
        }

        private async Task CheckAndConfigureStartupAsync()
        {
            StatusMessage = "Configuring startup registration...";

            try
            {
                var isRegistered = await _startupManager.IsSetToStartupAsync();
                if (isRegistered)
                {
                    StartupStatus = "Startup Registration: Already configured ✓";
                }
                else
                {
                    await _startupManager.AddToStartupAsync();
                    var success = await _startupManager.IsSetToStartupAsync();
                    StartupStatus = success 
                        ? "Startup Registration: Successfully configured ✓"
                        : "Startup Registration: Failed to configure ✗";
                }
            }
            catch (Exception ex)
            {
                StartupStatus = $"Startup Registration: Error - {ex.Message} ✗";
            }
        }

        private async Task CheckAndStartPostgreSQLAsync()
        {
            StatusMessage = "Checking PostgreSQL service...";
            PostgresStatus = "PostgreSQL Database: Checking status...";

            var status = await _serviceManager.CheckPostgreSQLServiceAsync();

            if (status.IsRunning)
            {
                PostgresStatus = "PostgreSQL Database: Running ✓";
                return;
            }

            PostgresStatus = "PostgreSQL Database: Starting...";
            StatusMessage = "Starting PostgreSQL service...";

            var started = await _serviceManager.StartPostgreSQLServiceAsync();

            if (started)
            {
                PostgresStatus = "PostgreSQL Database: Successfully started ✓";
            }
            else
            {
                var errorMsg = string.IsNullOrEmpty(status.ErrorMessage)
                    ? "Failed to start"
                    : status.ErrorMessage;
                PostgresStatus = $"PostgreSQL Database: {errorMsg} ✗";
                throw new Exception($"Failed to start PostgreSQL: {errorMsg}");
            }
        }

        private async Task CheckAndStartFixedServiceAsync()
        {
            StatusMessage = "Checking JMC FIXED service...";
            FixedServiceStatus = "JMC FIXED Service: Checking status...";

            var status = await _serviceManager.CheckFixedServiceAsync();

            if (status.IsRunning)
            {
                FixedServiceStatus = "JMC FIXED Service: Running ✓";
                return;
            }

            FixedServiceStatus = "JMC FIXED Service: Starting...";
            StatusMessage = "Starting JMC FIXED service...";

            var started = await _serviceManager.StartFixedServiceAsync();

            if (started)
            {
                FixedServiceStatus = "JMC FIXED Service: Successfully started ✓";
            }
            else
            {
                var errorMsg = string.IsNullOrEmpty(status.ErrorMessage)
                    ? "Failed to start"
                    : status.ErrorMessage;
                FixedServiceStatus = $"JMC FIXED Service: {errorMsg} ✗";
                throw new Exception($"Failed to start JMC FIXED service: {errorMsg}");
            }
        }

        private async Task LaunchCommerceClientAsync()
        {
            StatusMessage = "Launching Commerce_Client application...";
            CommerceClientStatus = "Commerce_Client: Launching...";

            var launched = await _applicationLauncher.LaunchCommerceClientAsync();

            if (launched)
            {
                CommerceClientStatus = "Commerce_Client: Successfully launched ✓";
            }
            else
            {
                CommerceClientStatus = "Commerce_Client: Failed to launch - check configuration ✗";
                throw new Exception("Failed to launch Commerce_Client application");
            }
        }
    }
}