# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

POSLauncher is a Windows WPF application that manages service startup for Point of Sale (POS) systems. It automatically starts PostgreSQL and JMC FIXED services, then launches the Commerce_Client application with machine-specific parameters.

## Architecture

### Two Project Structure
- **POSLauncher**: Main WPF application (.NET 8)  
- **POSLauncher.Portable**: Portable version with identical functionality

### Core Services Layer
Located in `POSLauncher/Services/`:
- **ServiceManager.cs**: Windows service detection, status checking, and startup
- **CommerceClientLauncher.cs**: Commerce_Client application launching with auto-discovery
- **ConfigurationManager.cs**: JSON-based configuration storage in `%ProgramData%\POSLauncher\`
- **StartupManager.cs**: Windows startup registration and management

### Service Startup Sequence
1. PostgreSQL service detection and startup (multiple version patterns supported)
2. JMC FIXED service startup (`fixed` service name)
3. Commerce_Client application launch with configured parameters
4. Application minimization to system tray

### Configuration Discovery
The application automatically scans desktop shortcuts for "Commerce_Client" to extract paths and command-line arguments, storing them in `config.json`.

## Development Commands

### Build Commands
```bash
# Restore dependencies
dotnet restore POSLauncher.sln

# Build solution
dotnet build POSLauncher.sln --configuration Release

# Publish self-contained executable
dotnet publish POSLauncher/POSLauncher.csproj --configuration Release --self-contained true --runtime win-x64

# Publish portable version  
dotnet publish POSLauncher.Portable/POSLauncher.Portable.csproj --configuration Release --self-contained true --runtime win-x64
```

### Key Dependencies
- **System.ServiceProcess.ServiceController**: Windows service management
- **Microsoft.Win32.Registry**: Startup registration
- **Microsoft.WindowsAPICodePack-Shell**: Desktop shortcut parsing (main project only)

## Configuration

### Default Configuration Structure
```json
{
  "CommerceClientPath": "",
  "CommerceClientArguments": "",
  "AutoStartOnBoot": true,
  "ServiceStartTimeoutSeconds": 30,
  "MinimizeOnComplete": true,
  "ShowStatusUpdates": true
}
```

### Service Detection Patterns
- **PostgreSQL**: `postgresql-x64-16`, `postgresql-x64-15`, `postgresql-x64-14`, `postgresql`, `PostgreSQL`
- **JMC FIXED**: Service name `fixed`, display name `JMC FIXED`

## Development Notes

### Windows-Specific Requirements
- Requires Administrator privileges for service management
- Uses Windows Service Controller API for service operations
- Registry access needed for startup registration
- Target framework: `net8.0-windows` with WPF enabled

### Configuration Storage
- Production: `%ProgramData%\POSLauncher\config.json`
- Auto-discovery scans desktop for Commerce_Client shortcuts
- Machine-specific parameters supported through configuration templates

### UI Pattern
- MainWindow.xaml: Primary interface with status indicators
- ConfigurationWindow.xaml: Settings management interface
- Real-time progress feedback during service startup sequence