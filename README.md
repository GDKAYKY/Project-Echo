# Project ECHO

This is a .NET Core web application that recreates the UI shown in the Figma design.

## Prerequisites

- .NET 7.0 SDK or later
- Visual Studio 2022 or Visual Studio Code

## How to Run

1. Clone the repository
2. Navigate to the project folder
3. Run the following command:

```
dotnet run
```

4. Open a browser and navigate to `https://localhost:5003` or `http://localhost:5002`

## Project Structure

- `Pages/` - Contains Razor Pages
- `wwwroot/` - Contains static files (CSS, JS, images)
  - `wwwroot/images/` - Contains icon SVGs and world map background
- `Program.cs` - Entry point for the application

## World Map Image

The application requires a world map image for the background. Please add a world map image file named `world-map.png` to the `wwwroot/images` directory before running the application. 