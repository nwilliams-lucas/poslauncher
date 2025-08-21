# POS Launcher ARM64 Installation Script
# Run this script as Administrator to install POS Launcher

param(
    [string]$SourcePath = ".",
    [string]$InstallPath = "$env:ProgramFiles\POS Launcher"
)

$ErrorActionPreference = "Stop"

Write-Host "POS Launcher ARM64 Installation Script" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as Administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ This script must be run as Administrator!" -ForegroundColor Red
    Write-Host ""
    Write-Host "To run as Administrator:" -ForegroundColor Yellow
    Write-Host "1. Right-click on PowerShell" -ForegroundColor White
    Write-Host "2. Select 'Run as Administrator'" -ForegroundColor White
    Write-Host "3. Navigate to this directory and run the script again" -ForegroundColor White
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

try {
    Write-Host "Installing POS Launcher (ARM64)..." -ForegroundColor Green
    Write-Host "Source: $SourcePath" -ForegroundColor Gray
    Write-Host "Destination: $InstallPath" -ForegroundColor Gray
    Write-Host ""

    # Verify source files exist
    $exePath = Join-Path $SourcePath "POSLauncher.exe"
    if (-not (Test-Path $exePath)) {
        throw "POSLauncher.exe not found in $SourcePath. Please ensure you're running this script from the extracted ZIP folder."
    }

    # Create installation directory
    Write-Host "📁 Creating installation directory..." -ForegroundColor Yellow
    if (-not (Test-Path $InstallPath)) {
        New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
    }

    # Copy files
    Write-Host "📋 Copying application files..." -ForegroundColor Yellow
    Copy-Item -Path "$SourcePath\POSLauncher.exe" -Destination $InstallPath -Force
    
    # Copy all DLLs and PDBs
    Get-ChildItem -Path $SourcePath -Filter "*.dll" | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $InstallPath -Force
    }
    
    Get-ChildItem -Path $SourcePath -Filter "*.pdb" | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $InstallPath -Force
    }

    # Create start menu shortcut
    Write-Host "🔗 Creating Start Menu shortcut..." -ForegroundColor Yellow
    $startMenuPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs"
    $shortcutPath = Join-Path $startMenuPath "POS Launcher.lnk"
    
    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($shortcutPath)
    $shortcut.TargetPath = Join-Path $InstallPath "POSLauncher.exe"
    $shortcut.Description = "Service launcher for PostgreSQL, JMC FIXED, and Commerce_Client"
    $shortcut.WorkingDirectory = $InstallPath
    $shortcut.Save()

    # Create startup shortcut
    Write-Host "🚀 Configuring automatic startup..." -ForegroundColor Yellow
    $startupPath = "$env:ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"
    $startupShortcutPath = Join-Path $startupPath "POS Launcher.lnk"
    
    $startupShortcut = $shell.CreateShortcut($startupShortcutPath)
    $startupShortcut.TargetPath = Join-Path $InstallPath "POSLauncher.exe"
    $startupShortcut.Description = "Service launcher for PostgreSQL, JMC FIXED, and Commerce_Client"
    $startupShortcut.WorkingDirectory = $InstallPath
    $startupShortcut.Save()

    # Success message
    Write-Host ""
    Write-Host "✅ POS Launcher installed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Installation Details:" -ForegroundColor Cyan
    Write-Host "  📍 Installation path: $InstallPath" -ForegroundColor White
    Write-Host "  🔗 Start Menu shortcut: $shortcutPath" -ForegroundColor White  
    Write-Host "  🚀 Startup shortcut: $startupShortcutPath" -ForegroundColor White
    Write-Host ""
    Write-Host "The application will start automatically when you log in to Windows." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To start now, you can:" -ForegroundColor Green
    Write-Host "  • Search for 'POS Launcher' in the Start Menu" -ForegroundColor White
    Write-Host "  • Or run: & '$InstallPath\POSLauncher.exe'" -ForegroundColor White
    Write-Host ""
    
    # Uninstall instructions
    Write-Host "To uninstall later, delete these items:" -ForegroundColor Cyan
    Write-Host "  • $InstallPath" -ForegroundColor Gray
    Write-Host "  • $shortcutPath" -ForegroundColor Gray
    Write-Host "  • $startupShortcutPath" -ForegroundColor Gray
    Write-Host ""
    
    $response = Read-Host "Would you like to start POS Launcher now? (y/N)"
    if ($response -eq "y" -or $response -eq "Y") {
        Write-Host "Starting POS Launcher..." -ForegroundColor Green
        Start-Process -FilePath (Join-Path $InstallPath "POSLauncher.exe") -WorkingDirectory $InstallPath
    }
}
catch {
    Write-Host ""
    Write-Host "❌ Installation failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Read-Host "Press Enter to exit"