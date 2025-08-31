using Microsoft.EntityFrameworkCore;
using ToDo.Api;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

/// <summary>
/// Точка входу для додатку ToDo API.
/// Цей клас налаштовує та ініціалізує веб-додаток ASP.NET Core з Entity Framework,
/// документацією Swagger, обслуговуванням статичних файлів та API контролерами.
/// </summary>
/// <remarks>
/// Додаток надає RESTful API для управління елементами ToDo з наступними функціями:
/// - CRUD операції для елементів ToDo
/// - Інтеграція бази даних SQLite через Entity Framework Core
/// - Документація Swagger/OpenAPI для API кінцевих точок
/// - Обслуговування статичних файлів для веб UI
/// - Маршрутизація на основі контролерів для API кінцевих точок
/// - Конфігурації специфічні для розробки (фільтри винятків бази даних, Swagger UI)
/// </remarks>
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Налаштування Entity Framework Core з провайдером бази даних SQLite.
/// Встановлює контекст TodoDb для використання бази даних SQLite з рядком підключення з конфігурації.
/// </summary>
/// <remarks>
/// Рядок підключення отримується з appsettings.json під "ConnectionStrings:DefaultConnection".
/// Файл бази даних SQLite буде створений, якщо він не існує.
/// </remarks>
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

/// <summary>
/// Додає фільтр винятків сторінки розробника бази даних для покращених сторінок помилок під час розробки.
/// Надає детальну інформацію про винятки пов'язані з базою даних коли операції бази даних зазнають невдачі.
/// </summary>
/// <remarks>
/// Цей сервіс в основному корисний під час розробки для діагностики проблем пов'язаних з Entity Framework.
/// Він показує детальну інформацію про винятки бази даних включаючи пропозиції міграцій.
/// </remarks>
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/// <summary>
/// Налаштування MVC контролерів з покращеними опціями серіалізації JSON.
/// Включає маршрутизацію на основі контролерів та API кінцеві точки з покращеною обробкою JSON.
/// </summary>
/// <remarks>
/// Налаштовані опції JSON:
/// - JsonStringEnumConverter: Серіалізує енуми як рядки замість чисел
/// - ReferenceHandler.IgnoreCycles: Запобігає проблемам циклічних посилань у графах об'єктів
/// </remarks>
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Конвертує енуми в рядкове представлення у JSON (наприклад, "High" замість 3)
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Обробляє циклічні посилання у графах об'єктів
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

/// <summary>
/// Налаштування API Explorer для автоматичного виявлення кінцевих точок API.
/// Необхідно для роботи генерації документації Swagger/OpenAPI з контролерами.
/// </summary>
/// <remarks>
/// Цей сервіс сканує додаток для кінцевих точок API та робить їх доступними
/// для генерації документації Swagger.
/// </remarks>
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Налаштування генерації документації Swagger/OpenAPI з користувацькими налаштуваннями.
/// Встановлює автоматичну документацію API з правильним версіонуванням та зіставленням типів.
/// </summary>
/// <remarks>
/// Конфігурація включає:
/// - Метадані API (назва, версія, опис)
/// - Зіставлення типу DateOnly для правильної генерації схеми OpenAPI
/// - Підтримка nullable типів DateOnly
/// 
/// Згенерована документація буде доступна на кінцевій точці /swagger під час розробки.
/// </remarks>
builder.Services.AddSwaggerGen(c =>
{
    // Визначає основний документ API з метаданими
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "Комплексне ToDo API створене з ASP.NET Core що надає CRUD операції для управління завданнями"
    });
    
    /// <summary>
    /// Зіставляє тип DateOnly з форматом дати OpenAPI.
    /// Необхідно тому що DateOnly є типом .NET 6+ який потребує явного зіставлення для Swagger.
    /// </summary>
    c.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
    
    /// <summary>
    /// Зіставляє nullable тип DateOnly з форматом дати OpenAPI.
    /// Обробляє опціональні поля дати в моделях API.
    /// </summary>
    c.MapType<DateOnly?>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date",
        Nullable = true
    });
});

/// <summary>
/// Створює налаштований екземпляр веб-додатку.
/// Створює додаток з усіма попередньо налаштованими сервісами та middleware.
/// </summary>
WebApplication app = builder.Build();

/// <summary>
/// Включає middleware для обслуговування статичних файлів.
/// Дозволяє додатку обслуговувати статичні файли (HTML, CSS, JavaScript) з директорії wwwroot.
/// </summary>
/// <remarks>
/// Статичні файли обслуговуються безпосередньо без обробки та зазвичай використовуються для:
/// - Файли веб UI (HTML, CSS, JavaScript)
/// - Зображення, шрифти та інші ресурси
/// - Файли клієнтського додатку
/// </remarks>
app.UseStaticFiles();

/// <summary>
/// Налаштування middleware Swagger тільки для середовища розробки.
/// Включає кінцеву точку Swagger JSON та інтерактивний Swagger UI для тестування API та документації.
/// </summary>
/// <remarks>
/// Конфігурація тільки для розробки включає:
/// - Кінцева точка документа Swagger JSON: /swagger/v1/swagger.json
/// - Інтерактивний Swagger UI: /swagger
/// 
/// Це middleware вимкнене у продакшені з міркувань безпеки та продуктивності.
/// </remarks>
if (app.Environment.IsDevelopment())
{
    // Включає кінцеву точку Swagger JSON
    app.UseSwagger();
    // Включає інтерактивний Swagger UI
    app.UseSwaggerUI();
}

/// <summary>
/// Налаштування кореневого маршруту для перенаправлення на основний веб-інтерфейс.
/// Зіставляє кореневий URL додатку ("/") для перенаправлення користувачів на основну HTML сторінку.
/// </summary>
/// <remarks>
/// Це надає типову цільову сторінку для користувачів що заходять на корінь додатку.
/// Перенаправлення вказує на index.html який повинен містити основний веб UI.
/// </remarks>
app.MapGet("/", () => Results.Redirect("/index.html"));

/// <summary>
/// Включає маршрутизацію на основі контролерів.
/// Зіставляє всі дії контролерів з їх відповідними маршрутами на основі атрибутів контролера та дії.
/// </summary>
/// <remarks>
/// Це включає всі контролери в додатку для обробки HTTP запитів на основі їх конфігурації маршрутизації.
/// Контролери використовують атрибути як [Route], [HttpGet], [HttpPost], тощо для визначення своїх кінцевих точок.
/// </remarks>
app.MapControllers();

/// <summary>
/// Запускає веб-додаток та починає прослуховування HTTP запитів.
/// Це асинхронна операція яка запускає додаток до його завершення.
/// </summary>
/// <remarks>
/// Додаток буде прослуховувати на налаштованих портах (зазвичай HTTPS та HTTP).
/// Цей метод блокується до завершення додатку (Ctrl+C, SIGTERM, тощо).
/// </remarks>
await app.RunAsync();