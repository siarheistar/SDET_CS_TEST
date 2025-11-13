using NUnit.Framework;
using Allure.Net.Commons;
using System;

[SetUpFixture]
public class AllureSetup
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Set Allure results directory in bin/Debug (will be moved to project root by run-tests.sh)
        var resultsDirectory = Path.Combine(
            TestContext.CurrentContext.TestDirectory,
            "allure-results"
        );

        Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIR", resultsDirectory);

        // Ensure directory exists
        if (!Directory.Exists(resultsDirectory))
        {
            Directory.CreateDirectory(resultsDirectory);
        }

        Console.WriteLine($"Allure results will be saved to: {resultsDirectory}");
        Console.WriteLine($"(will be moved to project root after test execution)");
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        var resultsDirectory = Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIR");
        Console.WriteLine($"Allure results saved to: {resultsDirectory}");

        if (Directory.Exists(resultsDirectory))
        {
            var fileCount = Directory.GetFiles(resultsDirectory, "*.json").Length;
            Console.WriteLine($"Generated {fileCount} Allure result files");
        }
    }
}
