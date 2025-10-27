using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using FluentAssertions;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace SampleWebApp.Tests.UITests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
[AllureNUnit]
[AllureSuite("UI Tests")]
public class TodoUITests : PageTest
{
    private string _appUrl = string.Empty;

    [SetUp]
    public async Task Setup()
    {
        var projectRoot = Directory.GetParent(TestContext.CurrentContext.TestDirectory)!.Parent!.Parent!.Parent!.FullName;
        var htmlPath = Path.Combine(projectRoot, "SampleWebApp", "index.html");
        _appUrl = $"file:///{htmlPath.Replace("\\", "/")}";

        await Page.GotoAsync(_appUrl);
        await Page.EvaluateAsync("localStorage.clear()");
        await Page.ReloadAsync();
    }

    [Test]
    public async Task PageLoad_ShouldDisplayTodoAppTitle()
    {
        // Assert
        var title = await Page.Locator("h1").TextContentAsync();
        title.Should().Be("Todo List");
    }

    [Test]
    public async Task AddTodo_WithValidText_ShouldDisplayInList()
    {
        // Act
        await Page.FillAsync("#todoInput", "Buy groceries");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(100);

        // Assert
        var todoText = await Page.Locator(".todo-item span").TextContentAsync();
        todoText.Should().Be("Buy groceries");
    }

    [Test]
    public async Task AddTodo_WithEmptyText_ShouldNotAddTodo()
    {
        // Act
        await Page.ClickAsync("#addBtn");
        await Task.Delay(100);

        // Assert
        var todoCount = await Page.Locator(".todo-item").CountAsync();
        todoCount.Should().Be(0);
    }

    [Test]
    public async Task AddTodo_PressingEnter_ShouldAddTodo()
    {
        // Act
        await Page.FillAsync("#todoInput", "Test task");
        await Page.PressAsync("#todoInput", "Enter");
        await Task.Delay(100);

        // Assert
        var todoCount = await Page.Locator(".todo-item").CountAsync();
        todoCount.Should().Be(1);
    }

    [Test]
    public async Task AddTodo_ShouldClearInputField()
    {
        // Act
        await Page.FillAsync("#todoInput", "Test task");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(100);

        // Assert
        var inputValue = await Page.InputValueAsync("#todoInput");
        inputValue.Should().BeEmpty();
    }

    [Test]
    public async Task ToggleTodo_ShouldMarkAsCompleted()
    {
        // Arrange
        await Page.FillAsync("#todoInput", "Complete me");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(100);

        // Act
        await Page.ClickAsync(".todo-item input[type='checkbox']");
        await Task.Delay(100);

        // Assert
        var className = await Page.Locator(".todo-item").GetAttributeAsync("class");
        className.Should().Contain("completed");
    }

    [Test]
    public async Task DeleteTodo_ShouldRemoveFromList()
    {
        // Arrange
        await Page.FillAsync("#todoInput", "Delete me");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(100);

        // Act
        await Page.ClickAsync(".todo-item .delete-btn");
        await Task.Delay(100);

        // Assert
        var todoCount = await Page.Locator(".todo-item").CountAsync();
        todoCount.Should().Be(0);
    }

    [Test]
    public async Task TodoCounter_ShouldDisplayCorrectCount()
    {
        // Act
        await Page.FillAsync("#todoInput", "Task 1");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        await Page.FillAsync("#todoInput", "Task 2");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        // Assert
        var counterText = await Page.Locator("#todoCount").TextContentAsync();
        counterText.Should().Contain("2 tasks left");
    }

    [Test]
    public async Task TodoCounter_AfterCompleting_ShouldDecrease()
    {
        // Arrange
        await Page.FillAsync("#todoInput", "Task 1");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        await Page.FillAsync("#todoInput", "Task 2");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        // Act
        var firstCheckbox = Page.Locator(".todo-item input[type='checkbox']").First;
        await firstCheckbox.CheckAsync();
        await Task.Delay(100);

        // Assert
        var counterText = await Page.Locator("#todoCount").TextContentAsync();
        counterText.Should().Contain("1 task left");
    }

    [Test]
    public async Task FilterActive_ShouldShowOnlyActiveTodos()
    {
        // Arrange
        await Page.FillAsync("#todoInput", "Active task");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        await Page.FillAsync("#todoInput", "Completed task");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        var lastCheckbox = Page.Locator(".todo-item input[type='checkbox']").Last;
        await lastCheckbox.CheckAsync();
        await Task.Delay(50);

        // Act
        await Page.ClickAsync("button:text('Active')");
        await Task.Delay(100);

        // Assert
        var visibleTodos = await Page.Locator(".todo-item").CountAsync();
        visibleTodos.Should().Be(1);

        var todoText = await Page.Locator(".todo-item span").TextContentAsync();
        todoText.Should().Be("Active task");
    }

    [Test]
    public async Task FilterCompleted_ShouldShowOnlyCompletedTodos()
    {
        // Arrange
        await Page.FillAsync("#todoInput", "Active task");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        await Page.FillAsync("#todoInput", "Completed task");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        var lastCheckbox = Page.Locator(".todo-item input[type='checkbox']").Last;
        await lastCheckbox.CheckAsync();
        await Task.Delay(50);

        // Act
        await Page.ClickAsync("button:text('Completed')");
        await Task.Delay(100);

        // Assert
        var visibleTodos = await Page.Locator(".todo-item").CountAsync();
        visibleTodos.Should().Be(1);

        var todoText = await Page.Locator(".todo-item span").TextContentAsync();
        todoText.Should().Be("Completed task");
    }

    [Test]
    public async Task ClearCompleted_ShouldRemoveCompletedTodos()
    {
        // Arrange
        await Page.FillAsync("#todoInput", "Active task");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        await Page.FillAsync("#todoInput", "Completed task 1");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        await Page.FillAsync("#todoInput", "Completed task 2");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(50);

        // Complete two tasks
        var checkboxes = Page.Locator(".todo-item input[type='checkbox']");
        await checkboxes.Nth(1).CheckAsync();
        await Task.Delay(50);
        await checkboxes.Nth(2).CheckAsync();
        await Task.Delay(50);

        // Act
        await Page.ClickAsync("#clearCompleted");
        await Task.Delay(100);

        // Assert
        var remainingTodos = await Page.Locator(".todo-item").CountAsync();
        remainingTodos.Should().Be(1);

        var todoText = await Page.Locator(".todo-item span").TextContentAsync();
        todoText.Should().Be("Active task");
    }

    [Test]
    public async Task LocalStorage_ShouldPersistTodos()
    {
        // Arrange - Add todos
        await Page.FillAsync("#todoInput", "Persistent task");
        await Page.ClickAsync("#addBtn");
        await Task.Delay(100);

        // Act - Reload page
        await Page.ReloadAsync();
        await Task.Delay(100);

        // Assert - Todo should still be there
        var todoCount = await Page.Locator(".todo-item").CountAsync();
        todoCount.Should().Be(1);

        var todoText = await Page.Locator(".todo-item span").TextContentAsync();
        todoText.Should().Be("Persistent task");
    }
}
