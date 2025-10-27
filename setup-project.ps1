# Setup script for Todo App Testing Project

Write-Host "Setting up Todo App Testing Project..." -ForegroundColor Green

# Create directory structure
$directories = @(
    "SampleWebApp",
    "SampleWebApp.Tests/BDDTests/Features",
    "SampleWebApp.Tests/BDDTests/StepDefinitions",
    "SampleWebApp.Tests/UITests",
    "SampleWebApp.Tests/UnitTests",
    "SampleWebApp.Tests/TestResults",
    ".github/workflows"
)

foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "Created directory: $dir" -ForegroundColor Yellow
    }
}

# Restore NuGet packages
Write-Host "`nRestoring NuGet packages..." -ForegroundColor Green
dotnet restore

# Build the solution
Write-Host "`nBuilding solution..." -ForegroundColor Green
dotnet build --configuration Debug

# Install Playwright browsers
Write-Host "`nInstalling Playwright browsers..." -ForegroundColor Green
if (Test-Path "SampleWebApp.Tests/bin/Debug/net8.0") {
    pwsh SampleWebApp.Tests/bin/Debug/net8.0/playwright.ps1 install chromium
    Write-Host "Playwright browsers installed successfully!" -ForegroundColor Green
} else {
    Write-Host "Build first, then run: pwsh SampleWebApp.Tests/bin/Debug/net8.0/playwright.ps1 install" -ForegroundColor Red
}

Write-Host "`nProject setup complete!" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Open TodoAppTesting.sln in Visual Studio" -ForegroundColor White
Write-Host "2. Build the solution" -ForegroundColor White
Write-Host "3. Run tests with: dotnet test" -ForegroundColor White
Write-Host "4. Generate Allure report with: .\run-tests.ps1" -ForegroundColor White