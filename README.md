# Project ECHO

This is a .NET Core web application that recreates the UI shown in the Figma design.

## Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or Visual Studio Code
- Docker (for containerized deployment)
- Docker Compose (for multi-container setup)

## How to Run the Project

### Option 1: Local Development (with Hot Reload)

1. Clone the repository
2. Navigate to the project folder
3. Run the following command:
```bash
dotnet watch run
```
4. Open a browser and navigate to `http://localhost:8080`

### Option 2: Docker Development (with Hot Reload)

1. Clone the repository
2. Navigate to the project folder
3. Run the following command:
```bash
docker-compose up app-dev
```
4. Open a browser and navigate to:
   - HTTP: `http://localhost:8080`
   - HTTPS: `https://localhost:8443`

This setup includes:
- Hot reload enabled
- All required databases (MySQL, PostgreSQL, SQL Server)
- Volume mounting for live code changes
- Development environment configuration

### Option 3: Production Docker Setup

1. Clone the repository
2. Navigate to the project folder
3. Run the following command:
```bash
docker-compose up app
```
4. Open a browser and navigate to:
   - HTTP: `http://localhost:8080`
   - HTTPS: `https://localhost:8443`

### Option 4: Individual Docker Container

You can also run just the application container:

```bash
docker build -t project-echo .
docker run -p 8080:8080 -p 8443:443 project-echo
```

## Port Configuration

The application uses the following ports:
- HTTP: 8080
- HTTPS: 8443 (mapped from container port 443)
- MySQL: 3306
- PostgreSQL: 5432
- SQL Server: 1433

## Database Configuration

The project supports multiple database types:
- MySQL (port 3306)
- PostgreSQL (port 5432)
- SQL Server (port 1433)

Default credentials:
- MySQL:
  - Database: echodb
  - Root Password: rootpassword
- PostgreSQL:
  - Database: echodb
  - Password: postgrespassword
- SQL Server:
  - SA Password: SqlServerPassword123!

## Project Structure

- `Pages/` - Contains Razor Pages
- `Controllers/` - Contains API controllers
- `Models/` - Contains data models
- `Views/` - Contains view templates
- `wwwroot/` - Contains static files (CSS, JS, images)
  - `wwwroot/images/` - Contains icon SVGs and world map background
- `docs/` - Project documentation
- `Program.cs` - Entry point for the application
- `Dockerfile` - Production container configuration
- `Dockerfile.dev` - Development container configuration with hot reload
- `docker-compose.yml` - Multi-container orchestration

## Development Features

- Hot Reload support in both local and Docker environments
- Multiple database support
- File upload support (up to 300MB)
- Session management
- Terminal service integration

## World Map Image

The application requires a world map image for the background. Please add a world map image file named `world-map.png` to the `wwwroot/images` directory before running the application.

## Deployment to Render

This project includes Docker configuration for deployment to Render. See the [deployment guide](docs/render-deployment.md) for detailed instructions. 