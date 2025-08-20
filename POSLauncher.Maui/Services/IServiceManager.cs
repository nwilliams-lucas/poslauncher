namespace POSLauncher.Maui.Services
{
    public interface IServiceManager
    {
        Task<ServiceStatus> CheckServiceStatusAsync(string serviceName);
        Task<bool> StartServiceAsync(string serviceName);
        Task<ServiceStatus> CheckPostgreSQLServiceAsync();
        Task<bool> StartPostgreSQLServiceAsync();
        Task<ServiceStatus> CheckFixedServiceAsync();
        Task<bool> StartFixedServiceAsync();
    }

    public class ServiceStatus
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public ServiceState Status { get; set; }
        public bool IsRunning { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public enum ServiceState
    {
        Unknown,
        Stopped,
        Running,
        Starting,
        Stopping,
        Error
    }
}