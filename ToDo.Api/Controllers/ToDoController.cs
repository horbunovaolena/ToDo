using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly TodoDb _db;

        public ToDoController(TodoDb db)
        {
            _db = db;
        }

        [HttpGet("/todoitems")]
        public async Task<ActionResult<List<Todo>>> GetAllTodos()
        {
            return await _db.Todos.ToListAsync();
        }

        [HttpGet("/todoitems/complete")]
        public async Task<ActionResult<List<Todo>>> GetCompleteTodos()
        {
            var completedTodos = await _db.Todos.Where(t => t.IsComplete).ToListAsync();
            return Ok(completedTodos);
        }

        [HttpGet("/todoitems/{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpGet("/todoitems/tag/{tag}")]
        public async Task<ActionResult<List<Todo>>> GetTodosByTag(string tag)
        {
            var normalizedTag = tag.Trim().ToLowerInvariant();
            var todos = await _db.Todos.ToListAsync();
            var filteredTodos = todos
                .Where(t => t.Tags != null && t.Tags.Any(todoTag => todoTag.Equals(normalizedTag, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return Ok(filteredTodos);
        }

        [HttpPost("/todoitems")]
        public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
        {
            _db.Todos.Add(todo);
            await _db.SaveChangesAsync();
            return Created($"/todoitems/{todo.Id}", todo);
        }

        [HttpPut("/todoitems/{id}")]
        public async Task<IActionResult> UpdateTodo(int id, Todo inputTodo)
        {
            var todo = await _db.Todos.FindAsync(id);

            if (todo is null) 
                return NotFound();

            todo.Name = inputTodo.Name;
            todo.IsComplete = inputTodo.IsComplete;
            todo.Description = inputTodo.Description;
            todo.DueDate = inputTodo.DueDate;
            todo.Priority = inputTodo.Priority;
            todo.Tags = inputTodo.Tags;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("/todoitems/{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("/tags")]
        public async Task<ActionResult<List<string>>> GetAllTags()
        {
            var todos = await _db.Todos.ToListAsync();
            var allTags = todos
                .SelectMany(t => t.Tags ?? new List<string>())
                .Distinct()
                .OrderBy(tag => tag)
                .ToList();
            return Ok(allTags);
        }
    }
}