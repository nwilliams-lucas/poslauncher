using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace POSLauncher.Services
{
    public class ServiceManager
    {
        public Task<ServiceStatus> CheckServiceStatus(string serviceName)
        {
            try
            {
                using (var service = new ServiceController(serviceName))
                {
                    service.Refresh();
                    return Task.FromResult(new ServiceStatus
                    {
                        Name = serviceName,
                        Status = service.Status,
                        IsRunning = service.Status == ServiceControllerStatus.Running
                    });
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ServiceStatus
                {
                    Name = serviceName,
                    Status = ServiceControllerStatus.Stopped,
                    IsRunning = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        public Task<bool> StartService(string serviceName)
        {
            try
            {
                using (var service = new ServiceController(serviceName))
                {
                    service.Refresh();
                    
                    if (service.Status == ServiceControllerStatus.Running)
                        return Task.FromResult(true);

                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        return Task.FromResult(service.Status == ServiceControllerStatus.Running);
                    }
                    
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start service {serviceName}: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task<ServiceStatus> CheckPostgreSQLService()
        {
            var commonNames = new[] { "postgresql-x64-16", "postgresql-x64-15", "postgresql-x64-14", "postgresql", "PostgreSQL" };
            
            foreach (var name in commonNames)
            {
                var status = await CheckServiceStatus(name);
                if (status.ErrorMessage == null)
                {
                    status.DisplayName = "PostgreSQL Database Server";
                    return status;
                }
            }
            
            return new ServiceStatus
            {
                Name = "PostgreSQL",
                DisplayName = "PostgreSQL Database Server",
                Status = ServiceControllerStatus.Stopped,
                IsRunning = false,
                ErrorMessage = "PostgreSQL service not found"
            };
        }

        public async Task<bool> StartPostgreSQLService()
        {
            var commonNames = new[] { "postgresql-x64-16", "postgresql-x64-15", "postgresql-x64-14", "postgresql", "PostgreSQL" };
            
            foreach (var name in commonNames)
            {
                try
                {
                    using (var service = new ServiceController(name))
                    {
                        service.Refresh();
                        if (await StartService(name))
                            return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            
            return false;
        }

        public async Task<ServiceStatus> CheckFixedService()
        {
            var status = await CheckServiceStatus("fixed");
            status.DisplayName = "JMC FIXED";
            return status;
        }

        public async Task<bool> StartFixedService()
        {
            return await StartService("fixed");
        }
    }

    public class ServiceStatus
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ServiceControllerStatus Status { get; set; }
        public bool IsRunning { get; set; }
        public string ErrorMessage { get; set; }
    }
}