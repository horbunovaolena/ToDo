# ToDo Application (ASP.NET Core .NET 9 with Enhanced Web UI)

A comprehensive ToDo application featuring an ASP.NET Core .NET 9 Web API backend with a feature-rich modern web frontend. The application manages tasks with detailed information including priorities, due dates, tags, and descriptions using a SQLite database with Entity Framework Core.

## 🚀 Features

### **🔧 Backend (.NET 9 Web API)**
- .NET 9 Web API with controller-based architecture
- Entity Framework Core with SQLite database and JSON serialization for complex types
- Full CRUD operations for Todo items with extended properties
- Swagger / OpenAPI documentation in Development mode
- CORS enabled for seamless frontend-backend communication
- Advanced tag management with extension methods
- Priority-based task organization
- Persistent data storage with SQLite

### **🎨 Frontend (Modern Web UI)**
- **Rich Task Management:**
  - Create tasks with name, description, priority, due date, and tags
  - Toggle task completion with visual feedback
  - Inline editing (double-click task names)
  - Full-featured edit modal with all properties
  - Bulk operations (clear all completed tasks)

- **Advanced Filtering & Views:**
  - All tasks view
  - Active (incomplete) tasks
  - Completed tasks
  - High priority tasks
  - Overdue tasks with visual indicators

- **Visual Enhancements:**
  - Priority indicators with color coding (High: Red, Medium: Yellow, Low: Green)
  - Due date display with overdue highlighting
  - Tag system with styled badges
  - Creation timestamps
  - Responsive design for all screen sizes

- **User Experience:**
  - Real-time task counter
  - Loading states and error handling
  - Smooth animations and transitions
  - Modal dialogs for detailed editing
  - Quick add functionality
  - Keyboard shortcuts (Enter to add, Escape to cancel)

### **☁️ Cloud Infrastructure (Optional)**
- Azure infrastructure as code using Pulumi C#
- Resource Group and Storage Account provisioning
- Ready for cloud deployment

## Tech Stack

### **Backend:**
- ASP.NET Core .NET 9 Web API
- Entity Framework Core with SQLite database
- JSON serialization for complex types (Tags)
- Swagger / Swashbuckle for API documentation
- Extension methods for clean code organization

### **Frontend:**
- HTML5 with semantic markup
- CSS3 with modern features (Grid, Flexbox, Custom Properties)
- Vanilla JavaScript (ES6+) with async/await
- Fetch API for HTTP requests
- No external dependencies for optimal performance

### **Infrastructure:**
- Pulumi for Infrastructure as Code (Azure)
- SQLite for local development and production

## Project Structure
```
/ToDo.Api
  Program.cs                    -> Web API bootstrap & configuration
  Controllers/
    ToDoController.cs          -> API endpoints for CRUD operations
  Todo.cs                      -> Enhanced Todo entity with all properties
  Priority.cs                  -> Priority enumeration (Low, Medium, High)
  TagHelper.cs                 -> Extension methods for tag management
  TodoDb.cs                    -> EF Core DbContext with JSON configuration
  appsettings.json             -> Database connection string
  /wwwroot                     -> Static web assets
    index.html                 -> Main UI with advanced features
    styles.css                 -> Comprehensive styling with responsive design
    script.js                  -> Feature-rich JavaScript application

/ToDo.Azure
  Program.cs                   -> Pulumi infrastructure definition
  README.md                    -> Azure deployment guide
```

## Getting Started

### **Prerequisites**
- .NET 9 SDK installed
- Modern web browser (Chrome 60+, Firefox 55+, Safari 12+, Edge 79+)
- (Optional) Azure CLI for cloud deployment
- (Optional) Pulumi CLI for infrastructure management

### **Running the Application**
```bash
# Navigate to the API project
cd ToDo.Api

# Run the application
dotnet run
```

The application will start on dynamic Kestrel ports (typically `https://localhost:7xxx`).

### **Database Setup**
The application automatically creates a SQLite database file (`todos.db`) on first run. No manual setup required.

### **Accessing the Application**
1. **🌐 Web Interface**: Navigate to `https://localhost:<port>/`
2. **📖 API Documentation**: Visit `https://localhost:<port>/swagger`
3. **🔌 Direct API Access**: Use endpoints programmatically

## 🎯 Web UI Guide

### **Adding Tasks**
- **Quick Add**: Type in the main input and press Enter
- **Detailed Add**: Use the "Quick Add with Details" section for full properties
- **Required**: Only task name is required, all other fields are optional

### **Managing Tasks**
- **✅ Complete**: Click checkbox to mark as done
- **✏️ Edit**: Click "Edit" button for full modal or double-click name for quick edit
- **🗑️ Delete**: Click "Delete" button (with confirmation)
- **🔍 Filter**: Use filter buttons to view specific task subsets

### **Task Properties**
- **📝 Name**: Task title (required)
- **📄 Description**: Optional detailed description
- **⚡ Priority**: Low (Green) / Medium (Yellow) / High (Red)
- **📅 Due Date**: Optional deadline with overdue detection
- **🏷️ Tags**: Comma-separated labels for organization
- **🕐 Created**: Automatic timestamp tracking

### **Advanced Features**
- **🔎 Smart Filtering**: View tasks by status, priority, or deadline
- **📊 Statistics**: Real-time count of remaining tasks
- **🧹 Bulk Actions**: Clear all completed tasks at once
- **📱 Mobile Ready**: Fully responsive across all devices

## 🔗 API Endpoints

