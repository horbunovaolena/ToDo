# ToDo Application (ASP.NET Core .NET 9 with Enhanced Web UI)

A comprehensive ToDo application featuring an ASP.NET Core .NET 9 Minimal API backend with a feature-rich modern web frontend. The application manages tasks with detailed information including priorities, due dates, tags, and descriptions using an in-memory database (Entity Framework Core InMemory provider).

## ? Features

### ** Backend (.NET 9 Minimal API)**
- .NET 9 Minimal API backend with no controllers
- Entity Framework Core InMemory database with JSON serialization for complex types
- Full CRUD operations for Todo items with extended properties
- Swagger / OpenAPI documentation in Development mode
- CORS enabled for seamless frontend-backend communication
- Advanced tag management with extension methods
- Priority-based task organization

### ** Frontend (Modern Web UI)**
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

## Tech Stack

### **Backend:**
- ASP.NET Core .NET 9
- Entity Framework Core (InMemory) with custom JSON converters
- Swagger / Swashbuckle for API documentation
- Extension methods for clean code organization

### **Frontend:**
- HTML5 with semantic markup
- CSS3 with modern features (Grid, Flexbox, Custom Properties)
- Vanilla JavaScript (ES6+) with async/await
- Fetch API for HTTP requests
- No external dependencies for optimal performance

## Project Structure
```
/ToDo.Api
  Program.cs           -> API endpoints & application bootstrap
  Todo.cs              -> Enhanced Todo entity with all properties
  Priority.cs          -> Priority enumeration (Low, Medium, High)
  TagHelper.cs         -> Extension methods for tag management
  TodoDb.cs            -> EF Core DbContext with JSON configuration
  /wwwroot             -> Static web assets
    index.html         -> Main UI with advanced features
    styles.css         -> Comprehensive styling with responsive design
    script.js          -> Feature-rich JavaScript application
```

## Getting Started

### **Prerequisites**
- .NET 9 SDK installed
- Modern web browser (Chrome 60+, Firefox 55+, Safari 12+, Edge 79+)
- (Optional) API testing tools: curl, Postman, etc.

### **Running the Application**
```bash
dotnet run --project ToDo.Api/ToDo.Api.csproj
```

The application will start on dynamic Kestrel ports (typically `https://localhost:7xxx`).

### **Accessing the Application**
1. **?? Web Interface**: Navigate to `https://localhost:<port>/`
2. **?? API Documentation**: Visit `https://localhost:<port>/swagger`
3. **?? Direct API Access**: Use endpoints programmatically

## ?? Web UI Guide

### **Adding Tasks**
- **Quick Add**: Type in the main input and press Enter
- **Detailed Add**: Use the "Quick Add with Details" section for full properties
- **Required**: Only task name is required, all other fields are optional

### **Managing Tasks**
- **? Complete**: Click checkbox to mark as done
- **?? Edit**: Click "Edit" button for full modal or double-click name for quick edit
- **??? Delete**: Click "Delete" button (with confirmation)
- **?? Filter**: Use filter buttons to view specific task subsets

### **Task Properties**
- **?? Name**: Task title (required)
- **?? Description**: Optional detailed description
- **? Priority**: Low (Green) / Medium (Yellow) / High (Red)
- **?? Due Date**: Optional deadline with overdue detection
- **??? Tags**: Comma-separated labels for organization
- **?? Created**: Automatic timestamp tracking

### **Advanced Features**
- **?? Smart Filtering**: View tasks by status, priority, or deadline
- **?? Statistics**: Real-time count of remaining tasks
- **?? Bulk Actions**: Clear all completed tasks at once
- **?? Mobile Ready**: Fully responsive across all devices

## ?? API Endpoints

### **Core Todo Operations**
| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `GET` | `/todoitems` | Get all todos | - |
| `GET` | `/todoitems/{id}` | Get specific todo | - |
| `GET` | `/todoitems/complete` | Get completed todos | - |
| `POST` | `/todoitems` | Create new todo | Todo object |
| `PUT` | `/todoitems/{id}` | Update existing todo | Complete Todo object |
| `DELETE` | `/todoitems/{id}` | Delete todo | - |

### **Tag Management**
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/tags` | Get all unique tags |
| `GET` | `/todoitems/tag/{tag}` | Get todos by specific tag |

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

## ??? Development Notes

### **Database Configuration**
- **Storage**: InMemory provider for development simplicity
- **Persistence**: Data resets on application restart
- **JSON Serialization**: Complex types (tags) stored as JSON
- **Production**: Consider SQLite/SQL Server for persistence

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

## ?? Example Usage

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

### **Filtering by Priority**
```bash
# Get all todos, then filter high priority on frontend
curl https://localhost:7000/todoitems
```

### **Tag-based Search**
```bash
# Get all todos tagged with "work"
curl https://localhost:7000/todoitems/tag/work
```

## ?? Configuration & Customization

### **Environment Settings**
- **Development**: Full Swagger UI and detailed error messages
- **Production**: Minimal error exposure, consider authentication

### **CORS Policy**
Currently configured for development with `AllowAnyOrigin`. Restrict for production:
```csharp
policy.WithOrigins("https://yourdomain.com")
```

## ?? Possible Enhancements

### **Backend Improvements**
- [ ] Add input validation with FluentValidation
- [ ] Implement DTOs for API contracts
- [ ] Add authentication/authorization (JWT/Identity)
- [ ] Implement rate limiting
- [ ] Add caching strategies
- [ ] Database persistence (SQLite/PostgreSQL)
- [ ] Real-time updates with SignalR

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

## ?? Testing

### **Setup Test Project**
```bash
dotnet new xunit -n ToDo.Api.Tests
dotnet add ToDo.Api.Tests reference ToDo.Api
```

### **Recommended Test Coverage**
- API endpoint functionality
- Business logic validation
- Frontend user interactions
- Database operations
- Error handling scenarios

## ?? Browser Support
- **Chrome**: 60+ ?
- **Firefox**: 55+ ?
- **Safari**: 12+ ?
- **Edge**: 79+ ?
- **Mobile**: iOS Safari, Chrome Mobile ?

## ?? License
Licensed under the Apache License, Version 2.0. See [LICENSE](LICENSE) for details.

## ?? Contributing
Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make focused, well-tested changes
4. Submit a pull request with clear description

## ??? Tags
`asp-net-core` `dotnet-9` `minimal-api` `javascript` `todo-app` `entity-framework` `responsive-design` `modern-ui`

## ?? Contact
Created as a comprehensive ToDo application demonstrating modern full-stack development practices with .NET 9 and vanilla JavaScript. Feel free to adapt and extend for your needs!

---

**? Star this repository if you find it helpful!**
