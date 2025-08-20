#if IOS
using POSLauncher.Maui.Services;

namespace POSLauncher.Maui.Services
{
    public class IOSServiceManager : IServiceManager
    {
        public async Task<ServiceStatus> CheckServiceStatusAsync(string serviceName)
        {
            return new ServiceStatus
            {
                Name = serviceName,
                Status = ServiceState.Error,
                IsRunning = false,
                ErrorMessage = "Service management not supported on iOS"
            };
        }

        public async Task<bool> StartServiceAsync(string serviceName) => false;
        public async Task<ServiceStatus> CheckPostgreSQLServiceAsync()
        {
            return new ServiceStatus
            {
                Name = "PostgreSQL",
                DisplayName = "PostgreSQL Database Server",
                Status = ServiceState.Error,
                IsRunning = false,
                ErrorMessage = "PostgreSQL service management not supported on iOS"
            };
        }
        public async Task<bool> StartPostgreSQLServiceAsync() => false;
        public async Task<ServiceStatus> CheckFixedServiceAsync()
        {
            return new ServiceStatus
            {
                Name = "fixed",
                DisplayName = "JMC FIXED",
                Status = ServiceState.Error,
                IsRunning = false,
                ErrorMessage = "Service management not supported on iOS"
            };
        }
        public async Task<bool> StartFixedServiceAsync() => false;
    }

    public class IOSStartupManager : IStartupManager
    {
        public async Task<bool> IsSetToStartupAsync() => false;
        public async Task<bool> AddToStartupAsync() => false;
        public async Task<bool> RemoveFromStartupAsync() => false;
        public async Task EnsureStartupRegistrationAsync() { }
    }

    public class IOSApplicationLauncher : IApplicationLauncher
    {
        public async Task<bool> LaunchCommerceClientAsync() => false;
        public async Task<bool> FindCommerceClientFromDesktopAsync() => false;
    }
}
#endif