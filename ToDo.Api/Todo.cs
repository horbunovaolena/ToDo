namespace ToDo.Api
{
    public class Todo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
        public DateOnly? DueDate { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
    }
}
