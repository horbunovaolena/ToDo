using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Api.Controllers;

[ApiController]
[Route("todoitems")]
public class ToDoController(TodoDb _db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Todo>>> GetAllTodos()
    {
        List<Todo> todos = await _db.Todos.ToListAsync();
        
        return Ok(todos);
    }

    [HttpGet("complete")]
    public async Task<ActionResult<List<Todo>>> GetCompleteTodos()
    {
        List<Todo> completedTodos = await _db.Todos
            .Where(t => t.IsComplete)
            .ToListAsync();

        return Ok(completedTodos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetTodo(int id)
    {
        Todo? todo = await _db.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
    {
        _db.Todos.Add(todo);
        
        await _db.SaveChangesAsync();
        
        return Created($"/api/todoitems/{todo.Id}", todo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, Todo inputTodo)
    {
        Todo? todo = await _db.Todos.FindAsync(id);

        if (todo is null)
        {
            return NotFound();
        }

        todo.Name = inputTodo.Name;
        todo.IsComplete = inputTodo.IsComplete;
        todo.Description = inputTodo.Description;
        todo.DueDate = inputTodo.DueDate;
        todo.Priority = inputTodo.Priority;
        todo.Tags = inputTodo.Tags;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        Todo? todo = await _db.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}