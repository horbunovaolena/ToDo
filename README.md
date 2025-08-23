# ToDo Application (ASP.NET Core .NET 9 with Web UI)

A complete ToDo application featuring an ASP.NET Core .NET 9 Minimal API backend with a modern web frontend. The application manages a to-do list using an in-memory database (Entity Framework Core InMemory provider) and provides both API endpoints and a user-friendly web interface.

## Features
- .NET 9 Minimal API backend (no controllers)
- **Modern Web UI** with HTML, CSS, and JavaScript
- Entity Framework Core InMemory database (volatile – resets on each run)
- Full CRUD operations for Todo items
- **Interactive Frontend Features:**
  - Add, edit, delete, and toggle completion of tasks
  - Filter tasks by status (All, Active, Completed)
  - Real-time task counter
  - Clear all completed tasks
  - Responsive design for desktop and mobile
  - Double-click to edit tasks inline
- Swagger / OpenAPI documentation in Development
- CORS enabled for frontend-backend communication

## Tech Stack
**Backend:**
- ASP.NET Core .NET 9
- EF Core (InMemory)
- Swagger / Swashbuckle

**Frontend:**
- HTML5
- CSS3 (with modern styling and responsive design)
- Vanilla JavaScript (ES6+)
- Fetch API for HTTP requests

## Project Structure
```
/ToDo.Api
  Program.cs           -> API endpoint definitions & app bootstrap
  Todo.cs              -> Todo entity/model
  TodoDb.cs            -> EF Core DbContext
  /wwwroot             -> Static web files
    index.html         -> Main UI page
    styles.css         -> Modern CSS styling
    script.js          -> Frontend JavaScript logic
```

## Prerequisites
- .NET 9 SDK installed
- Modern web browser (Chrome, Firefox, Safari, Edge)
- (Optional) An HTTP client tool for API testing: curl, HTTPie, Postman, VS Code REST, etc.

## Getting Started

### Running the Application
```bash
dotnet run --project ToDo.Api/ToDo.Api.csproj
```

The app will listen on the dynamic Kestrel ports printed to console (typically https://localhost:7xxx).

### Accessing the Application
1. **Web UI**: Open your browser and navigate to `https://localhost:<port>/` to use the interactive todo interface
2. **API Documentation**: Browse Swagger UI at `https://localhost:<port>/swagger` (Development mode)
3. **Direct API Access**: Use the API endpoints directly for integration or testing

## Web UI Usage

### Adding Tasks
- Type your task in the input field
- Press Enter or click "Add Task"

### Managing Tasks
- **Mark Complete**: Click the checkbox next to any task
- **Edit Task**: Double-click on the task text or click the "Edit" button
- **Delete Task**: Click the "Delete" button (confirmation required)

### Filtering Tasks
- **All**: View all tasks
- **Active**: View only incomplete tasks
- **Completed**: View only completed tasks

### Additional Features
- **Task Counter**: Shows remaining active tasks
- **Clear Completed**: Remove all completed tasks at once
- **Responsive Design**: Works seamlessly on desktop and mobile devices

## API Endpoints Summary
Base path: (root)

| Method | Route                  | Description                          | Body (JSON)                     |
|--------|------------------------|--------------------------------------|---------------------------------|
| GET    | /todoitems             | Get all todo items                   | –                               |
| GET    | /todoitems/complete    | Get only completed items             | –                               |
| GET    | /todoitems/{id}        | Get single item by id                | –                               |
| POST   | /todoitems             | Create new item                      | { "name": string, "isComplete": bool } |
| PUT    | /todoitems/{id}        | Update existing item (full replace)  | { "id": int, "name": string, "isComplete": bool } |
| DELETE | /todoitems/{id}        | Delete item                          | –                               |

Notes:
- Id is server-assigned on POST; you may omit it or set 0.
- PUT requires the resource to exist; otherwise 404 is returned.
- CORS is enabled to allow frontend-backend communication.

## Example API Requests (curl)
Create:
```bash
curl -s -X POST https://localhost:7000/todoitems \
  -H "Content-Type: application/json" \
  -d '{"name":"Learn minimal APIs","isComplete":false}'
```
List:
```bash
curl -s https://localhost:7000/todoitems | jq
```
Update:
```bash
curl -s -X PUT https://localhost:7000/todoitems/1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Learn minimal APIs","isComplete":true}'
```
Delete:
```bash
curl -X DELETE https://localhost:7000/todoitems/1 -i
```
Adjust the port to match what dotnet run outputs.

## Development Notes
- **Database**: InMemory provider is used, so data is not persisted across restarts. For persistence, swap to a real provider (e.g., SQLite or SQL Server) and update Program.cs service registration.
- **Environment**: Swagger is enabled only in Development (default when running locally). Set ASPNETCORE_ENVIRONMENT=Development to ensure UI availability.
- **Static Files**: The web UI is served from the wwwroot folder using ASP.NET Core's static file middleware.
- **CORS**: Configured to allow all origins for development. Restrict this for production use.

## Frontend Architecture
The frontend follows modern web development practices:
- **Separation of Concerns**: HTML for structure, CSS for styling, JavaScript for behavior
- **Responsive Design**: Mobile-first approach with flexible layouts
- **Error Handling**: User-friendly error messages and loading states
- **Accessibility**: Semantic HTML and keyboard navigation support
- **Performance**: Minimal dependencies, optimized for fast loading

## Possible Improvements
**Backend:**
- Add validation (e.g., reject empty names)
- Introduce DTOs to decouple persistence model
- Add filtering / paging to API endpoints
- Persist with SQLite for local development
- Add authentication / authorization
- Implement rate limiting

**Frontend:**
- Add drag-and-drop reordering
- Implement local storage backup
- Add task categories/tags
- Include due dates and priorities
- Add dark mode toggle
- Implement Progressive Web App (PWA) features

**General:**
- Add unit/integration tests
- Set up CI/CD pipeline
- Add Docker containerization
- Implement real-time updates with SignalR

## Running Tests
(No tests yet) – You can add a test project with:
```bash
dotnet new xunit -n ToDo.Api.Tests
```
Reference the API project and write tests against the endpoints using WebApplicationFactory.

## Browser Compatibility
The frontend is compatible with:
- Chrome 60+
- Firefox 55+
- Safari 12+
- Edge 79+

## License
Licensed under the Apache License, Version 2.0. See LICENSE for details.

## Contributing
Issues & PRs welcome. Please keep changes small & focused.

## Contact
Created as a full-stack ToDo application sample. Adapt freely for your needs.