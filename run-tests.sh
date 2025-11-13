#!/bin/bash

# Run all tests locally and generate Allure report

echo "Starting test execution..."

# Clean previous results
rm -rf ./allure-results

# Build the solution
echo "Building solution..."
dotnet build --configuration Debug

# Install Playwright browsers
echo "Installing Playwright browsers..."
pwsh SampleWebApp.Tests/bin/Debug/net8.0/playwright.ps1 install

# Run tests
echo "Running tests..."
dotnet test --no-build --configuration Debug --logger "console;verbosity=detailed"

# Move allure results from bin/Debug to project root for easier access
echo "Moving allure results to project root..."
if [ -d "SampleWebApp.Tests/bin/Debug/net8.0/allure-results" ]; then
    mv SampleWebApp.Tests/bin/Debug/net8.0/allure-results ./allure-results
fi

# Generate Allure report
echo "Generating Allure report..."

if ! command -v allure &> /dev/null; then
    echo "Allure is not installed. Please install it"
    echo "Mac: brew install allure"
    echo "Or download from: https://github.com/allure-framework/allure2/releases"
    exit 1
fi

# Copy history from previous report to preserve trends
if [ -d "allure-report/history" ]; then
    echo "Preserving test history for trends..."
    mkdir -p allure-results/history
    cp -r allure-report/history/* allure-results/history/
fi

# Generate report
allure generate allure-results -o allure-report --clean

echo ""
echo "Note: Run tests multiple times to see trends in the Allure report."
echo "Trend history is being preserved between runs."
echo ""

allure open allure-report

echo "Test execution complete!"