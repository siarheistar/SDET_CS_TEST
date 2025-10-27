using NUnit.Framework;
using FluentAssertions;
using Allure.NUnit;
using Allure.NUnit.Attributes;

namespace SampleWebApp.Tests.UnitTests;

[TestFixture]
[AllureNUnit]
[AllureSuite("Unit Tests")]
public class TodoAppUnitTests
{
    private TodoService _todoService = null!;

    [SetUp]
    public void Setup()
    {
        _todoService = new TodoService();
    }

    [Test]
    public void AddTodo_WithValidText_ShouldAddTodoToList()
    {
        // Arrange
        var todoText = "Buy groceries";

        // Act
        _todoService.AddTodo(todoText);

        // Assert
        var todos = _todoService.GetTodos();
        todos.Should().HaveCount(1);
        todos[0].Text.Should().Be(todoText);
        todos[0].IsCompleted.Should().BeFalse();
    }

    [Test]
    public void AddTodo_WithEmptyText_ShouldThrowException()
    {
        // Act & Assert
        Action act = () => _todoService.AddTodo("");
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be empty*");
    }

    [Test]
    public void AddTodo_WithWhitespaceText_ShouldThrowException()
    {
        // Act & Assert
        Action act = () => _todoService.AddTodo("   ");
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddTodo_MultipleTodos_ShouldAssignUniqueIds()
    {
        // Act
        _todoService.AddTodo("Task 1");
        _todoService.AddTodo("Task 2");
        _todoService.AddTodo("Task 3");

        // Assert
        var todos = _todoService.GetTodos();
        todos.Should().HaveCount(3);
        todos.Select(t => t.Id).Should().OnlyHaveUniqueItems();
    }

    [Test]
    public void ToggleTodo_WithValidId_ShouldToggleCompletionStatus()
    {
        // Arrange
        _todoService.AddTodo("Test task");
        var todoId = _todoService.GetTodos()[0].Id;

        // Act
        _todoService.ToggleTodo(todoId);

        // Assert
        _todoService.GetTodos()[0].IsCompleted.Should().BeTrue();

        // Act again
        _todoService.ToggleTodo(todoId);

        // Assert
        _todoService.GetTodos()[0].IsCompleted.Should().BeFalse();
    }

    [Test]
    public void ToggleTodo_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        Action act = () => _todoService.ToggleTodo(999);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Test]
    public void DeleteTodo_WithValidId_ShouldRemoveTodo()
    {
        // Arrange
        _todoService.AddTodo("Task to delete");
        var todoId = _todoService.GetTodos()[0].Id;

        // Act
        _todoService.DeleteTodo(todoId);

        // Assert
        _todoService.GetTodos().Should().BeEmpty();
    }

    [Test]
    public void DeleteTodo_WithInvalidId_ShouldThrowException()
    {
        // Act & Assert
        Action act = () => _todoService.DeleteTodo(999);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Test]
    public void ClearCompleted_ShouldRemoveOnlyCompletedTodos()
    {
        // Arrange
        _todoService.AddTodo("Active task");
        _todoService.AddTodo("Completed task 1");
        _todoService.AddTodo("Completed task 2");

        var todos = _todoService.GetTodos();
        _todoService.ToggleTodo(todos[1].Id);
        _todoService.ToggleTodo(todos[2].Id);

        // Act
        _todoService.ClearCompleted();

        // Assert
        var remaining = _todoService.GetTodos();
        remaining.Should().HaveCount(1);
        remaining[0].Text.Should().Be("Active task");
    }

    [Test]
    public void GetActiveTodos_ShouldReturnOnlyActiveTodos()
    {
        // Arrange
        _todoService.AddTodo("Active task");
        _todoService.AddTodo("Completed task");

        var todos = _todoService.GetTodos();
        _todoService.ToggleTodo(todos[1].Id);

        // Act
        var activeTodos = _todoService.GetActiveTodos();

        // Assert
        activeTodos.Should().HaveCount(1);
        activeTodos[0].Text.Should().Be("Active task");
    }

    [Test]
    public void GetCompletedTodos_ShouldReturnOnlyCompletedTodos()
    {
        // Arrange
        _todoService.AddTodo("Active task");
        _todoService.AddTodo("Completed task");

        var todos = _todoService.GetTodos();
        _todoService.ToggleTodo(todos[1].Id);

        // Act
        var completedTodos = _todoService.GetCompletedTodos();

        // Assert
        completedTodos.Should().HaveCount(1);
        completedTodos[0].Text.Should().Be("Completed task");
    }

    [Test]
    public void GetActiveCount_ShouldReturnCorrectCount()
    {
        // Arrange
        _todoService.AddTodo("Active task 1");
        _todoService.AddTodo("Completed task");
        _todoService.AddTodo("Active task 2");

        var todos = _todoService.GetTodos();
        _todoService.ToggleTodo(todos[1].Id);

        // Act
        var activeCount = _todoService.GetActiveCount();

        // Assert
        activeCount.Should().Be(2);
    }
}