### **Core Todo Operations**
| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `GET` | `/todoitems` | Get all todos | - |
| `GET` | `/todoitems/{id}` | Get specific todo | - |
| `GET` | `/todoitems/complete` | Get completed todos | - |
| `POST` | `/todoitems` | Create new todo | Todo object |
| `PUT` | `/todoitems/{id}` | Update existing todo | Complete Todo object |
| `DELETE` | `/todoitems/{id}` | Delete todo | - |

### **Enhanced Todo Object**
```json
{
  "id": 1,
  "name": "Complete project documentation",
  "description": "Write comprehensive README and API docs",
  "isComplete": false,
  "priority": 3,
  "dueDate": "2024-01-31",
  "tags": ["work", "documentation", "important"],
  "createdDate": "2024-01-15T10:30:00.123Z"
}
```

### **Priority Values**
- `1`: Low Priority (Green indicator)
- `2`: Medium Priority (Yellow indicator) - Default
- `3`: High Priority (Red indicator)

## 💾 Database Configuration

### **SQLite Setup**
- **Storage**: SQLite database file (`todos.db`)
- **Persistence**: Data persists across application restarts
- **JSON Serialization**: Complex types (tags) stored as JSON strings
- **Automatic Creation**: Database and tables created automatically on first run

### **Connection String**
Located in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=todos.db"
  }
}
```

### **Production Considerations**
- Current SQLite setup suitable for development and small-scale production
- For high-traffic scenarios, consider SQL Server, PostgreSQL, or Azure SQL
- Database file should be backed up regularly in production

## 🔧 Development Notes

### **Architecture Pattern**
- **Controller-based Web API** for clear separation of concerns
- **Repository pattern** through Entity Framework DbContext
- **JSON serialization** for complex types in SQLite

### **Frontend Architecture**
- **Modular Design**: Separation of concerns across files
- **Error Handling**: Comprehensive error states and user feedback
- **Performance**: Optimized rendering and minimal DOM manipulation
- **Accessibility**: Semantic HTML and keyboard navigation

### **Extension Methods**
Custom tag management with extension methods:
```csharp
todo.AddTag("important");        // Add normalized tag
todo.RemoveTag("old-tag");       // Remove tag
bool hasTag = todo.HasTag("work"); // Check tag existence
```

## 📖 Example Usage

### **Creating a Detailed Task**
```bash
curl -X POST https://localhost:7000/todoitems \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Prepare presentation",
    "description": "Create slides for quarterly review",
    "priority": 3,
    "dueDate": "2024-02-15",
    "tags": ["work", "presentation", "urgent"]
  }'
```

### **Getting All Tasks**
```bash
curl https://localhost:7000/todoitems
```

### **Getting Completed Tasks**
```bash
curl https://localhost:7000/todoitems/complete
```

## ⚙️ Configuration & Customization

### **Environment Settings**
- **Development**: Full Swagger UI and detailed error messages
- **Production**: Minimal error exposure, consider authentication

### **Database Configuration**
Current setup uses SQLite. For production scaling, update `Program.cs`:
```csharp
// Example: Switch to SQL Server
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

## ☁️ Azure Deployment

The project includes Azure infrastructure as code using Pulumi:

### **Prerequisites**
- Azure CLI (`az login`)
- Pulumi CLI
- Azure subscription

### **Deploy to Azure**
```bash
cd ToDo.Azure
pulumi up
```

See `ToDo.Azure/README.md` for detailed deployment instructions.

## 🚀 Possible Enhancements

### **Backend Improvements**
- [ ] Add input validation with FluentValidation
- [ ] Implement DTOs for API contracts
- [ ] Add authentication/authorization (JWT/Identity)
- [ ] Implement rate limiting
- [ ] Add caching strategies
- [ ] Real-time updates with SignalR
- [ ] Add tag-specific endpoints (GET /tags, GET /todoitems/tag/{tag})

### **Frontend Enhancements**
- [ ] Drag-and-drop task reordering
- [ ] Local storage for offline capability
- [ ] Dark/light theme toggle
- [ ] Progressive Web App (PWA) features
- [ ] Advanced search and filtering
- [ ] Task categories and projects
- [ ] Recurring task support
- [ ] Time tracking integration

### **DevOps & Testing**
- [ ] Unit and integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline setup
- [ ] Performance monitoring
- [ ] Automated UI testing

## 🧪 Testing

### **Setup Test Project**
```bash
dotnet new xunit -n ToDo.Api.Tests
dotnet add ToDo.Api.Tests reference ToDo.Api
dotnet add ToDo.Api.Tests package Microsoft.AspNetCore.Mvc.Testing
```

### **Recommended Test Coverage**
- API endpoint functionality
- Business logic validation
- Frontend user interactions
- Database operations
- Error handling scenarios

## 🌐 Browser Support
- **Chrome**: 60+ ✅
- **Firefox**: 55+ ✅
- **Safari**: 12+ ✅
- **Edge**: 79+ ✅
- **Mobile**: iOS Safari, Chrome Mobile ✅

## 📄 License
Licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE) for details.

## 🤝 Contributing
Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make focused, well-tested changes
4. Submit a pull request with clear description

## 🏷️ Tags
`asp-net-core` `dotnet-9` `web-api` `javascript` `todo-app` `entity-framework` `sqlite` `responsive-design` `modern-ui` `pulumi` `azure`

## 📞 Contact
Created as a comprehensive ToDo application demonstrating modern full-stack development practices with .NET 9 and vanilla JavaScript. Feel free to adapt and extend for your needs!

---

**⭐ Star this repository if you find it helpful!**
