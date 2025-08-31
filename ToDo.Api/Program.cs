using Microsoft.EntityFrameworkCore;
using ToDo.Api;

var builder = WebApplication.CreateBuilder(args);

// Change from InMemory to SQLite
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add controller services
builder.Services.AddControllers();

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();