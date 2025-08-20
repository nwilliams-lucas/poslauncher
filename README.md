# POS Launcher

A cross-platform application designed to automatically start and manage services required for a Point of Sale (POS) system. Built with .NET MAUI for Windows, macOS, iOS, and Android support.

## Features

### Core Features

- **Cross-Platform Support**: Built with .NET MAUI for Windows, macOS, iOS, and Android
- **Service Management**: Automatically starts PostgreSQL and JMC FIXED services (Windows primary focus)
- **Application Launcher**: Launches Commerce_Client with machine-specific parameters
- **Visual Feedback**: Real-time status updates with intuitive UI and banner logo
- **Startup Integration**: Automatically registers to start with system boot
- **Configuration Management**: Machine-specific settings for different deployment environments
- **Enterprise Ready**: Includes deployment options for Microsoft Intune

### User Experience

- **Modern UI**: Clean, responsive interface with status indicators
- **Banner Logo**: Professional branding with gear icon and service status indicators
- **Progress Tracking**: Visual feedback for each service startup stage
- **Configuration Interface**: Easy-to-use settings page for customization
- **Cross-Platform Consistency**: Unified experience across all supported platforms

## Architecture

The application uses a hybrid approach with both WPF and .NET MAUI implementations:

### .NET MAUI Cross-Platform (Recommended)

- **Platform-Specific Services**: Abstracted service management for each platform
- **Dependency Injection**: Clean separation of concerns with DI container
- **MVVM Pattern**: ViewModels with data binding for reactive UI
- **Cross-Platform Interfaces**: Consistent API across Windows, Mac, iOS, Android

#### Core Components

- **IServiceManager**: Platform-specific service detection and management
- **IApplicationLauncher**: Application launching with parameter handling
- **IConfigurationService**: JSON-based configuration persistence
- **IStartupManager**: Platform-specific startup registration
- **ViewModels**: Business logic and data binding
- **Views**: XAML-based UI with responsive design

### Legacy WPF Windows (Maintained)

- **ServiceManager**: Windows service detection and startup
- **CommerceClientLauncher**: Commerce_Client application launching
- **ConfigurationManager**: Machine-specific configuration storage
- **StartupManager**: Windows startup registration
- **Visual UI**: Progress feedback and configuration interface

## Startup Sequence

1. **Startup Registration**: Ensures the application is registered to start with Windows
2. **PostgreSQL Service**: Checks and starts PostgreSQL database service
3. **JMC FIXED Service**: Checks and starts the "fixed" service (JMC FIXED)
4. **Commerce_Client**: Launches the Commerce_Client application with configured parameters
5. **Minimization**: Minimizes to system tray after successful completion

## Configuration

The application stores configuration in `%ProgramData%\POSLauncher\config.json`:

```json
{
  "CommerceClientPath": "C:\\Path\\To\\Commerce_Client.exe",
  "CommerceClientArguments": "--param1 value1 --param2 value2",
  "AutoStartOnBoot": true,
  "ServiceStartTimeoutSeconds": 30
}
```

### Automatic Configuration Discovery

The application attempts to automatically find Commerce_Client configuration by:

1. Scanning desktop shortcuts for "Commerce_Client" or "Commerce Client"
2. Extracting the target path and command line arguments
3. Storing the configuration for future use

## Building

### Requirements

- .NET 8 SDK
- Windows development environment
- Visual Studio 2022 (recommended)

### Local Build

```bash
dotnet restore POSLauncher.sln
dotnet build POSLauncher.sln --configuration Release
dotnet publish POSLauncher/POSLauncher.csproj --configuration Release --self-contained true --runtime win-x64
```

### GitHub Actions

The project includes a complete CI/CD pipeline that:

1. Builds the application
2. Creates an MSI installer using WiX Toolset
3. Uploads artifacts
4. Creates GitHub releases for tagged versions
5. Prepares for Intune deployment

## Deployment

### Manual Installation

1. Download the MSI installer from the releases page
2. Run the installer as Administrator
3. The application will automatically start and configure itself

### Intune Deployment

1. Upload the MSI file to Microsoft Intune
2. Create a Win32 app package
3. Configure installation command: `msiexec /i "POSLauncher.msi" /quiet`
4. Deploy to target device groups

The application requires administrator privileges to manage Windows services and startup registration.

## Configuration for Different Machines

Each machine can have different Commerce_Client parameters. The configuration is stored locally and can be updated through the Configuration window or by placing a desktop shortcut with the correct parameters.

## Troubleshooting

### Common Issues

1. **Services Not Starting**: Ensure the application is running as Administrator
2. **Commerce_Client Not Found**: Use the Configuration window to manually set the path
3. **Startup Registration Failed**: Check Windows permissions and UAC settings

### Logs

The application outputs debug information to the Visual Studio output window during development. In production, check Windows Event Viewer for application errors.

## Development

### Project Structure

```shell
POSLauncher/
├── Services/
│   ├── ServiceManager.cs          # Windows service management
│   ├── CommerceClientLauncher.cs  # Application launcher
│   ├── ConfigurationManager.cs    # Settings management
│   └── StartupManager.cs          # Windows startup registration
├── MainWindow.xaml                # Main UI
├── ConfigurationWindow.xaml       # Settings UI
└── App.xaml                       # Application entry point
```

### Testing in VM

The application is designed to be tested in a Windows virtual machine:

1. Build and deploy the MSI
2. Install in the VM
3. Verify service startup behavior
4. Test with different Commerce_Client configurations

## Security Considerations

- The application requires administrator privileges
- Configuration files are stored in a secure location (`%ProgramData%`)
- No sensitive information is logged or transmitted
- All service operations use Windows APIs with proper error handling

## License

[Add your license information here]
