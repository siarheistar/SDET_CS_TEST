# Todo App Testing Project

Comprehensive testing suite for a Todo web application using C# + Playwright following SDET best practices.

## 🚀 Features

- ✅ **Unit Tests** - Business logic testing with NUnit
- ✅ **UI Tests** - Browser automation with Playwright
- ✅ **BDD Tests** - Behavior-driven development with SpecFlow
- ✅ **Allure Reports** - Beautiful HTML test reports
- ✅ **CI/CD** - GitHub Actions integration
- ✅ **GitHub Pages** - Automated report hosting

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Allure Commandline](https://docs.qameta.io/allure/#_installing_a_commandline)
  - Windows: `scoop install allure`
  - macOS: `brew install allure`
  - Linux: Download from [releases](https://github.com/allure-framework/allure2/releases)

## 🔧 Setup

### Initial Setup

# Clone or create the project
git init
git add .
git commit -m "Initial commit with test framework"

# Setup
.\setup-project.ps1

# Run tests
.\run-tests.ps1

# Push to GitHub
git remote add origin https://github.com/yourusername/todo-app-testing.git
git branch -M main
git push -u origin main

## 📚 Documentation

- [Playwright for .NET](https://playwright.dev/dotnet/)
- [SpecFlow Documentation](https://docs.specflow.org/)
- [Allure Framework](https://docs.qameta.io/allure/)
- [NUnit Documentation](https://docs.nunit.org/)

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Add tests for new features
4. Ensure all tests pass
5. Submit a pull request

## 📄 License

MIT License - feel free to use this project for learning and development.
