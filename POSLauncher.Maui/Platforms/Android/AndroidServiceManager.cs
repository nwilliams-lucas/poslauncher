#if ANDROID
using POSLauncher.Maui.Services;

namespace POSLauncher.Maui.Services
{
    public class AndroidServiceManager : IServiceManager
    {
        public async Task<ServiceStatus> CheckServiceStatusAsync(string serviceName)
        {
            // Android doesn't have Windows services, so we'll return a not supported status
            return new ServiceStatus
            {
                Name = serviceName,
                Status = ServiceState.Error,
                IsRunning = false,
                ErrorMessage = "Service management not supported on Android"
            };
        }

        public async Task<bool> StartServiceAsync(string serviceName)
        {
            return false; // Not supported on Android
        }

        public async Task<ServiceStatus> CheckPostgreSQLServiceAsync()
        {
            return new ServiceStatus
            {
                Name = "PostgreSQL",
                DisplayName = "PostgreSQL Database Server",
                Status = ServiceState.Error,
                IsRunning = false,
                ErrorMessage = "PostgreSQL service management not supported on Android"
            };
        }

        public async Task<bool> StartPostgreSQLServiceAsync()
        {
            return false;
        }

        public async Task<ServiceStatus> CheckFixedServiceAsync()
        {
            return new ServiceStatus
            {
                Name = "fixed",
                DisplayName = "JMC FIXED",
                Status = ServiceState.Error,
                IsRunning = false,
                ErrorMessage = "Service management not supported on Android"
            };
        }

        public async Task<bool> StartFixedServiceAsync()
        {
            return false;
        }
    }

    public class AndroidStartupManager : IStartupManager
    {
        public async Task<bool> IsSetToStartupAsync() => false;
        public async Task<bool> AddToStartupAsync() => false;
        public async Task<bool> RemoveFromStartupAsync() => false;
        public async Task EnsureStartupRegistrationAsync() { }
    }

    public class AndroidApplicationLauncher : IApplicationLauncher
    {
        public async Task<bool> LaunchCommerceClientAsync() => false;
        public async Task<bool> FindCommerceClientFromDesktopAsync() => false;
    }
}
#endif