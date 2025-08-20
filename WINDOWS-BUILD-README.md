# POS Launcher - Windows Portable Build

This portable build was created on macOS using .NET 8 and is ready for testing on Windows machines.

## Contents

- `POSLauncher.Portable.exe` - Main application executable (154MB, self-contained)
- `*.dll` - Required Windows runtime libraries
- `*.pdb` - Debug symbols (optional)

## Installation & Testing

### Quick Test
1. Extract `POSLauncher-Windows-Portable.tar.gz` to a folder
2. Right-click `POSLauncher.Portable.exe` → "Run as Administrator"
3. The application will start and attempt to manage services

### Features in This Build
- ✅ **User Interface** - Full WPF UI with status indicators
- ✅ **Service Detection** - Detects PostgreSQL and JMC FIXED services
- ✅ **Service Management** - Attempts to start stopped services
- ✅ **Configuration** - JSON-based configuration system
- ✅ **Startup Registration** - Can register for Windows startup
- ⚠️ **Shortcut Parsing** - Disabled (requires full Windows build with COM)

### Limitations of Portable Build
- **COM References**: Desktop shortcut parsing is disabled (built without COM support)
- **Testing Focus**: This build is primarily for UI and basic functionality testing
- **Service Management**: Full service management requires Administrator privileges

### For Production Use
For production deployment, use the full Windows build created via:
1. **GitHub Actions** - Automated build with full COM support and MSI installer
2. **Windows Machine** - Build directly on Windows with Visual Studio
3. **Full WPF Project** - Use `POSLauncher/POSLauncher.csproj` instead of portable version

## Configuration

The application will create its configuration at:
```
C:\ProgramData\POSLauncher\config.json
```

Example configuration:
```json
{
  "CommerceClientPath": "C:\\Program Files\\Commerce\\Commerce_Client.exe",
  "CommerceClientArguments": "--terminal-id TEST-001 --debug",
  "AutoStartOnBoot": true,
  "ServiceStartTimeoutSeconds": 30
}
```

## Testing Scenarios

### 1. UI Testing
- Launch application and verify interface loads
- Check status indicators and progress feedback
- Test configuration dialog functionality

### 2. Service Testing (Requires Services)
- Install PostgreSQL service
- Create mock "fixed" service for testing
- Verify service detection and startup attempts

### 3. Configuration Testing  
- Modify configuration via UI
- Restart application and verify persistence
- Test different Commerce_Client paths and arguments

## Troubleshooting

### "Application failed to start"
- Ensure .NET 8 runtime is installed (or use self-contained build)
- Run as Administrator for service management
- Check Windows Event Viewer for error details

### Services not starting
- Verify services are installed and accessible
- Check service names match expected patterns:
  - PostgreSQL: `postgresql*` pattern
  - JMC FIXED: Service name `fixed`, Display name `JMC FIXED`
- Ensure Administrator privileges

### Configuration not persisting
- Check write permissions to `C:\ProgramData\POSLauncher\`
- Verify antivirus is not blocking file creation
- Run as Administrator if needed

## Next Steps

After testing this portable build:

1. **Feedback** - Report any UI or functionality issues
2. **Service Testing** - Test with actual PostgreSQL and JMC FIXED services
3. **Production Build** - Use GitHub Actions or Windows development environment for full build
4. **MSI Creation** - Generate proper installer for enterprise deployment

## Build Information

- **Built on**: macOS using .NET 8.0.413
- **Target**: Windows x64
- **Type**: Self-contained portable executable
- **Size**: ~154MB (includes .NET runtime)
- **Version**: 1.0.0.0

This build demonstrates cross-platform development capabilities while targeting Windows-specific functionality.