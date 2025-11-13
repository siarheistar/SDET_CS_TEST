using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using FluentAssertions;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using SampleWebApp.Tests.Helpers;

namespace SampleWebApp.Tests.UITests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
[AllureNUnit]
[AllureSuite("UI Tests")]
public class TodoUITests : PageTest
{
    private string _appUrl = string.Empty;

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            RecordVideoDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "videos"),
            RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
        };
    }

    [SetUp]
    public async Task Setup()
    {
        await AllureHelper.StepAsync("Setup test environment", async () =>
        {
            var projectRoot = Directory.GetParent(TestContext.CurrentContext.TestDirectory)!.Parent!.Parent!.Parent!.FullName;
            var htmlPath = Path.Combine(projectRoot, "SampleWebApp", "index.html");
            _appUrl = $"file:///{htmlPath.Replace("\\", "/")}";

            await Page.GotoAsync(_appUrl);
            await Page.EvaluateAsync("localStorage.clear()");
            await Page.ReloadAsync();
            await AllureHelper.AttachScreenshot(Page, "Initial Page State");
        });
    }

    [TearDown]
    public async Task TearDown()
    {
        await AllureHelper.AttachScreenshot(Page, "Final Page State");

        // Attach video if available
        var videoPath = await Page.Video?.PathAsync();
        if (videoPath != null)
        {
            await Page.CloseAsync();
            await AllureHelper.AttachVideo(videoPath, "Test Execution Video");
        }
    }

    [Test]
    [AllureDescription("Verify that the Todo application displays the correct title on page load")]
    public async Task PageLoad_ShouldDisplayTodoAppTitle()
    {
        var title = await AllureHelper.StepAsync("Get page title", async () =>
        {
            return await Page.Locator("h1").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify title is 'Todo List'", async () =>
        {
            title.Should().Be("Todo List");
            await AllureHelper.AttachScreenshot(Page, "Page Title Verification");
        });
    }

    [Test]
    [AllureDescription("Verify that adding a todo with valid text displays it in the list")]
    public async Task AddTodo_WithValidText_ShouldDisplayInList()
    {
        await AllureHelper.StepAsync("Enter todo text 'Buy groceries'", async () =>
        {
            await Page.FillAsync("#todoInput", "Buy groceries");
            await AllureHelper.AttachScreenshot(Page, "After entering text");
        });

        await AllureHelper.StepAsync("Click Add button", async () =>
        {
            await Page.ClickAsync("#addBtn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After clicking Add");
        });

        var todoText = await AllureHelper.StepAsync("Get todo item text", async () =>
        {
            return await Page.Locator(".todo-item span").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify todo text is 'Buy groceries'", async () =>
        {
            todoText.Should().Be("Buy groceries");
        });
    }

    [Test]
    [AllureDescription("Verify that adding a todo with empty text does not add it to the list")]
    public async Task AddTodo_WithEmptyText_ShouldNotAddTodo()
    {
        await AllureHelper.StepAsync("Click Add button without entering text", async () =>
        {
            await Page.ClickAsync("#addBtn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After clicking Add with empty input");
        });

        var todoCount = await AllureHelper.StepAsync("Count todo items", async () =>
        {
            return await Page.Locator(".todo-item").CountAsync();
        });

        await AllureHelper.StepAsync("Verify todo count is 0", async () =>
        {
            todoCount.Should().Be(0);
        });
    }

    [Test]
    [AllureDescription("Verify that pressing Enter key adds a todo")]
    public async Task AddTodo_PressingEnter_ShouldAddTodo()
    {
        await AllureHelper.StepAsync("Enter todo text 'Test task'", async () =>
        {
            await Page.FillAsync("#todoInput", "Test task");
        });

        await AllureHelper.StepAsync("Press Enter key", async () =>
        {
            await Page.PressAsync("#todoInput", "Enter");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After pressing Enter");
        });

        var todoCount = await AllureHelper.StepAsync("Count todo items", async () =>
        {
            return await Page.Locator(".todo-item").CountAsync();
        });

        await AllureHelper.StepAsync("Verify todo count is 1", async () =>
        {
            todoCount.Should().Be(1);
        });
    }

    [Test]
    [AllureDescription("Verify that the input field is cleared after adding a todo")]
    public async Task AddTodo_ShouldClearInputField()
    {
        await AllureHelper.StepAsync("Enter and add todo", async () =>
        {
            await Page.FillAsync("#todoInput", "Test task");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(100);
        });

        var inputValue = await AllureHelper.StepAsync("Get input field value", async () =>
        {
            return await Page.InputValueAsync("#todoInput");
        });

        await AllureHelper.StepAsync("Verify input is empty", async () =>
        {
            inputValue.Should().BeEmpty();
            await AllureHelper.AttachScreenshot(Page, "Empty input field after add");
        });
    }

    [Test]
    [AllureDescription("Verify that toggling a todo marks it as completed")]
    public async Task ToggleTodo_ShouldMarkAsCompleted()
    {
        await AllureHelper.StepAsync("Add a todo 'Complete me'", async () =>
        {
            await Page.FillAsync("#todoInput", "Complete me");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "Todo added");
        });

        await AllureHelper.StepAsync("Click todo checkbox", async () =>
        {
            await Page.ClickAsync(".todo-item input[type='checkbox']");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After toggling checkbox");
        });

        var className = await AllureHelper.StepAsync("Get todo item class name", async () =>
        {
            return await Page.Locator(".todo-item").GetAttributeAsync("class");
        });

        await AllureHelper.StepAsync("Verify todo has 'completed' class", async () =>
        {
            className.Should().Contain("completed");
        });
    }

    [Test]
    [AllureDescription("Verify that deleting a todo removes it from the list")]
    public async Task DeleteTodo_ShouldRemoveFromList()
    {
        await AllureHelper.StepAsync("Add a todo 'Delete me'", async () =>
        {
            await Page.FillAsync("#todoInput", "Delete me");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "Todo added");
        });

        await AllureHelper.StepAsync("Click delete button", async () =>
        {
            await Page.ClickAsync(".todo-item .delete-btn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After clicking delete");
        });

        var todoCount = await AllureHelper.StepAsync("Count remaining todos", async () =>
        {
            return await Page.Locator(".todo-item").CountAsync();
        });

        await AllureHelper.StepAsync("Verify todo count is 0", async () =>
        {
            todoCount.Should().Be(0);
        });
    }

    [Test]
    [AllureDescription("Verify that the todo counter displays the correct count")]
    public async Task TodoCounter_ShouldDisplayCorrectCount()
    {
        await AllureHelper.StepAsync("Add two todos", async () =>
        {
            await Page.FillAsync("#todoInput", "Task 1");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            await Page.FillAsync("#todoInput", "Task 2");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);
            await AllureHelper.AttachScreenshot(Page, "Two todos added");
        });

        var counterText = await AllureHelper.StepAsync("Get counter text", async () =>
        {
            return await Page.Locator("#todoCount").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify counter shows '2 tasks left'", async () =>
        {
            counterText.Should().Contain("2 tasks left");
        });
    }

    [Test]
    [AllureDescription("Verify that the todo counter decreases after completing a todo")]
    public async Task TodoCounter_AfterCompleting_ShouldDecrease()
    {
        await AllureHelper.StepAsync("Add two todos", async () =>
        {
            await Page.FillAsync("#todoInput", "Task 1");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            await Page.FillAsync("#todoInput", "Task 2");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);
        });

        await AllureHelper.StepAsync("Complete first todo", async () =>
        {
            var firstCheckbox = Page.Locator(".todo-item input[type='checkbox']").First;
            await firstCheckbox.CheckAsync();
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After completing first todo");
        });

        var counterText = await AllureHelper.StepAsync("Get counter text", async () =>
        {
            return await Page.Locator("#todoCount").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify counter shows '1 task left'", async () =>
        {
            counterText.Should().Contain("1 task left");
        });
    }

    [Test]
    [AllureDescription("Verify that the Active filter shows only active todos")]
    public async Task FilterActive_ShouldShowOnlyActiveTodos()
    {
        await AllureHelper.StepAsync("Add one active and one completed todo", async () =>
        {
            await Page.FillAsync("#todoInput", "Active task");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            await Page.FillAsync("#todoInput", "Completed task");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            var lastCheckbox = Page.Locator(".todo-item input[type='checkbox']").Last;
            await lastCheckbox.CheckAsync();
            await Task.Delay(50);
            await AllureHelper.AttachScreenshot(Page, "Before filtering");
        });

        await AllureHelper.StepAsync("Click Active filter", async () =>
        {
            await Page.ClickAsync("button:text('Active')");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After applying Active filter");
        });

        var visibleTodos = await AllureHelper.StepAsync("Count visible todos", async () =>
        {
            return await Page.Locator(".todo-item").CountAsync();
        });

        var todoText = await AllureHelper.StepAsync("Get visible todo text", async () =>
        {
            return await Page.Locator(".todo-item span").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify only one active todo is visible", async () =>
        {
            visibleTodos.Should().Be(1);
            todoText.Should().Be("Active task");
        });
    }

    [Test]
    [AllureDescription("Verify that the Completed filter shows only completed todos")]
    public async Task FilterCompleted_ShouldShowOnlyCompletedTodos()
    {
        await AllureHelper.StepAsync("Add one active and one completed todo", async () =>
        {
            await Page.FillAsync("#todoInput", "Active task");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            await Page.FillAsync("#todoInput", "Completed task");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            var lastCheckbox = Page.Locator(".todo-item input[type='checkbox']").Last;
            await lastCheckbox.CheckAsync();
            await Task.Delay(50);
            await AllureHelper.AttachScreenshot(Page, "Before filtering");
        });

        await AllureHelper.StepAsync("Click Completed filter", async () =>
        {
            await Page.ClickAsync("button:text('Completed')");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After applying Completed filter");
        });

        var visibleTodos = await AllureHelper.StepAsync("Count visible todos", async () =>
        {
            return await Page.Locator(".todo-item").CountAsync();
        });

        var todoText = await AllureHelper.StepAsync("Get visible todo text", async () =>
        {
            return await Page.Locator(".todo-item span").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify only one completed todo is visible", async () =>
        {
            visibleTodos.Should().Be(1);
            todoText.Should().Be("Completed task");
        });
    }

    [Test]
    [AllureDescription("Verify that Clear Completed removes only completed todos")]
    public async Task ClearCompleted_ShouldRemoveCompletedTodos()
    {
        await AllureHelper.StepAsync("Add one active and two completed todos", async () =>
        {
            await Page.FillAsync("#todoInput", "Active task");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            await Page.FillAsync("#todoInput", "Completed task 1");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            await Page.FillAsync("#todoInput", "Completed task 2");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(50);

            var checkboxes = Page.Locator(".todo-item input[type='checkbox']");
            await checkboxes.Nth(1).CheckAsync();
            await Task.Delay(50);
            await checkboxes.Nth(2).CheckAsync();
            await Task.Delay(50);
            await AllureHelper.AttachScreenshot(Page, "Before clearing completed");
        });

        await AllureHelper.StepAsync("Click Clear Completed button", async () =>
        {
            await Page.ClickAsync("#clearCompleted");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After clearing completed");
        });

        var remainingTodos = await AllureHelper.StepAsync("Count remaining todos", async () =>
        {
            return await Page.Locator(".todo-item").CountAsync();
        });

        var todoText = await AllureHelper.StepAsync("Get remaining todo text", async () =>
        {
            return await Page.Locator(".todo-item span").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify only active todo remains", async () =>
        {
            remainingTodos.Should().Be(1);
            todoText.Should().Be("Active task");
        });
    }

    [Test]
    [AllureDescription("Verify that todos persist in localStorage after page reload")]
    public async Task LocalStorage_ShouldPersistTodos()
    {
        await AllureHelper.StepAsync("Add a todo 'Persistent task'", async () =>
        {
            await Page.FillAsync("#todoInput", "Persistent task");
            await Page.ClickAsync("#addBtn");
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "Before reload");
        });

        await AllureHelper.StepAsync("Reload the page", async () =>
        {
            await Page.ReloadAsync();
            await Task.Delay(100);
            await AllureHelper.AttachScreenshot(Page, "After reload");
        });

        var todoCount = await AllureHelper.StepAsync("Count todos after reload", async () =>
        {
            return await Page.Locator(".todo-item").CountAsync();
        });

        var todoText = await AllureHelper.StepAsync("Get todo text after reload", async () =>
        {
            return await Page.Locator(".todo-item span").TextContentAsync();
        });

        await AllureHelper.StepAsync("Verify todo persisted", async () =>
        {
            todoCount.Should().Be(1);
            todoText.Should().Be("Persistent task");
        });
    }
}
