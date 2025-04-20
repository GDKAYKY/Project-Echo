# Getting Started with Project ECHO

This guide will help you install and set up Project ECHO on your system.

## Prerequisites

Before installing Project ECHO, ensure your system meets the following requirements:

- .NET 7.0 SDK or later
- Modern web browser (Chrome, Firefox, Edge, Safari)
- Git (for cloning the repository)
- Visual Studio 2022 or Visual Studio Code (recommended for development)

## Installation

### Option 1: Clone the Repository

1. Open a terminal or command prompt
2. Clone the repository:
   ```
   git clone https://github.com/yourusername/Project-Echo.git
   ```
3. Navigate to the project directory:
   ```
   cd Project-Echo
   ```

### Option 2: Download the Release

1. Download the latest release from the [Releases page](https://github.com/yourusername/Project-Echo/releases)
2. Extract the ZIP file to your desired location

## Running the Application

### Using the Command Line

1. Navigate to the project directory
2. Run the application:
   ```
   dotnet run
   ```
3. Open your web browser and navigate to:
   - `https://localhost:5003` (HTTPS)
   - `http://localhost:5002` (HTTP)

### Using Visual Studio

1. Open the `Project-Echo.sln` solution file in Visual Studio
2. Press F5 or click the "Run" button
3. The application will start and open in your default web browser

### Using Visual Studio Code

1. Open the project folder in Visual Studio Code
2. Make sure you have the C# extension installed
3. Press F5 or select "Run > Start Debugging"
4. The application will start and open in your default web browser

## Initial Configuration

After starting the application for the first time, you'll need to:

1. Configure your database connections (if using the database query feature)
2. Set up SSH credentials (if using the SSH terminal feature)
3. Configure remote desktop connections (if using the remote access feature)

See the [User Guide](user-guide.md) for detailed instructions on these configuration steps.

## Troubleshooting

If you encounter issues during installation or startup, please refer to the [Troubleshooting](troubleshooting.md) guide or open an issue on the GitHub repository. 