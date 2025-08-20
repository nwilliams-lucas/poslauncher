using POSLauncher.Maui.Services;
using System.Windows.Input;

namespace POSLauncher.Maui.ViewModels
{
    public class ConfigurationPageViewModel : BaseViewModel
    {
        private readonly IConfigurationService _configService;
        private Configuration _configuration;

        private string _clientPath = string.Empty;
        private string _arguments = string.Empty;
        private bool _autoStartOnBoot = true;
        private int _serviceTimeout = 30;
        private bool _minimizeOnComplete = true;
        private bool _showStatusUpdates = true;

        public ConfigurationPageViewModel(IConfigurationService configService)
        {
            _configService = configService;
            _configuration = new Configuration();

            SaveCommand = new Command(async () => await SaveConfigurationAsync());
            BrowseCommand = new Command(async () => await BrowseForApplicationAsync());
        }

        public string ClientPath
        {
            get => _clientPath;
            set => SetProperty(ref _clientPath, value);
        }

        public string Arguments
        {
            get => _arguments;
            set => SetProperty(ref _arguments, value);
        }

        public bool AutoStartOnBoot
        {
            get => _autoStartOnBoot;
            set => SetProperty(ref _autoStartOnBoot, value);
        }

        public int ServiceTimeout
        {
            get => _serviceTimeout;
            set => SetProperty(ref _serviceTimeout, value);
        }

        public bool MinimizeOnComplete
        {
            get => _minimizeOnComplete;
            set => SetProperty(ref _minimizeOnComplete, value);
        }

        public bool ShowStatusUpdates
        {
            get => _showStatusUpdates;
            set => SetProperty(ref _showStatusUpdates, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand BrowseCommand { get; }

        public async Task InitializeAsync()
        {
            _configuration = await _configService.GetConfigurationAsync();
            
            ClientPath = _configuration.CommerceClientPath;
            Arguments = _configuration.CommerceClientArguments;
            AutoStartOnBoot = _configuration.AutoStartOnBoot;
            ServiceTimeout = _configuration.ServiceStartTimeoutSeconds;
            MinimizeOnComplete = _configuration.MinimizeOnComplete;
            ShowStatusUpdates = _configuration.ShowStatusUpdates;
        }

        private async Task SaveConfigurationAsync()
        {
            _configuration.CommerceClientPath = ClientPath;
            _configuration.CommerceClientArguments = Arguments;
            _configuration.AutoStartOnBoot = AutoStartOnBoot;
            _configuration.ServiceStartTimeoutSeconds = ServiceTimeout;
            _configuration.MinimizeOnComplete = MinimizeOnComplete;
            _configuration.ShowStatusUpdates = ShowStatusUpdates;

            await _configService.SaveConfigurationAsync(_configuration);
            
            await Application.Current.MainPage.DisplayAlert("Success", "Configuration saved successfully!", "OK");
            await Shell.Current.GoToAsync("..");
        }

        private async Task BrowseForApplicationAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Commerce_Client Application",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".exe" } },
                        { DevicePlatform.macOS, new[] { ".app" } },
                    })
                });

                if (result != null)
                {
                    ClientPath = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to browse for file: {ex.Message}", "OK");
            }
        }
    }
}