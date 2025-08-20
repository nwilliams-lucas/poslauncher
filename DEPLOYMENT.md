# POS Launcher Deployment Guide

This guide covers the complete deployment process for the POS Launcher application, from building and packaging to enterprise distribution via Microsoft Intune.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Building the Application](#building-the-application)
- [Packaging for Distribution](#packaging-for-distribution)
- [GitHub Actions CI/CD](#github-actions-cicd)
- [Microsoft Intune Deployment](#microsoft-intune-deployment)
- [VM Testing Environment](#vm-testing-environment)
- [Monitoring and Maintenance](#monitoring-and-maintenance)

## Prerequisites

### Development Environment

- **Visual Studio 2022** (17.8 or later) with MAUI workload
- **.NET 8 SDK** (8.0.100 or later)
- **Windows SDK** (for Windows deployment)
- **Git** for source control

### Build Environment (CI/CD)

- **GitHub Actions** (automated via provided workflow)
- **WiX Toolset v3.11** (for MSI creation)
- **Code signing certificate** (optional, for production)

### Target Environment

- **Windows 10/11** (primary target)
- **Administrator privileges** (for service management)
- **PostgreSQL** and **JMC FIXED** services installed
- **Commerce_Client** application available

## Building the Application

### Local Development Build

#### WPF Version (Windows-specific)

```bash
# Clean and restore
dotnet clean POSLauncher.sln
dotnet restore POSLauncher.sln

# Build debug version
dotnet build POSLauncher/POSLauncher.csproj --configuration Debug

# Build release version
dotnet build POSLauncher/POSLauncher.csproj --configuration Release

# Publish self-contained
dotnet publish POSLauncher/POSLauncher.csproj \
  --configuration Release \
  --output ./publish-wpf \
  --self-contained true \
  --runtime win-x64
```

#### MAUI Version (Cross-platform)

```bash
# Build Windows target
dotnet build POSLauncher.Maui/POSLauncher.Maui.csproj \
  --configuration Release \
  --framework net8.0-windows10.0.19041.0

# Publish Windows MSIX package
dotnet publish POSLauncher.Maui/POSLauncher.Maui.csproj \
  --configuration Release \
  --framework net8.0-windows10.0.19041.0 \
  --output ./publish-maui

# Build macOS target (on macOS)
dotnet build POSLauncher.Maui/POSLauncher.Maui.csproj \
  --configuration Release \
  --framework net8.0-maccatalyst
```

### Production Build Checklist

- [ ] Version numbers updated in project files
- [ ] Release configuration used
- [ ] Self-contained deployment enabled
- [ ] Target runtime specified (win-x64 for Windows)
- [ ] All dependencies included
- [ ] Application manifest configured for admin privileges

## Packaging for Distribution

### MSI Installer Creation

The GitHub Actions workflow automatically creates MSI installers using WiX Toolset. For manual creation:

#### 1. Install WiX Toolset

```powershell
# Download and install WiX 3.11
Invoke-WebRequest -Uri "https://github.com/wixtoolset/wix3/releases/download/wix3112rtm/wix311.exe" -OutFile "wix311.exe"
Start-Process -FilePath "wix311.exe" -ArgumentList "/S" -Wait
```

#### 2. Create WiX Source File (POSLauncher.wxs)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*" Name="POS Launcher" Language="1033" Version="1.0.0.0"
           Manufacturer="Your Company" UpgradeCode="12345678-1234-1234-1234-123456789012">
    <Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine"
             InstallPrivileges="elevated" />

    <MajorUpgrade DowngradeErrorMessage="A newer version is already installed." />
    <MediaTemplate EmbedCab="yes" />

    <Feature Id="ProductFeature" Title="POS Launcher" Level="1">
      <ComponentGroupRef Id="ProductComponents" />
      <ComponentRef Id="StartupRegistryComponent" />
    </Feature>

    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFilesFolder">
        <Directory Id="INSTALLFOLDER" Name="POS Launcher" />
      </Directory>
      <Directory Id="ProgramMenuFolder">
        <Directory Id="ApplicationProgramsFolder" Name="POS Launcher"/>
      </Directory>
    </Directory>

    <ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">
      <Component Id="MainExecutable" Guid="*">
        <File Id="POSLauncherEXE" Source="publish\POSLauncher.exe" KeyPath="yes">
          <Shortcut Id="ApplicationStartMenuShortcut" Directory="ApplicationProgramsFolder"
                   Name="POS Launcher" WorkingDirectory="INSTALLFOLDER"
                   Icon="POSLauncher.exe" IconIndex="0" Advertise="yes" />
          <Shortcut Id="DesktopShortcut" Directory="DesktopFolder"
                   Name="POS Launcher" WorkingDirectory="INSTALLFOLDER"
                   Icon="POSLauncher.exe" IconIndex="0" Advertise="yes" />
        </File>
      </Component>
      <!-- Include all additional files from publish directory -->
    </ComponentGroup>

    <Component Id="StartupRegistryComponent" Directory="INSTALLFOLDER" Guid="*">
      <RegistryValue Root="HKLM" Key="SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"
                     Name="POSLauncher" Type="string"
                     Value="&quot;[INSTALLFOLDER]POSLauncher.exe&quot;" KeyPath="yes" />
    </Component>
  </Product>
</Wix>
```

#### 3. Build MSI

```batch
# Compile WiX source
candle.exe -out POSLauncher.wixobj POSLauncher.wxs

# Link and create MSI
light.exe -out POSLauncher-1.0.0.msi POSLauncher.wixobj
```

### MSIX Package (MAUI)

For Microsoft Store distribution or modern deployment:

```bash
# Build MSIX package
dotnet publish POSLauncher.Maui/POSLauncher.Maui.csproj \
  --configuration Release \
  --framework net8.0-windows10.0.19041.0 \
  --output ./publish-msix \
  /p:GenerateAppxPackageOnBuild=true \
  /p:AppxPackageSigningEnabled=true
```

## GitHub Actions CI/CD

### Automated Build Pipeline

The repository includes a comprehensive GitHub Actions workflow (`.github/workflows/build-and-deploy.yml`) that:

#### 1. Build Stage

- Triggers on push to main/develop branches and tags
- Sets up .NET 8 environment
- Restores dependencies
- Builds solution in Release configuration
- Runs tests (if present)
- Publishes self-contained Windows executable

#### 2. Package Stage

- Creates version information from git tags or build numbers
- Installs WiX Toolset
- Generates MSI installer
- Creates application artifacts

#### 3. Deploy Stage (on main branch or tags)

- Uploads MSI to Intune (requires configuration)
- Creates GitHub releases for tagged versions
- Attaches MSI installer to releases

### Required GitHub Secrets

Configure these secrets in your GitHub repository:

```yaml
AZURE_TENANT_ID          # Azure AD tenant ID
AZURE_CLIENT_ID          # Service principal client ID
AZURE_CLIENT_SECRET      # Service principal secret
CODE_SIGNING_CERT        # Code signing certificate (optional)
CODE_SIGNING_PASSWORD    # Certificate password (optional)
```

### Triggering Builds

#### Development Builds

```bash
git push origin develop
# Triggers build, no deployment
```

#### Production Releases

```bash
git tag v1.0.0
git push origin v1.0.0
# Triggers build, creates MSI, deploys to Intune, creates GitHub release
```

## Microsoft Intune Deployment

### Prerequisites

1. **Microsoft Intune License** and administrative access
2. **Azure AD Service Principal** with appropriate permissions:
   - `DeviceManagementApps.ReadWrite.All`
   - `DeviceManagementConfiguration.ReadWrite.All`

### Service Principal Setup

#### 1. Create Service Principal

```powershell
# Connect to Azure AD
Connect-AzureAD

# Create application registration
$app = New-AzureADApplication -DisplayName "POSLauncher-Intune-Deploy"

# Create service principal
$sp = New-AzureADServicePrincipal -AppId $app.AppId

# Create client secret
$secret = New-AzureADApplicationPasswordCredential -ObjectId $app.ObjectId -CustomKeyIdentifier "Deployment"
```

#### 2. Assign Permissions

1. Go to Azure Portal → App Registrations
2. Find "POSLauncher-Intune-Deploy"
3. Go to API Permissions
4. Add Microsoft Graph permissions:
   - `DeviceManagementApps.ReadWrite.All`
   - `DeviceManagementConfiguration.ReadWrite.All`
5. Grant admin consent

### Intune Application Configuration

#### 1. Upload Application Package

The GitHub Actions workflow can automatically upload the MSI to Intune, or you can do it manually:

1. Sign in to **Microsoft Endpoint Manager admin center**
2. Navigate to **Apps** → **All apps** → **Add**
3. Select **Windows app (Win32)**
4. Upload the MSI file

#### 2. Configure Application Details

```txt
Name: POS Launcher
Description: Automatically manages PostgreSQL, JMC FIXED services and launches Commerce_Client application
Publisher: Your Company
Category: Business
Information URL: https://github.com/yourcompany/poslauncher
Privacy URL: https://yourcompany.com/privacy
Developer: Your Development Team
Owner: IT Department
Notes: Requires administrator privileges for service management
```

#### 3. Program Configuration

```txt
Install command: msiexec /i "POSLauncher.msi" /quiet /norestart ALLUSERS=1
Uninstall command: msiexec /x {ProductCode} /quiet /norestart
Install behavior: System
Device restart behavior: No specific action
Return codes: 0 (Success), 3010 (Success with restart), 1603 (General failure)
```

#### 4. Requirements

```txt
Operating system architecture: 64-bit
Minimum operating system: Windows 10 1809
Additional requirements: .NET 8 Runtime (can be installed separately)
```

#### 5. Detection Rules

```txt
Rule type: MSI
MSI product code: {Auto-detected from MSI file}

OR Custom detection script:
```

```powershell
# Detection script
$appPath = "$env:ProgramFiles\POS Launcher\POSLauncher.exe"
if (Test-Path $appPath) {
    $version = (Get-ItemProperty $appPath).VersionInfo.FileVersion
    Write-Host "POS Launcher version $version detected"
    exit 0
} else {
    exit 1
}
```

#### 6. Dependencies (Optional)

If .NET 8 is not pre-installed:

1. Create .NET 8 Runtime as dependency
2. Configure POS Launcher to depend on it

#### 7. Supersedence (For Updates)

Configure newer versions to supersede older ones:

- Select previous version of POS Launcher
- Set supersedence behavior (uninstall or update)

### Assignment Configuration

#### 1. Target Groups

Create and assign to appropriate groups:

- **POS-Terminals** (Required assignment)
- **Store-Managers** (Available assignment)
- **IT-Test-Group** (Required assignment for testing)

#### 2. Assignment Settings

```txt
Assignment type: Required
Deadline: As soon as possible
User experience settings:
  - End user notifications: Show all toast notifications
  - User experience: Available during business hours
  - Grace period: 3 days
```

#### 3. Delivery Optimization (Optional)

```txt
Download mode: HTTP blended with peering behind same NAT
Bandwidth usage: Percentage of available bandwidth (50%)
```

### Deployment Monitoring

#### 1. Monitor Installation Status

1. Go to **Apps** → **POS Launcher** → **Device install status**
2. Monitor success/failure rates
3. Review error details for failed installations

#### 2. Reporting

Create custom reports for:

- Installation success rates by device group
- Configuration compliance
- Application usage analytics
- Error trends and patterns

## VM Testing Environment

### Test VM Setup

#### 1. Create Base VM

```txt
OS: Windows 10/11 Professional
RAM: 4GB minimum
Storage: 50GB minimum
Network: NAT or Bridged
```

#### 2. Install Prerequisites

```powershell
# Install .NET 8 Runtime
Invoke-WebRequest -Uri "https://download.microsoft.com/download/dotnet/8.0/windowsdesktop-runtime-8.0.0-win-x64.exe" -OutFile "dotnet8.exe"
Start-Process -FilePath "dotnet8.exe" -ArgumentList "/quiet" -Wait

# Install PostgreSQL (for testing)
# Install JMC FIXED service (mock or actual)
# Create test Commerce_Client application
```

#### 3. Test Scenarios

##### Scenario 1: Clean Install

```powershell
# Test MSI installation
msiexec /i POSLauncher.msi /quiet /l*v install.log
# Verify installation
Test-Path "$env:ProgramFiles\POS Launcher\POSLauncher.exe"
# Test application launch
& "$env:ProgramFiles\POS Launcher\POSLauncher.exe"
```

##### Scenario 2: Upgrade Test

```powershell
# Install older version first
msiexec /i POSLauncher-v1.0.0.msi /quiet
# Install newer version
msiexec /i POSLauncher-v1.1.0.msi /quiet
# Verify upgrade success and configuration retention
```

##### Scenario 3: Configuration Persistence

```powershell
# Configure application settings
# Stop application
# Restart application
# Verify settings retained
```

### Automated Testing Script

```powershell
# Comprehensive test script
param(
    [string]$MsiPath = "POSLauncher.msi",
    [string]$TestConfigPath = "test-config.json"
)

function Test-Installation {
    Write-Host "Testing MSI installation..."
    $result = Start-Process -FilePath "msiexec.exe" -ArgumentList "/i `"$MsiPath`" /quiet /norestart" -Wait -PassThru
    return $result.ExitCode -eq 0
}

function Test-ServiceStartup {
    Write-Host "Testing service startup functionality..."
    $app = Start-Process -FilePath "$env:ProgramFiles\POS Launcher\POSLauncher.exe" -PassThru
    Start-Sleep 10

    # Check if services started (mock test)
    $postgresRunning = Get-Service -Name "postgresql*" -ErrorAction SilentlyContinue | Where-Object {$_.Status -eq "Running"}
    $fixedRunning = Get-Service -Name "fixed" -ErrorAction SilentlyContinue | Where-Object {$_.Status -eq "Running"}

    Stop-Process -Id $app.Id -Force

    return ($postgresRunning -and $fixedRunning)
}

function Test-ConfigurationPersistence {
    Write-Host "Testing configuration persistence..."

    # Create test configuration
    $testConfig = @{
        CommerceClientPath = "C:\Test\Commerce_Client.exe"
        CommerceClientArguments = "--test-mode"
        AutoStartOnBoot = $false
    }

    $configPath = "$env:ProgramData\POSLauncher\config.json"
    $testConfig | ConvertTo-Json | Out-File -FilePath $configPath -Encoding UTF8

    # Start and stop application
    $app = Start-Process -FilePath "$env:ProgramFiles\POS Launcher\POSLauncher.exe" -PassThru
    Start-Sleep 5
    Stop-Process -Id $app.Id -Force

    # Verify configuration retained
    $savedConfig = Get-Content -Path $configPath | ConvertFrom-Json
    return ($savedConfig.CommerceClientPath -eq $testConfig.CommerceClientPath)
}

# Run all tests
$installTest = Test-Installation
$serviceTest = Test-ServiceStartup
$configTest = Test-ConfigurationPersistence

Write-Host "Installation Test: $(if($installTest) {'PASS'} else {'FAIL'})"
Write-Host "Service Test: $(if($serviceTest) {'PASS'} else {'FAIL'})"
Write-Host "Configuration Test: $(if($configTest) {'PASS'} else {'FAIL'})"

$allPassed = $installTest -and $serviceTest -and $configTest
Write-Host "Overall Result: $(if($allPassed) {'PASS'} else {'FAIL'})"
exit $(if($allPassed) {0} else {1})
```

## Monitoring and Maintenance

### Application Monitoring

#### 1. Event Logging

The application writes to Windows Event Log:

```txt
Source: POS Launcher
Log: Application
Event IDs:
  1001 - Application started
  1002 - Service startup success
  1003 - Service startup failure
  1004 - Configuration changed
  1005 - Application error
```

#### 2. Performance Counters

Monitor key metrics:

- Service startup times
- Application launch success rate
- Configuration errors
- Resource usage

### Health Checks

#### PowerShell Health Check Script

```powershell
function Test-POSLauncherHealth {
    $healthStatus = @{
        ApplicationInstalled = $false
        ServicesRunning = $false
        ConfigurationValid = $false
        StartupRegistered = $false
    }

    # Check installation
    $appPath = "$env:ProgramFiles\POS Launcher\POSLauncher.exe"
    $healthStatus.ApplicationInstalled = Test-Path $appPath

    # Check services
    $postgres = Get-Service -Name "postgresql*" -ErrorAction SilentlyContinue
    $fixed = Get-Service -Name "fixed" -ErrorAction SilentlyContinue
    $healthStatus.ServicesRunning = ($postgres.Status -eq "Running") -and ($fixed.Status -eq "Running")

    # Check configuration
    $configPath = "$env:ProgramData\POSLauncher\config.json"
    if (Test-Path $configPath) {
        try {
            $config = Get-Content $configPath | ConvertFrom-Json
            $healthStatus.ConfigurationValid = -not [string]::IsNullOrEmpty($config.CommerceClientPath)
        } catch {
            $healthStatus.ConfigurationValid = $false
        }
    }

    # Check startup registration
    $startupReg = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run" -Name "POSLauncher" -ErrorAction SilentlyContinue
    $healthStatus.StartupRegistered = $null -ne $startupReg

    return $healthStatus
}

# Usage
$health = Test-POSLauncherHealth
$health | ConvertTo-Json -Depth 2
```

### Update Management

#### 1. Version Control Strategy

- **Major versions** (1.x.x): Breaking changes, new features
- **Minor versions** (x.1.x): New features, backward compatible
- **Patch versions** (x.x.1): Bug fixes, security updates

#### 2. Rollout Strategy

1. **Alpha Testing**: Internal testing team
2. **Beta Testing**: Select pilot stores/terminals
3. **Staged Rollout**: Gradual deployment to production
4. **Full Deployment**: All systems updated

#### 3. Rollback Procedure

```powershell
# Emergency rollback script
param([string]$PreviousVersion = "1.0.0")

Write-Host "Rolling back to version $PreviousVersion..."

# Uninstall current version
$currentApp = Get-WmiObject -Query "SELECT * FROM Win32_Product WHERE Name='POS Launcher'"
if ($currentApp) {
    $currentApp.Uninstall()
}

# Install previous version
$rollbackMsi = "POSLauncher-$PreviousVersion.msi"
if (Test-Path $rollbackMsi) {
    Start-Process -FilePath "msiexec.exe" -ArgumentList "/i `"$rollbackMsi`" /quiet /norestart" -Wait
    Write-Host "Rollback completed successfully."
} else {
    Write-Error "Rollback MSI not found: $rollbackMsi"
}
```

### Backup and Recovery

#### Configuration Backup

```powershell
# Daily configuration backup
$source = "$env:ProgramData\POSLauncher"
$destination = "\\backup-server\POSLauncher\$env:COMPUTERNAME\$(Get-Date -Format 'yyyy-MM-dd')"

if (Test-Path $source) {
    New-Item -Path $destination -ItemType Directory -Force
    Copy-Item -Path "$source\*" -Destination $destination -Recurse -Force
    Write-Host "Configuration backed up to $destination"
}
```

This deployment guide provides comprehensive instructions for building, packaging, deploying, and maintaining the POS Launcher application across enterprise environments.
