#if WINDOWS
using System.ServiceProcess;

namespace POSLauncher.Maui.Services
{
    public class WindowsServiceManager : IServiceManager
    {
        public async Task<ServiceStatus> CheckServiceStatusAsync(string serviceName)
        {
            try
            {
                using (var service = new ServiceController(serviceName))
                {
                    service.Refresh();
                    return new ServiceStatus
                    {
                        Name = serviceName,
                        Status = ConvertServiceControllerStatus(service.Status),
                        IsRunning = service.Status == ServiceControllerStatus.Running
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceStatus
                {
                    Name = serviceName,
                    Status = ServiceState.Error,
                    IsRunning = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<bool> StartServiceAsync(string serviceName)
        {
            try
            {
                using (var service = new ServiceController(serviceName))
                {
                    service.Refresh();
                    
                    if (service.Status == ServiceControllerStatus.Running)
                        return true;

                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        return service.Status == ServiceControllerStatus.Running;
                    }
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to start service {serviceName}: {ex.Message}");
                return false;
            }
        }

        public async Task<ServiceStatus> CheckPostgreSQLServiceAsync()
        {
            var commonNames = new[] { "postgresql-x64-16", "postgresql-x64-15", "postgresql-x64-14", "postgresql", "PostgreSQL" };
            
            foreach (var name in commonNames)
            {
                var status = await CheckServiceStatusAsync(name);
                if (string.IsNullOrEmpty(status.ErrorMessage))
                {
                    status.DisplayName = "PostgreSQL Database Server";
                    return status;
                }
            }
            
            return new ServiceStatus
            {
                Name = "PostgreSQL",
                DisplayName = "PostgreSQL Database Server",
                Status = ServiceState.Error,
                IsRunning = false,
                ErrorMessage = "PostgreSQL service not found"
            };
        }

        public async Task<bool> StartPostgreSQLServiceAsync()
        {
            var commonNames = new[] { "postgresql-x64-16", "postgresql-x64-15", "postgresql-x64-14", "postgresql", "PostgreSQL" };
            
            foreach (var name in commonNames)
            {
                try
                {
                    if (await StartServiceAsync(name))
                        return true;
                }
                catch
                {
                    continue;
                }
            }
            
            return false;
        }

        public async Task<ServiceStatus> CheckFixedServiceAsync()
        {
            var status = await CheckServiceStatusAsync("fixed");
            status.DisplayName = "JMC FIXED";
            return status;
        }

        public async Task<bool> StartFixedServiceAsync()
        {
            return await StartServiceAsync("fixed");
        }

        private ServiceState ConvertServiceControllerStatus(ServiceControllerStatus status)
        {
            return status switch
            {
                ServiceControllerStatus.Stopped => ServiceState.Stopped,
                ServiceControllerStatus.Running => ServiceState.Running,
                ServiceControllerStatus.StartPending => ServiceState.Starting,
                ServiceControllerStatus.StopPending => ServiceState.Stopping,
                _ => ServiceState.Unknown
            };
        }
    }
}
#endif