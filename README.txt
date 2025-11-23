Exercise 1: Database Design
---------------------------
Time spent: 8 hours

PREREQUISITES
-------------
- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022


INSTRUCTIONS TO RUN
-------------------

1. START THE DATABASE
   
   Open a terminal in the project root directory and execute:
   
   docker-compose up -d
   
   This will start PostgreSQL on port 5432.

2. RUN THE API
   
   - Open the solution "Par.Api.sln" in Visual Studio 2022
   - Press F5 to run the project
   
   The API will run in Development mode and will be available at:
   https://localhost:7020/api/

3. ACCESS SWAGGER
   
   Once the API is running, navigate to URL test the endpoints
   https://localhost:7020/api/swagger


When starting the application for the first time, 50,000 test records are 
automatically created in the database.

ENDPOINTS

- GET    /api/boxes              - List boxes (with pagination)
- GET    /api/boxes/{id}         - Get box by ID
- POST   /api/boxes              - Create new box
- PUT    /api/boxes/{id}         - Update complete box
- PATCH  /api/boxes/{id}         - Partially update box
- DELETE /api/boxes/{id}         - Delete box
- GET    /api/boxes/export       - Export all boxes to CSV
- GET    /api/boxes/benchmark    - Run export benchmark

The GET /api/boxes endpoint accepts the following parameters:
- page: page number (default: 1)
- pageSize: page size (default: 20)


DATABASE CONFIGURATION
PostgreSQL:
  Host: localhost
  Port: 5432
  Database: Par
  Username: postgres
  Password: password

PROJECT STRUCTURE
- Data/           - EF Core context and seed data
- Entities/       - Domain entities
- DTOs/           - Data Transfer Objects
- Services/       - Business logic
- Endpoints/      - Endpoint definitions
- Enums/          - Enumerations
- Extensions/     - Extension methods
- Migrations/     - Entity Framework migrations