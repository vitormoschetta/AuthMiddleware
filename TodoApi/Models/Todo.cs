namespace TodoApi.Models;
public class Todo
{
    public Todo()
    {
        Id = Guid.NewGuid();
    }

    public Todo(string title)
    {
        Id = Guid.NewGuid();
        Title = title;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public bool Done { get; private set; }

    public void Update(string title, bool done = false)
    {
        Title = title;
        Done = done;
    }
}