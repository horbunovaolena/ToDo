using Microsoft.EntityFrameworkCore;
using ToDo.Api;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable static files
app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowAll");

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve index.html as default page
app.MapGet("/", () => Results.Redirect("/index.html"));

// Get all unique tags
app.MapGet("/tags", async (TodoDb db) =>
{
    var allTags = await db.Todos
        .SelectMany(t => t.Tags)
        .Distinct()
        .OrderBy(tag => tag)
        .ToListAsync();
    return Results.Ok(allTags);
});

// Get todos by tag
app.MapGet("/todoitems/tag/{tag}", async (string tag, TodoDb db) =>
{
    var normalizedTag = tag.Trim().ToLowerInvariant();
    var todos = await db.Todos
        .Where(t => t.Tags.Contains(normalizedTag))
        .ToListAsync();
    return Results.Ok(todos);
});

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete == true).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;
    todo.Description = inputTodo.Description;
    todo.DueDate = inputTodo.DueDate;
    todo.Priority = inputTodo.Priority;
    todo.Tags = inputTodo.Tags;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();