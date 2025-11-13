namespace SampleWebApp.Tests.UnitTests;

public class TodoService
{
    private List<TodoItem> _todos = new();

    public IReadOnlyList<TodoItem> GetTodos() => _todos.AsReadOnly();

    public void AddTodo(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Todo text cannot be empty", nameof(text));

        _todos.Add(new TodoItem
        {
            Id = _todos.Count > 0 ? _todos.Max(t => t.Id) + 1 : 1,
            Text = text,
            IsCompleted = false
        });
    }

    public void ToggleTodo(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
            throw new InvalidOperationException($"Todo with id {id} not found");

        todo.IsCompleted = !todo.IsCompleted;
    }

    public void DeleteTodo(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo == null)
            throw new InvalidOperationException($"Todo with id {id} not found");

        _todos.Remove(todo);
    }

    public void ClearCompleted()
    {
        _todos.RemoveAll(t => t.IsCompleted);
    }

    public IReadOnlyList<TodoItem> GetActiveTodos()
    {
        return _todos.Where(t => !t.IsCompleted).ToList().AsReadOnly();
    }

    public IReadOnlyList<TodoItem> GetCompletedTodos()
    {
        return _todos.Where(t => t.IsCompleted).ToList().AsReadOnly();
    }

    public int GetActiveCount()
    {
        return _todos.Count(t => !t.IsCompleted);
    }
}

public class TodoItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
