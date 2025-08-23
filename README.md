# ToDo Minimal API (ASP.NET Core .NET 9)

A simple reference implementation of an ASP.NET Core .NET 9 Minimal API that manages a to?do list using an in?memory database (Entity Framework Core InMemory provider).

## Features
- .NET 9 Minimal API style (no controllers)
- Entity Framework Core InMemory database (volatile – resets on each run)
- CRUD operations for Todo items
- Swagger / OpenAPI documentation in Development

## Tech Stack
- ASP.NET Core .NET 9
- EF Core (InMemory)
- Swagger / Swashbuckle

## Project Structure
```
/ToDo.Api
  Program.cs      -> Endpoint definitions & app bootstrap
  Todo.cs         -> Todo entity/model
  TodoDb.cs       -> EF Core DbContext
```

## Prerequisites
- .NET 9 SDK (preview if not RTM yet) installed
- (Optional) An HTTP client tool: curl, HTTPie, Postman, VS Code REST, etc.

## Getting Started
Restore & run (first run triggers restore automatically):
```
 dotnet run --project to-doApi/ToDo.Api.csproj
```
The app will listen on the dynamic Kestrel ports printed to console (typically https://localhost:7xxx). In Development you can browse Swagger UI at:
```
https://localhost:<port>/swagger
```

## Endpoints Summary
Base path: (root)

| Method | Route                  | Description                          | Body (JSON)                     |
|--------|------------------------|--------------------------------------|---------------------------------|
| GET    | /todoitems             | Get all todo items                   | —                               |
| GET    | /todoitems/complete    | Get only completed items             | —                               |
| GET    | /todoitems/{id}        | Get single item by id                | —                               |
| POST   | /todoitems             | Create new item                      | { "name": string, "isComplete": bool } |
| PUT    | /todoitems/{id}        | Update existing item (full replace)  | { "id": int, "name": string, "isComplete": bool } |
| DELETE | /todoitems/{id}        | Delete item                          | —                               |

Notes:
- Id is server-assigned on POST; you may omit it or set 0.
- PUT requires the resource to exist; otherwise 404 is returned.

## Example Requests (curl)
Create:
```
curl -s -X POST https://localhost:7000/todoitems \
  -H "Content-Type: application/json" \
  -d '{"name":"Learn minimal APIs","isComplete":false}'
```
List:
```
curl -s https://localhost:7000/todoitems | jq
```
Update:
```
curl -s -X PUT https://localhost:7000/todoitems/1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Learn minimal APIs","isComplete":true}'
```
Delete:
```
curl -X DELETE https://localhost:7000/todoitems/1 -i
```
Adjust the port to match what dotnet run outputs.

## Development Notes
- Because the InMemory provider is used, data is not persisted across restarts. For persistence, swap to a real provider (e.g., SQLite or SQL Server) and update Program.cs service registration.
- Swagger is enabled only in Development (default when running locally). Set ASPNETCORE_ENVIRONMENT=Development to ensure UI availability.

## Possible Improvements
- Add validation (e.g., reject empty names)
- Introduce DTOs to decouple persistence model
- Add filtering / paging
- Persist with SQLite for local development
- Add unit/integration tests
- Add authentication / authorization

## Running Tests
(No tests yet) – You can add a test project with:
```
 dotnet new xunit -n ToDo.Api.Tests
```
Reference the API project and write tests against the endpoints using WebApplicationFactory.

## License
Licensed under the Apache License, Version 2.0. See LICENSE for details.

## Contributing
Issues & PRs welcome. Please keep changes small & focused.

## Contact
Created as a minimal sample. Adapt freely.