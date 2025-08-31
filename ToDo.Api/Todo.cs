namespace ToDo.Api;

public class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    public DateOnly? DueDate { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public List<string> Tags { get; set; } = new List<string>();

    // Convenience methods using extension methods
    public void AddTag(string tag)
    {
        Tags.AddTag(tag);
    }

    public void RemoveTag(string tag)
    {
        Tags.RemoveTag(tag);
    }

    public bool HasTag(string tag)
    {
        return Tags.HasTag(tag);
    }
}
