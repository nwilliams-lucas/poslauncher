# Build script for both architectures
param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

Write-Host "Building POS Launcher for both x64 and ARM64 architectures..." -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "publish-x64") { Remove-Item -Path "publish-x64" -Recurse -Force }
if (Test-Path "publish-arm64") { Remove-Item -Path "publish-arm64" -Recurse -Force }

# Build and publish x64
Write-Host "Building x64 version..." -ForegroundColor Cyan
dotnet publish "POSLauncher/POSLauncher.csproj" `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained true `
    --output "publish-x64"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ x64 build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ x64 build completed successfully" -ForegroundColor Green

# Build and publish ARM64
Write-Host "Building ARM64 version..." -ForegroundColor Cyan
dotnet publish "POSLauncher/POSLauncher.csproj" `
    --configuration $Configuration `
    --runtime win-arm64 `
    --self-contained true `
    --output "publish-arm64"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ ARM64 build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ ARM64 build completed successfully" -ForegroundColor Green

# Build Portable versions too
Write-Host "Building Portable x64 version..." -ForegroundColor Cyan
dotnet publish "POSLauncher.Portable/POSLauncher.Portable.csproj" `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained true `
    --output "publish-portable-x64"

Write-Host "Building Portable ARM64 version..." -ForegroundColor Cyan
dotnet publish "POSLauncher.Portable/POSLauncher.Portable.csproj" `
    --configuration $Configuration `
    --runtime win-arm64 `
    --self-contained true `
    --output "publish-portable-arm64"

# Summary
Write-Host ""
Write-Host "🎉 All builds completed successfully!" -ForegroundColor Green
Write-Host "Output directories:" -ForegroundColor Yellow
Write-Host "  - publish-x64/          (Main project x64)" -ForegroundColor White
Write-Host "  - publish-arm64/        (Main project ARM64)" -ForegroundColor White
Write-Host "  - publish-portable-x64/ (Portable project x64)" -ForegroundColor White
Write-Host "  - publish-portable-arm64/ (Portable project ARM64)" -ForegroundColor White
Write-Host ""

# Show file sizes
Write-Host "Executable sizes:" -ForegroundColor Yellow
if (Test-Path "publish-x64/POSLauncher.exe") {
    $x64Size = (Get-Item "publish-x64/POSLauncher.exe").Length
    Write-Host "  - x64: $([math]::Round($x64Size/1MB, 2)) MB" -ForegroundColor White
}
if (Test-Path "publish-arm64/POSLauncher.exe") {
    $arm64Size = (Get-Item "publish-arm64/POSLauncher.exe").Length
    Write-Host "  - ARM64: $([math]::Round($arm64Size/1MB, 2)) MB" -ForegroundColor White
}