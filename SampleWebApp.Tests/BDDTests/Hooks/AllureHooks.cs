using TechTalk.SpecFlow;

namespace SampleWebApp.Tests.BDDTests.Hooks;

[Binding]
public class AllureHooks
{
    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext)
    {
        // Add scenario information to console for debugging
        var scenarioName = scenarioContext.ScenarioInfo.Title;
        Console.WriteLine($"Starting scenario: {scenarioName}");
    }

    [AfterScenario]
    public void AfterScenario(ScenarioContext scenarioContext)
    {
        if (scenarioContext.TestError != null)
        {
            Console.WriteLine($"Scenario failed: {scenarioContext.TestError.Message}");
        }
    }
}
