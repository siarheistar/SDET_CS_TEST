#!/bin/bash

# Run all tests locally and generate Allure report

echo "Starting test execution..."

# Clean previous results
rm -rf ./SampleWebApp.Tests/bin/Debug/net8.0/allure-results

# Build the solution
echo "Building solution..."
dotnet build --configuration Debug

# Install Playwright browsers
echo "Installing Playwright browsers..."
pwsh SampleWebApp.Tests/bin/Debug/net8.0/playwright.ps1 install

# Run tests
echo "Running tests..."
dotnet test --no-build --configuration Debug --logger "console;verbosity=detailed"

# Generate Allure report
echo "Generating Allure report..."

if ! command -v allure &> /dev/null; then
    echo "Allure is not installed. Please install it"
    echo "Mac: brew install allure"
    echo "Or download from: https://github.com/allure-framework/allure2/releases"
    exit 1
fi

allure generate SampleWebApp.Tests/bin/Debug/net8.0/allure-results -o allure-report --clean
allure open allure-report

echo "Test execution complete!"