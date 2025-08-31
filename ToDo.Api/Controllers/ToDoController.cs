using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Api.Controllers;

/// <summary>
/// REST API контролер для управління завданнями ToDo.
/// Надає повний набір CRUD операцій для роботи з завданнями через HTTP endpoints.
/// </summary>
/// <remarks>
/// ToDoController є основним API контролером що забезпечує:
/// 
/// <strong>HTTP Endpoints:</strong>
/// - GET /todoitems - отримання всіх завдань
/// - GET /todoitems/complete - отримання завершених завдань  
/// - GET /todoitems/{id} - отримання конкретного завдання
/// - POST /todoitems - створення нового завдання
/// - PUT /todoitems/{id} - оновлення існуючого завдання
/// - DELETE /todoitems/{id} - видалення завдання
/// 
/// <strong>Архітектурні особливості:</strong>
/// - Використовує Primary Constructor (C# 12+) для dependency injection
/// - Асинхронні операції для кращої продуктивності
/// - Правильні HTTP статус коди для різних сценаріїв
/// - Entity Framework Core для доступу до даних
/// - Автоматична серіалізація JSON через ASP.NET Core
/// 
/// <strong>Обробка помилок:</strong>
/// - 404 Not Found для неіснуючих завдань
/// - 201 Created для успішного створення
/// - 204 No Content для успішного оновлення/видалення
/// - 200 OK для успішного отримання даних
/// 
/// Контролер спроектований для роботи з Single Page Applications (SPA)
/// та мобільними клієнтами через стандартний REST API.
/// </remarks>
[ApiController]
[Route("todoitems")]
public class ToDoController(TodoDb _db) : ControllerBase
{
    /// <summary>
    /// Отримує список всіх завдань з бази даних.
    /// Повертає повну колекцію завдань без фільтрації.
    /// </summary>
    /// <returns>
    /// ActionResult що містить список всіх завдань ToDo у форматі JSON.
    /// HTTP 200 OK з масивом об'єктів Todo або порожній масив якщо завдань немає.
    /// </returns>
    /// <remarks>
    /// <strong>HTTP Method:</strong> GET
    /// <strong>Route:</strong> /todoitems
    /// <strong>Response:</strong> 200 OK + JSON масив
    /// 
    /// <strong>Особливості реалізації:</strong>
    /// - Використовує ToListAsync() для асинхронного завантаження
    /// - Завантажує ВСІ завдання в пам'ять (розгляньте пагінацію для великих наборів)
    /// - JSON серіалізація включає всі властивості Todo
    /// - Теги автоматично десеріалізуються з JSON стовпця БД
    /// 
    /// <strong>Приклад відповіді:</strong>
    /// <code>
    /// HTTP 200 OK
    /// [
    ///   {
    ///     "id": 1,
    ///     "name": "Купити молоко",
    ///     "isComplete": false,
    ///     "description": "В найближчому магазині",
    ///     "createdDate": "2024-01-15T10:30:00Z",
    ///     "dueDate": "2024-01-16",
    ///     "priority": "Medium",
    ///     "tags": ["shopping", "personal"]
    ///   }
    /// ]
    /// </code>
    /// 
    /// <strong>Клієнтське використання:</strong>
    /// - Відображення списку всіх завдань
    /// - Ініціалізація додатку з поточними даними
    /// - Основа для клієнтської фільтрації та сортування
    /// </remarks>
    [HttpGet]
    public async Task<ActionResult<List<Todo>>> GetAllTodos()
    {
        List<Todo> todos = await _db.Todos.ToListAsync();
        
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
    /// <strong>HTTP Method:</strong> GET
    /// <strong>Route:</strong> /todoitems/complete
    /// <strong>Response:</strong> 200 OK + JSON масив
    /// 
    /// <strong>Фільтрація на рівні БД:</strong>
    /// Використовує Where() клаузулу що транслюється в SQL WHERE,
    /// тому фільтрація виконується на рівні бази даних для оптимальної продуктивності.
    /// 
    /// <strong>SQL запит (приблизно):</strong>
    /// <code>
    /// SELECT * FROM Todos WHERE IsComplete = 1
    /// </code>
    /// 
    /// <strong>Застосування:</strong>
    /// - Відображення архіву завершених завдань
    /// - Статистика виконаних робіт
    /// - Очищення завершених завдань
    /// - Аналіз продуктивності користувача
    /// 
    /// <strong>Приклад використання клієнтом:</strong>
    /// <code>
    /// fetch('/todoitems/complete')
    ///   .then(response => response.json())
    ///   .then(completedTodos => {
    ///     displayCompletedTasks(completedTodos);
    ///   });
    /// </code>
    /// 
    /// <strong>Оптимізація:</strong>
    /// Для великих обсягів даних розгляньте додавання:
    /// - Пагінації (skip/take параметри)
    /// - Обмеження за датою (останні N днів)
    /// - Індексу на IsComplete стовпець
    /// </remarks>
    [HttpGet("complete")]
    public async Task<ActionResult<List<Todo>>> GetCompleteTodos()
    {
        List<Todo> completedTodos = await _db.Todos
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
    /// <remarks>
    /// <strong>HTTP Method:</strong> GET
    /// <strong>Route:</strong> /todoitems/{id}
    /// <strong>Response:</strong> 200 OK + JSON об'єкт ИЛИ 404 Not Found
    /// 
    /// <strong>Параметри маршруту:</strong>
    /// - {id} автоматично прив'язується до параметра int id методу
    /// - ASP.NET Core автоматично валідує що id є числом
    /// - Неправильний формат id поверне 400 Bad Request автоматично
    /// 
    /// <strong>Логіка пошуку:</strong>
    /// 1. FindAsync(id) - оптимізований пошук за первинним ключем
    /// 2. Перевірка на null (завдання не існує)
    /// 3. Повернення відповідного HTTP статусу
    /// 
    /// <strong>Приклади відповідей:</strong>
    /// 
    /// <em>Успішний пошук:</em>
    /// <code>
    /// GET /todoitems/1
    /// HTTP 200 OK
    /// {
    ///   "id": 1,
    ///   "name": "Завершити проект",
    ///   "isComplete": false,
    ///   ...
    /// }
    /// </code>
    /// 
    /// <em>Завдання не знайдено:</em>
    /// <code>
    /// GET /todoitems/999
    /// HTTP 404 Not Found
    /// {
    ///   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    ///   "title": "Not Found",
    ///   "status": 404
    /// }
    /// </code>
    /// 
    /// <strong>Використання клієнтом:</strong>
    /// - Відображення деталей завдання
    /// - Редагування конкретного завдання
    /// - Перевірка існування завдання
    /// - Deep linking до конкретних завдань
    /// 
    /// <strong>Оптимізація:</strong>
    /// FindAsync() використовує кеш Entity Framework для покращення продуктивності
    /// при повторних запитах того ж завдання.
    /// </remarks>
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
    /// <strong>Процес створення:</strong>
    /// 1. Model binding - автоматичне перетворення JSON в об'єкт Todo
    /// 2. Add() - додавання до контексту EF (ще не в БД)
    /// 3. SaveChangesAsync() - збереження в базу даних
    /// 4. Автоматичне присвоєння ID новому завданню
    /// 5. Повернення Created response з Location header
    /// 
    /// <strong>Автоматичні значення:</strong>
    /// - Id: Генерується базою даних (IDENTITY/AUTOINCREMENT)
    /// - CreatedDate: Встановлюється в конструкторі Todo
    /// - Priority: За замовчуванням Medium якщо не вказано
    /// - Tags: Порожній список якщо не вказано
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
    /// 
    /// <strong>Приклад відповіді:</strong>
    /// <code>
    /// HTTP 201 Created
    /// Location: /api/todoitems/5
    /// 
    /// {
    ///   "id": 5,
    ///   "name": "Вивчити ASP.NET Core", 
    ///   "description": "Пройти офіційний туторіал",
    ///   "isComplete": false,
    ///   "createdDate": "2024-01-15T14:30:00Z",
    ///   "priority": "High",
    ///   "dueDate": "2024-02-01",
    ///   "tags": ["навчання", "технології"]
    /// }
    /// </code>
    /// 
    /// <strong>Валідація:</strong>
    /// - ASP.NET Core автоматично валідує JSON структуру
    /// - Model validation attributes можна додати до Todo класу
    /// - Неправильний JSON поверне 400 Bad Request
    /// 
    /// <strong>Ідемпотентність:</strong>
    /// POST НЕ є ідемпотентним - кожен виклик створить нове завдання.
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
    {
        _db.Todos.Add(todo);
        
        await _db.SaveChangesAsync();
        
        return Created($"/api/todoitems/{todo.Id}", todo);
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
    /// <remarks>
    /// <strong>HTTP Method:</strong> PUT
    /// <strong>Route:</strong> /todoitems/{id}
    /// <strong>Request Body:</strong> JSON об'єкт Todo
    /// <strong>Response:</strong> 204 No Content ИЛИ 404 Not Found
    /// 
    /// <strong>Семантика PUT:</strong>
    /// PUT виконує ПОВНУ заміну ресурсу - всі оновлювані поля встановлюються з inputTodo.
    /// Для часткового оновлення розгляньте PATCH метод.
    /// 
    /// <strong>Процес оновлення:</strong>
    /// 1. Пошук існуючого завдання за ID
    /// 2. Перевірка існування (404 якщо не знайдено)
    /// 3. Оновлення всіх модифікуючих полів
    /// 4. Збереження змін в базу даних
    /// 5. Повернення 204 No Content (стандарт для PUT)
    /// 
    /// <strong>Поля що оновлюються:</strong>
    /// - Name - назва завдання
    /// - IsComplete - статус завершення
    /// - Description - опис завдання  
    /// - DueDate - термін виконання
    /// - Priority - пріоритет завдання
    /// - Tags - список тегів
    /// 
    /// <strong>Поля що НЕ оновлюються:</strong>
    /// - Id - незмінний ідентифікатор
    /// - CreatedDate - дата створення зберігається
    /// 
    /// <strong>Приклад запиту:</strong>
    /// <code>
    /// PUT /todoitems/1
    /// Content-Type: application/json
    /// 
    /// {
    ///   "name": "Оновлена назва завдання",
    ///   "isComplete": true,
    ///   "description": "Нові деталі завдання",
    ///   "priority": "Low",
    ///   "dueDate": null,
    ///   "tags": ["оновлено"]
    /// }
    /// </code>
    /// 
    /// <strong>Приклади відповідей:</strong>
    /// 
    /// <em>Успішне оновлення:</em>
    /// <code>
    /// HTTP 204 No Content
    /// (порожнє тіло відповіді)
    /// </code>
    /// 
    /// <em>Завдання не знайдено:</em>
    /// <code>
    /// HTTP 404 Not Found
    /// {
    ///   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    ///   "title": "Not Found", 
    ///   "status": 404
    /// }
    /// </code>
    /// 
    /// <strong>Change Tracking:</strong>
    /// Entity Framework автоматично відстежує зміни і генерує SQL UPDATE
    /// тільки для полів що реально змінились.
    /// 
    /// <strong>Транзакційність:</strong>
    /// SaveChangesAsync() виконується в транзакції - або всі зміни зберігаються,
    /// або жодна (atomic operation).
    /// </remarks>
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

    /// <summary>
    /// Видаляє завдання з бази даних за вказаним ідентифікатором.
    /// Виконує повне видалення запису без можливості відновлення.
    /// </summary>
    /// <param name="id">Ідентифікатор завдання яке потрібно видалити.</param>
    /// <returns>
    /// HTTP 204 No Content при успішному видаленні або 404 Not Found якщо завдання не існує.
    /// </returns>
    /// <remarks>
    /// <strong>HTTP Method:</strong> DELETE
    /// <strong>Route:</strong> /todoitems/{id}
    /// <strong>Response:</strong> 204 No Content ИЛИ 404 Not Found
    /// 
    /// <strong>Семантика DELETE:</strong>
    /// DELETE є ідемпотентним - повторні виклики з тим же ID будуть повертати 404
    /// після першого успішного видалення.
    /// 
    /// <strong>Процес видалення:</strong>
    /// 1. Пошук завдання за ID в базі даних
    /// 2. Перевірка існування (404 якщо не знайдено)
    /// 3. Позначення для видалення в EF контексті
    /// 4. Збереження змін (фактичне видалення з БД)
    /// 5. Повернення 204 No Content
    /// 
    /// <strong>Безпека операції:</strong>
    /// - Операція незворотна - видалені дані не можна відновити
    /// - Рекомендується додати підтвердження на клієнті
    /// - Для аудиту розгляньте "soft delete" (IsDeleted flag)
    /// 
    /// <strong>Приклади запитів та відповідей:</strong>
    /// 
    /// <em>Успішне видалення:</em>
    /// <code>
    /// DELETE /todoitems/1
    /// HTTP 204 No Content
    /// (порожнє тіло відповіді)
    /// </code>
    /// 
    /// <em>Завдання не існує:</em>
    /// <code>
    /// DELETE /todoitems/999  
    /// HTTP 404 Not Found
    /// {
    ///   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    ///   "title": "Not Found",
    ///   "status": 404
    /// }
    /// </code>
    /// 
    /// <em>Повторне видалення:</em>
    /// <code>
    /// DELETE /todoitems/1 (уже видалено)
    /// HTTP 404 Not Found
    /// (ідемпотентна поведінка)
    /// </code>
    /// 
    /// <strong>Клієнтське використання:</strong>
    /// <code>
    /// // JavaScript приклад з підтвердженням
    /// if (confirm('Видалити це завдання назавжди?')) {
    ///   fetch(`/todoitems/${todoId}`, { method: 'DELETE' })
    ///     .then(response => {
    ///       if (response.status === 204) {
    ///         removeFromUI(todoId);
    ///       }
    ///     });
    /// }
    /// </code>
    /// 
    /// <strong>Альтернативні підходи:</strong>
    /// - Soft Delete: додати IsDeleted поле замість фізичного видалення
    /// - Archive: перемістити в архівну таблицю перед видаленням
    /// - Cascade Delete: автоматичне видалення пов'язаних даних
    /// 
    /// <strong>Транзакційність:</strong>
    /// Видалення виконується в транзакції через SaveChangesAsync(),
    /// що гарантує консистентність даних.
    /// </remarks>
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