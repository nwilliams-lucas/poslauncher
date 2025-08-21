# POS Launcher Configuration Guide

This document provides comprehensive configuration instructions for deploying and managing the POS Launcher application across different environments.

## Table of Contents

- [Initial Configuration](#initial-configuration)
- [Machine-Specific Settings](#machine-specific-settings)
- [Service Configuration](#service-configuration)
- [Application Deployment](#application-deployment)
- [Intune Deployment](#intune-deployment)
- [Troubleshooting](#troubleshooting)

## Initial Configuration

### 1. First-Time Setup

When the application runs for the first time, it will:

1. **Create Configuration Directory**:

   - Windows: `%ProgramData%\POSLauncher\`
   - macOS: `~/Library/Application Support/POSLauncher/`
   - Other platforms: App-specific data directory

2. **Generate Default Configuration**:

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

3. **Attempt Auto-Discovery**: The application will scan for Commerce_Client shortcuts on the desktop to automatically configure paths and parameters.

### 2. Manual Configuration

If auto-discovery fails or you need to customize settings:

1. **Launch the Application**
2. **Click "Configure" Button**
3. **Update Settings**:
   - **Application Path**: Browse to Commerce_Client.exe location
   - **Arguments**: Enter command line parameters specific to your machine
   - **Service Timeout**: Adjust if services take longer to start
   - **UI Behavior**: Configure minimization and status display preferences

## Machine-Specific Settings

### Commerce_Client Parameters

Each machine may require different Commerce_Client parameters. Common scenarios:

#### Point of Sale Terminal

```shell
--terminal-id 001 --location "Store-Front" --network-mode local
```

#### Back Office System

```shell
--role admin --database-server postgres01 --port 5432
```

#### Kiosk Mode

```shell
--kiosk-mode --fullscreen --auto-login --terminal-id kiosk-01
```

### Configuration Methods

#### Method 1: Desktop Shortcut (Recommended)

1. Create a Desktop shortcut to Commerce_Client.exe
2. Right-click shortcut → Properties
3. Add parameters to "Target" field after the executable path
4. The POS Launcher will automatically detect and use these parameters

#### Method 2: Manual Configuration

1. Open POS Launcher
2. Click "Configure"
3. Enter path and parameters manually
4. Save configuration

#### Method 3: Configuration File Deployment

Deploy a pre-configured `config.json` file to each machine:

```json
{
  "CommerceClientPath": "C:\\Program Files\\Commerce\\Commerce_Client.exe",
  "CommerceClientArguments": "--terminal-id {MACHINE-ID} --location {STORE-NAME}",
  "AutoStartOnBoot": true,
  "ServiceStartTimeoutSeconds": 45,
  "MinimizeOnComplete": true,
  "ShowStatusUpdates": true
}
```

## Service Configuration

### PostgreSQL Service Detection

The application automatically detects PostgreSQL services using common naming patterns:

- `postgresql-x64-16` (PostgreSQL 16)
- `postgresql-x64-15` (PostgreSQL 15)
- `postgresql-x64-14` (PostgreSQL 14)
- `postgresql` (Generic)
- `PostgreSQL` (Generic)

#### Custom PostgreSQL Configuration

If your PostgreSQL service uses a different name, you may need to modify the service detection logic in the platform-specific service managers.

### JMC FIXED Service

The application looks for a Windows service with:

- **Service Name**: `fixed`
- **Display Name**: `JMC FIXED`

Ensure your JMC FIXED service is installed with these exact identifiers.

### Service Startup Order

Services are started in this specific order:

1. PostgreSQL Database Server
2. JMC FIXED Service
3. Commerce_Client Application

Each service must successfully start before proceeding to the next.

## Application Deployment

### Windows Deployment Options

#### Option 1: MSI Installer (Recommended)

1. Download the MSI from GitHub releases
2. Run as Administrator: `msiexec /i POSLauncher.msi /quiet`
3. Application installs to `%ProgramFiles%\POS Launcher\`
4. Automatically registers for startup

#### Option 2: Standalone Executable

1. Download the published executable package
2. Copy to desired location (e.g., `C:\Tools\POSLauncher\`)
3. Run once as Administrator to configure startup registration
4. Create desktop shortcuts as needed

#### Option 3: Group Policy Deployment

1. Place MSI in a network share
2. Create Group Policy for software installation
3. Deploy to target organizational units
4. Configure startup policies if needed

### Cross-Platform Deployment



## Intune Deployment

### Prerequisites

1. Microsoft Intune subscription
2. Appropriate permissions to upload and deploy applications
3. Target device groups configured

### Deployment Steps

#### 1. Prepare Application Package



#### 2. Upload to Intune

1. Sign in to Microsoft Endpoint Manager admin center
2. Navigate to **Apps** → **All apps** → **Add**
3. Select **Windows app (Win32)**
4. Upload the MSI file

#### 3. Configure App Information

```txt
Name: POS Launcher
Description: Service launcher for PostgreSQL, JMC FIXED, and Commerce_Client
Publisher: Your Company Name
Category: Business
```

#### 4. Configure Program Settings

```txt
Install command: msiexec /i "POSLauncher.msi" /quiet /norestart
Uninstall command: msiexec /x "POSLauncher.msi" /quiet /norestart
Install behavior: System
```

#### 5. Configure Requirements

```txt
Operating system architecture: x64
Minimum operating system: Windows 10 1809
```

#### 6. Configure Detection Rules

```txt
Rule type: MSI
MSI product code: {Auto-detected from MSI}
```

#### 7. Configure Assignment

1. Select target groups (e.g., "POS Terminals", "Store Systems")
2. Set assignment type to "Required"
3. Configure schedule if needed

### Post-Deployment Verification

After Intune deployment, verify on target machines:

1. **Application Installation**: Check `%ProgramFiles%\POS Launcher\`
2. **Startup Registration**: Verify in Windows startup programs
3. **Service Functionality**: Test service startup capability
4. **Configuration**: Ensure machine-specific settings are applied

## Advanced Configuration

### Group Policy Integration

Create Group Policy settings for enterprise-wide configuration:

1. **Registry Settings**:

   ```txt
   HKEY_LOCAL_MACHINE\SOFTWARE\Policies\POSLauncher
   - CommerceClientPath (REG_SZ)
   - CommerceClientArguments (REG_SZ)
   - ServiceTimeout (REG_DWORD)
   ```

2. **Administrative Templates**: Create custom ADMX files for centralized management

### PowerShell Configuration Script

Deploy configuration via PowerShell:

```powershell
# Configure POS Launcher settings
$configPath = "$env:ProgramData\POSLauncher\config.json"
$config = @{
    CommerceClientPath = "C:\Program Files\Commerce\Commerce_Client.exe"
    CommerceClientArguments = "--terminal-id $env:COMPUTERNAME --location Store01"
    AutoStartOnBoot = $true
    ServiceStartTimeoutSeconds = 30
    MinimizeOnComplete = $true
    ShowStatusUpdates = $true
} | ConvertTo-Json

# Ensure directory exists
New-Item -Path (Split-Path $configPath) -ItemType Directory -Force
$config | Out-File -FilePath $configPath -Encoding UTF8
```

### Environment-Specific Configuration

#### Development Environment

```json
{
  "CommerceClientPath": "C:\\Dev\\Commerce\\bin\\Debug\\Commerce_Client.exe",
  "CommerceClientArguments": "--debug --log-level verbose",
  "ServiceStartTimeoutSeconds": 60,
  "ShowStatusUpdates": true
}
```

#### Production Environment

```json
{
  "CommerceClientPath": "C:\\Program Files\\Commerce\\Commerce_Client.exe",
  "CommerceClientArguments": "--production --terminal-id PROD-001",
  "ServiceStartTimeoutSeconds": 30,
  "MinimizeOnComplete": true
}
```

## Configuration Validation

### Validation Checklist

Before deploying to production, validate:

- [ ] PostgreSQL service is installed and configured
- [ ] JMC FIXED service is installed with correct name
- [ ] Commerce_Client application exists at specified path
- [ ] Command line arguments are valid for target environment
- [ ] Application has necessary permissions (Administrator for service management)
- [ ] Network connectivity requirements are met
- [ ] Startup registration works correctly
- [ ] Configuration persists after application restart

### Testing Procedure

1. **Clean Install Test**:

   - Install on fresh system
   - Verify auto-configuration
   - Test service startup sequence
   - Confirm application launches

2. **Configuration Persistence Test**:

   - Modify configuration
   - Restart application
   - Verify settings retained
   - Test different parameter combinations

3. **Error Handling Test**:
   - Test with missing services
   - Test with invalid application path
   - Test with incorrect parameters
   - Verify error messages and recovery

## Security Considerations

### Permissions Required

- **Administrator privileges**: Required for service management and startup registration
- **File system access**: Read/write to configuration directory
- **Registry access**: For startup registration (Windows)
- **Network access**: If Commerce_Client requires network connectivity

### Security Best Practices

1. **Principle of Least Privilege**: Run with minimum required permissions
2. **Configuration Security**: Protect configuration files from unauthorized modification
3. **Service Account**: Consider dedicated service account for production deployments
4. **Audit Logging**: Monitor configuration changes and service activities

## Maintenance and Updates

### Update Process

1. Test new version in non-production environment
2. Update MSI package in Intune
3. Deploy to pilot group first
4. Monitor for issues before full rollout
5. Maintain rollback capability

### Configuration Backup

Regularly backup configuration files:

```powershell
# Backup configuration
Copy-Item "$env:ProgramData\POSLauncher\config.json" "\\backup-server\config-backup\$env:COMPUTERNAME-$(Get-Date -Format 'yyyyMMdd').json"
```

### Health Monitoring

Implement monitoring for:

- Application startup success/failure
- Service startup times
- Configuration changes
- Error rates and patterns

This configuration guide should be reviewed and updated as new requirements emerge or the application evolves.
