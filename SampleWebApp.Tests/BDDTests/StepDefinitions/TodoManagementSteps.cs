using TechTalk.SpecFlow;
using Microsoft.Playwright;
using NUnit.Framework;
using FluentAssertions;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SampleWebApp.Tests.Helpers;

namespace SampleWebApp.Tests.BDDTests.StepDefinitions;

[Binding]
[AllureNUnit]
[AllureSuite("BDD Tests")]
public class TodoManagementSteps
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    private string _appUrl = string.Empty;

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        await AllureHelper.StepAsync("Initialize browser for scenario", async () =>
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });

            var videoDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "videos");
            _context = await _browser.NewContextAsync(new()
            {
                RecordVideoDir = videoDir,
                RecordVideoSize = new() { Width = 1280, Height = 720 }
            });

            _page = await _context.NewPageAsync();

            var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.FullName;
            var htmlPath = Path.Combine(projectRoot, "SampleWebApp", "index.html");
            _appUrl = $"file:///{htmlPath.Replace("\\", "/")}";
        });
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_page != null)
        {
            await AllureHelper.AttachScreenshot(_page, "Scenario End State");

            var videoPath = await _page.Video?.PathAsync();
            await _page.CloseAsync();

            if (videoPath != null)
            {
                await AllureHelper.AttachVideo(videoPath, "Scenario Execution Video");
            }
        }

        if (_context != null) await _context.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    [Given(@"the todo application is loaded")]
    public async Task GivenTheTodoApplicationIsLoaded()
    {
        await AllureHelper.StepAsync("Load todo application", async () =>
        {
            await _page!.GotoAsync(_appUrl);
            await AllureHelper.AttachScreenshot(_page, "Application loaded");
        });
    }

    [Given(@"the todo list is empty")]
    public async Task GivenTheTodoListIsEmpty()
    {
        await AllureHelper.StepAsync("Clear localStorage and reload", async () =>
        {
            await _page!.EvaluateAsync("localStorage.clear()");
            await _page.ReloadAsync();
            await AllureHelper.AttachScreenshot(_page, "Empty todo list");
        });
    }

    [Given(@"I have added a todo ""(.*)""")]
    public async Task GivenIHaveAddedATodo(string todoText)
    {
        await AllureHelper.StepAsync($"Add todo: '{todoText}'", async () =>
        {
            await _page!.FillAsync("#todoInput", todoText);
            await _page.ClickAsync("#addBtn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(_page, $"Todo '{todoText}' added");
        });
    }

    [Given(@"I have added the following todos:")]
    public async Task GivenIHaveAddedTheFollowingTodos(Table table)
    {
        await AllureHelper.StepAsync("Add multiple todos from table", async () =>
        {
            foreach (var row in table.Rows)
            {
                var text = row["Text"];
                var completed = bool.Parse(row["Completed"]);

                await _page!.FillAsync("#todoInput", text);
                await _page.ClickAsync("#addBtn");
                await Task.Delay(50);

                if (completed)
                {
                    var checkbox = _page.Locator($".todo-item span:text('{text}')").Locator("..").Locator("input[type='checkbox']");
                    await checkbox.CheckAsync();
                    await Task.Delay(50);
                }
            }
            await AllureHelper.AttachScreenshot(_page, "Multiple todos added");
            AllureHelper.AttachJson("Todos Data", table.Rows.Select(r => new { Text = r["Text"], Completed = r["Completed"] }));
        });
    }

    [When(@"I add a todo with text ""(.*)""")]
    public async Task WhenIAddATodoWithText(string todoText)
    {
        await AllureHelper.StepAsync($"Add todo with text: '{todoText}'", async () =>
        {
            await _page!.FillAsync("#todoInput", todoText);
            await AllureHelper.AttachScreenshot(_page, "Text entered");
            await _page.ClickAsync("#addBtn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(_page, "Todo added");
        });
    }

    [When(@"I mark the todo ""(.*)"" as completed")]
    public async Task WhenIMarkTheTodoAsCompleted(string todoText)
    {
        await AllureHelper.StepAsync($"Mark todo '{todoText}' as completed", async () =>
        {
            var checkbox = _page!.Locator($".todo-item span:text('{todoText}')").Locator("..").Locator("input[type='checkbox']");
            await checkbox.CheckAsync();
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(_page, $"Todo '{todoText}' marked as completed");
        });
    }

    [When(@"I delete the todo ""(.*)""")]
    public async Task WhenIDeleteTheTodo(string todoText)
    {
        await AllureHelper.StepAsync($"Delete todo: '{todoText}'", async () =>
        {
            var deleteButton = _page!.Locator($".todo-item span:text('{todoText}')").Locator("..").Locator("button");
            await deleteButton.ClickAsync();
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(_page, $"Todo '{todoText}' deleted");
        });
    }

    [When(@"I filter by ""(.*)""")]
    public async Task WhenIFilterBy(string filter)
    {
        await AllureHelper.StepAsync($"Apply filter: '{filter}'", async () =>
        {
            await _page!.ClickAsync($"button:text('{filter}')");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(_page, $"Filter '{filter}' applied");
        });
    }

    [When(@"I clear completed todos")]
    public async Task WhenIClearCompletedTodos()
    {
        await AllureHelper.StepAsync("Clear completed todos", async () =>
        {
            await _page!.ClickAsync("#clearCompleted");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(_page, "Completed todos cleared");
        });
    }

    [Then(@"the todo ""(.*)"" should be displayed in the list")]
    public async Task ThenTheTodoShouldBeDisplayedInTheList(string todoText)
    {
        await AllureHelper.StepAsync($"Verify todo '{todoText}' is displayed", async () =>
        {
            var todo = _page!.Locator($".todo-item span:text('{todoText}')");
            var count = await todo.CountAsync();
            count.Should().BeGreaterThan(0);
            await AllureHelper.AttachScreenshot(_page, $"Todo '{todoText}' is displayed");
        });
    }

    [Then(@"the todo ""(.*)"" should be marked as completed")]
    public async Task ThenTheTodoShouldBeMarkedAsCompleted(string todoText)
    {
        await AllureHelper.StepAsync($"Verify todo '{todoText}' is marked as completed", async () =>
        {
            var todoItem = _page!.Locator($".todo-item span:text('{todoText}')").Locator("..");
            var className = await todoItem.GetAttributeAsync("class");
            className.Should().Contain("completed");
            await AllureHelper.AttachScreenshot(_page, $"Todo '{todoText}' is completed");
        });
    }

    [Then(@"the todo ""(.*)"" should not be in the list")]
    public async Task ThenTheTodoShouldNotBeInTheList(string todoText)
    {
        await AllureHelper.StepAsync($"Verify todo '{todoText}' is not in the list", async () =>
        {
            var todo = _page!.Locator($".todo-item span:text('{todoText}')");
            var count = await todo.CountAsync();
            count.Should().Be(0);
            await AllureHelper.AttachScreenshot(_page, $"Todo '{todoText}' is not in list");
        });
    }

    [Then(@"the remaining tasks count should be (.*)")]
    public async Task ThenTheRemainingTasksCountShouldBe(int expectedCount)
    {
        await AllureHelper.StepAsync($"Verify remaining tasks count is {expectedCount}", async () =>
        {
            var counterText = await _page!.Locator("#todoCount").TextContentAsync();
            counterText.Should().Contain($"{expectedCount} task");
            await AllureHelper.AttachScreenshot(_page, $"Task count: {expectedCount}");
            AllureHelper.AttachText("Counter Text", counterText ?? "null");
        });
    }

    [Then(@"I should see (.*) todos in the list")]
    public async Task ThenIShouldSeeTodosInTheList(int expectedCount)
    {
        await AllureHelper.StepAsync($"Verify {expectedCount} todos are visible", async () =>
        {
            var count = await _page!.Locator(".todo-item").CountAsync();
            count.Should().Be(expectedCount);
            await AllureHelper.AttachScreenshot(_page, $"Visible todos: {expectedCount}");
        });
    }

    [Then(@"all displayed todos should be active")]
    public async Task ThenAllDisplayedTodosShouldBeActive()
    {
        await AllureHelper.StepAsync("Verify all displayed todos are active", async () =>
        {
            var completedCount = await _page!.Locator(".todo-item.completed").CountAsync();
            completedCount.Should().Be(0);
            await AllureHelper.AttachScreenshot(_page, "All todos are active");
        });
    }

    [Then(@"all displayed todos should be completed")]
    public async Task ThenAllDisplayedTodosShouldBeCompleted()
    {
        await AllureHelper.StepAsync("Verify all displayed todos are completed", async () =>
        {
            var totalCount = await _page!.Locator(".todo-item").CountAsync();
            var completedCount = await _page.Locator(".todo-item.completed").CountAsync();
            completedCount.Should().Be(totalCount);
            await AllureHelper.AttachScreenshot(_page, "All todos are completed");
        });
    }

    [Then(@"the todo count should be (.*)")]
    public async Task ThenTheTodoCountShouldBe(int expectedCount)
    {
        await AllureHelper.StepAsync($"Verify total todo count is {expectedCount}", async () =>
        {
            var count = await _page!.Locator(".todo-item").CountAsync();
            count.Should().Be(expectedCount);
            await AllureHelper.AttachScreenshot(_page, $"Total todos: {expectedCount}");
        });
    }
}
