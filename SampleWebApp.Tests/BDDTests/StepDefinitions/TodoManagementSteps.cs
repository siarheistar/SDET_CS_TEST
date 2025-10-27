using TechTalk.SpecFlow;
using Microsoft.Playwright;
using NUnit.Framework;
using FluentAssertions;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace SampleWebApp.Tests.BDDTests.StepDefinitions;

[Binding]
[AllureNUnit]
[AllureSuite("BDD Tests")]
public class TodoManagementSteps
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    private string _appUrl = string.Empty;

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
        _page = await _browser.NewPageAsync();

        var projectRoot = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.Parent!.FullName;
        var htmlPath = Path.Combine(projectRoot, "SampleWebApp", "index.html");
        _appUrl = $"file:///{htmlPath.Replace("\\", "/")}";
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_page != null) await _page.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }

    [Given(@"the todo application is loaded")]
    public async Task GivenTheTodoApplicationIsLoaded()
    {
        await _page!.GotoAsync(_appUrl);
    }

    [Given(@"the todo list is empty")]
    public async Task GivenTheTodoListIsEmpty()
    {
        await _page!.EvaluateAsync("localStorage.clear()");
        await _page.ReloadAsync();
    }

    [Given(@"I have added a todo ""(.*)""")]
    public async Task GivenIHaveAddedATodo(string todoText)
    {
        await _page!.FillAsync("#todoInput", todoText);
        await _page.ClickAsync("#addBtn");
        await Task.Delay(100); // Small delay for rendering
    }

    [Given(@"I have added the following todos:")]
    public async Task GivenIHaveAddedTheFollowingTodos(Table table)
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
    }

    [When(@"I add a todo with text ""(.*)""")]
    public async Task WhenIAddATodoWithText(string todoText)
    {
        await _page!.FillAsync("#todoInput", todoText);
        await _page.ClickAsync("#addBtn");
        await Task.Delay(100);
    }

    [When(@"I mark the todo ""(.*)"" as completed")]
    public async Task WhenIMarkTheTodoAsCompleted(string todoText)
    {
        var checkbox = _page!.Locator($".todo-item span:text('{todoText}')").Locator("..").Locator("input[type='checkbox']");
        await checkbox.CheckAsync();
        await Task.Delay(100);
    }

    [When(@"I delete the todo ""(.*)""")]
    public async Task WhenIDeleteTheTodo(string todoText)
    {
        var deleteButton = _page!.Locator($".todo-item span:text('{todoText}')").Locator("..").Locator("button");
        await deleteButton.ClickAsync();
        await Task.Delay(100);
    }

    [When(@"I filter by ""(.*)""")]
    public async Task WhenIFilterBy(string filter)
    {
        await _page!.ClickAsync($"button:text('{filter}')");
        await Task.Delay(100);
    }

    [When(@"I clear completed todos")]
    public async Task WhenIClearCompletedTodos()
    {
        await _page!.ClickAsync("#clearCompleted");
        await Task.Delay(100);
    }

    [Then(@"the todo ""(.*)"" should be displayed in the list")]
    public async Task ThenTheTodoShouldBeDisplayedInTheList(string todoText)
    {
        var todo = _page!.Locator($".todo-item span:text('{todoText}')");
        (await todo.CountAsync()).Should().BeGreaterThan(0);
    }

    [Then(@"the todo ""(.*)"" should be marked as completed")]
    public async Task ThenTheTodoShouldBeMarkedAsCompleted(string todoText)
    {
        var todoItem = _page!.Locator($".todo-item span:text('{todoText}')").Locator("..");
        var className = await todoItem.GetAttributeAsync("class");
        className.Should().Contain("completed");
    }

    [Then(@"the todo ""(.*)"" should not be in the list")]
    public async Task ThenTheTodoShouldNotBeInTheList(string todoText)
    {
        var todo = _page!.Locator($".todo-item span:text('{todoText}')");
        (await todo.CountAsync()).Should().Be(0);
    }

    [Then(@"the remaining tasks count should be (.*)")]
    public async Task ThenTheRemainingTasksCountShouldBe(int expectedCount)
    {
        var counterText = await _page!.Locator("#todoCount").TextContentAsync();
        counterText.Should().Contain($"{expectedCount} task");
    }

    [Then(@"I should see (.*) todos in the list")]
    public async Task ThenIShouldSeeTodosInTheList(int expectedCount)
    {
        var count = await _page!.Locator(".todo-item").CountAsync();
        count.Should().Be(expectedCount);
    }

    [Then(@"all displayed todos should be active")]
    public async Task ThenAllDisplayedTodosShouldBeActive()
    {
        var completedCount = await _page!.Locator(".todo-item.completed").CountAsync();
        completedCount.Should().Be(0);
    }

    [Then(@"all displayed todos should be completed")]
    public async Task ThenAllDisplayedTodosShouldBeCompleted()
    {
        var totalCount = await _page!.Locator(".todo-item").CountAsync();
        var completedCount = await _page.Locator(".todo-item.completed").CountAsync();
        completedCount.Should().Be(totalCount);
    }

    [Then(@"the todo count should be (.*)")]
    public async Task ThenTheTodoCountShouldBe(int expectedCount)
    {
        var count = await _page!.Locator(".todo-item").CountAsync();
        count.Should().Be(expectedCount);
    }
}