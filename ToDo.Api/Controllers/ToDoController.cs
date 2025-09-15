using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Api.Controllers;

/// <summary>
/// REST API контролер для управління завданнями ToDo.
/// Надає повний набір CRUD операцій для роботи з завданнями через HTTP endpoints.
/// </summary>
[ApiController]
[Route("todoitems")]
public class ToDoController(TodoDb db) : ControllerBase
{
    /// <summary>
    /// Отримує список завдань з підтримкою пагінації, сортування та фільтрації.
    /// </summary>
    /// <param name="pageNumber">Номер сторінки (за замовчуванням 1)</param>
    /// <param name="pageSize">Розмір сторінки (за замовчуванням 10)</param>
    /// <param name="sortBy">Поле для сортування (name, priority, duedate, iscomplete, createddate)</param>
    /// <param name="sortDirection">Напрямок сортування (asc або desc)</param>
    /// <param name="priority">Фільтр за пріоритетом</param>
    /// <param name="isComplete">Фільтр за статусом завершення</param>
    /// <param name="searchQuery">Пошуковий запит для назви та опису</param>
    /// <returns>
    /// ActionResult що містить paginated результат з метаданими.
    /// HTTP 200 OK з об'єктом що містить data, pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage.
    /// </returns>
    /// <remarks>
    /// <strong>Приклади використання:</strong>
    /// - GET /todoitems?pageNumber=1&pageSize=5 - пагінація
    /// - GET /todoitems?sortBy=priority&sortDirection=desc - сортування за пріоритетом
    /// - GET /todoitems?priority=3 - фільтрація високого пріоритету  
    /// - GET /todoitems?searchQuery=робота - пошук завданнь
    /// - GET /todoitems?pageNumber=2&pageSize=5&sortBy=duedate&sortDirection=asc&priority=3&searchQuery=проект - комбінація всіх параметрів
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<List<Todo>>> GetPaginatedToDos(string query = "", int pageNumber = 1, int pageSize = 10)
    {
   
        List<Todo> allTodos = await db.Todos.ToListAsync();

        // 🔍 FILTERING in `allTodos`
        //if умова
        //код виконується якщо умова істинна

        List<Todo> filteredTodos = new List<Todo>();

        foreach (Todo todo in allTodos)
        {
            if (todo.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                || todo.Description.Contains(query, StringComparison.InvariantCultureIgnoreCase))
            {
                filteredTodos.Add(todo);
            }
        }


        // 📄 PAGINATION
        // | pageNumber | pageSize | result
        // | 1          | 10       | пропустити 0 і вернути {pageSize} записів
        // | 2          | 10       | пропустити {pageSize} і вернути {pageSize}
        // | 3          | 10       | пропустити 2 * {pageSize} і вернути {pageSize}
        // | 4          | 10       | пропустити 3 * {pageSize} і вернути {pageSize}
        List<Todo> paginatedTODOs = filteredTodos
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(paginatedTODOs);

    }

    /// <summary>
    /// Отримує всі завдання без пагінації (для backward compatibility).
    /// Повертає повну колекцію завдань без фільтрації.
    /// </summary>
    /// <returns>
    /// ActionResult що містить список всіх завдань ToDo у форматі JSON.
    /// HTTP 200 OK з масивом об'єктів Todo або порожній масив якщо завдань немає.
    /// </returns>
    /// <remarks>
    /// <strong>Backward Compatibility:</strong>
    /// Цей endpoint зберігає сумісність зі старим кодом який очікує всі записи без пагінації.
    /// </remarks>
    [HttpGet("all")]
    public async Task<ActionResult<List<Todo>>> GetAllTodosWithoutPagination()
    {
        List<Todo> todos = await db.Todos.ToListAsync();
        return Ok(todos);
    }

    /// <summary>
    /// Отримує тільки завершені завдання з бази даних.
    /// Фільтрує завдання за статусом IsComplete = true.
    /// </summary>
    /// <returns>
    /// ActionResult що містить список завершених завдань у форматі JSON.
    /// HTTP 200 OK з масивом завершених Todo або порожній масив.
    /// </returns>
    /// <remarks>
    /// <strong>Застосування:</strong>
    /// - Відображення архіву завершених завдань
    /// - Статистика виконаних робіт
    /// - Очищення завершених завдань
    /// - Аналіз продуктивності користувача
    /// </remarks>
    [HttpGet("complete")]
    public async Task<ActionResult<List<Todo>>> GetCompleteTodos()
    {
        List<Todo> completedTodos = await db.Todos
            .Where(t => t.IsComplete)
            .ToListAsync();

        return Ok(completedTodos);
    }

    /// <summary>
    /// Отримує конкретне завдання за його унікальним ідентифікатором.
    /// Виконує пошук завдання в базі даних та повертає його або помилку 404.
    /// </summary>
    /// <param name="id">Унікальний ідентифікатор завдання для пошуку.</param>
    /// <returns>
    /// ActionResult що містить знайдене завдання або HTTP 404 якщо не знайдено.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetTodo(int id)
    {
        Todo? todo = await db.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        return Ok(todo);
    }

    /// <summary>
    /// Створює нове завдання в базі даних.
    /// Приймає дані завдання з HTTP Body та зберігає їх з автоматичним присвоєнням ID.
    /// </summary>
    /// <param name="todo">Об'єкт Todo з даними нового завдання для створення.</param>
    /// <returns>
    /// ActionResult з створеним завданням та HTTP 201 Created статусом.
    /// Включає Location header з URL нового ресурсу.
    /// </returns>
    /// <remarks>
    /// <strong>HTTP Method:</strong> POST
    /// <strong>Route:</strong> /todoitems
    /// <strong>Request Body:</strong> JSON об'єкт Todo
    /// <strong>Response:</strong> 201 Created + JSON об'єкт + Location header
    /// 
    /// <strong>Приклад запиту:</strong>
    /// <code>
    /// POST /todoitems
    /// Content-Type: application/json
    /// 
    /// {
    ///   "name": "Вивчити ASP.NET Core",
    ///   "description": "Пройти офіційний туторіал",
    ///   "priority": "High",
    ///   "dueDate": "2024-02-01",
    ///   "tags": ["навчання", "технології"]
    /// }
    /// </code>
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
    {
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return Created($"/todoitems/{todo.Id}", todo);
    }

    /// <summary>
    /// Оновлює існуюче завдання новими даними.
    /// Замінює всі оновлювані поля завдання даними з запиту.
    /// </summary>
    /// <param name="id">Ідентифікатор завдання яке потрібно оновити.</param>
    /// <param name="inputTodo">Об'єкт Todo з новими даними для оновлення.</param>
    /// <returns>
    /// HTTP 204 No Content при успішному оновленні або 404 Not Found якщо завдання не існує.
    /// </returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(int id, Todo inputTodo)
    {
        Todo? todo = await db.Todos.FindAsync(id);

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

        await db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Видаляє завдання з бази даних за вказаним ідентифікатором.
    /// Виконує повне видалення запису без можливості відновлення.
    /// </summary>
    /// <param name="id">Ідентифікатор завдання яке потрібно видалити.</param>
    /// <returns>
    /// HTTP 204 No Content при успішному видаленні або 404 Not Found якщо завдання не існує.
    /// </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        Todo? todo = await db.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
