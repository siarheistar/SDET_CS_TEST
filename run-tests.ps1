# Run all tests locally and generate Allure report

Write-Host "Starting test execution..." -ForegroundColor Green

# Clean previous results
if (Test-Path ".\SampleWebApp.Tests\bin\Debug\net8.0\allure-results") {
    Remove-Item -Path ".\SampleWebApp.Tests\bin\Debug\net8.0\allure-results" -Recurse -Force
}

# Build the solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --configuration Debug

# Install Playwright browsers
Write-Host "Installing Playwright browsers..." -ForegroundColor Yellow
pwsh SampleWebApp.Tests/bin/Debug/net8.0/playwright.ps1 install

# Run tests
Write-Host "Running tests..." -ForegroundColor Yellow
dotnet test --no-build --configuration Debug --logger "console;verbosity=detailed"

# Generate Allure report
Write-Host "Generating Allure report..." -ForegroundColor Yellow

# Check if Allure is installed
if (-not (Get-Command allure -ErrorAction SilentlyContinue)) {
    Write-Host "Allure is not installed. Please install it using: scoop install allure" -ForegroundColor Red
    Write-Host "Or download from: https://github.com/allure-framework/allure2/releases" -ForegroundColor Red
    exit 1
}

allure generate SampleWebApp.Tests/bin/Debug/net8.0/allure-results -o allure-report --clean
allure open allure-report

Write-Host "Test execution complete!" -ForegroundColor Green