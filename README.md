# Project ECHO

This is a .NET Core web application that recreates the UI shown in the Figma design.

## Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or Visual Studio Code
- Docker (for containerized deployment)

## How to Run Locally

1. Clone the repository
2. Navigate to the project folder
3. Run the following command:

```
dotnet run
```

4. Open a browser and navigate to `https://localhost:5003` or `http://localhost:5002`

## Docker Deployment

You can also run the application using Docker:

```
docker build -t project-echo .
docker run -p 8080:80 project-echo
```

Then access the application at `http://localhost:8080`

## Deployment to Render

This project includes Docker configuration for deployment to Render. See the [deployment guide](docs/render-deployment.md) for detailed instructions.

## Project Structure

- `Pages/` - Contains Razor Pages
- `Controllers/` - Contains API controllers
- `Models/` - Contains data models
- `Views/` - Contains view templates
- `wwwroot/` - Contains static files (CSS, JS, images)
  - `wwwroot/images/` - Contains icon SVGs and world map background
- `docs/` - Project documentation
- `Program.cs` - Entry point for the application
- `Dockerfile` - Container configuration for Docker and Render deployment

## World Map Image

The application requires a world map image for the background. Please add a world map image file named `world-map.png` to the `wwwroot/images` directory before running the application. 