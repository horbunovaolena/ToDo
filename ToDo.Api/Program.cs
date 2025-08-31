using Microsoft.EntityFrameworkCore;
using ToDo.Api;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Change from InMemory to SQLite
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add controller services
builder.Services.AddControllers();

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToDo API",
        Version = "v1",
        Description = "A simple ToDo API built with ASP.NET Core"
    });
});

WebApplication app = builder.Build();

// Enable static files
app.UseStaticFiles();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve index.html as default page
app.MapGet("/", () => Results.Redirect("/index.html"));

// Map controllers
app.MapControllers();

await app.RunAsync();