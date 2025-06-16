# Getting Started with Project ECHO

This guide will help you install and set up Project ECHO on your system.

## Prerequisites

Before installing Project ECHO, ensure your system meets the following requirements:

- .NET 9.0 SDK
- Modern web browser (Chrome, Firefox, Edge, Safari)
- Git (for cloning the repository)
- Visual Studio 2022 or Visual Studio Code (recommended for development)
- Docker and Docker Compose (for containerized deployment)
- Tailscale account and client

## Tailscale Setup

Project ECHO is designed to run on Tailscale for secure network access. Follow these steps to set up Tailscale:

1. Install Tailscale on your system:
   - Windows: Download from [Tailscale website](https://tailscale.com/download)
   - Linux: Follow [Linux installation guide](https://tailscale.com/download/linux)
   - macOS: Download from [Mac App Store](https://apps.apple.com/app/tailscale/id1475387142)

2. Sign in to Tailscale:
   ```bash
   tailscale up
   ```

3. Note your Tailscale IP address:
   ```bash
   tailscale ip
   ```

4. Configure your firewall to allow Tailscale traffic:
   - Allow UDP port 41641
   - Allow TCP port 41641

## Installation

### Option 1: Clone the Repository

1. Open a terminal or command prompt
2. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/Project-Echo.git
   ```
3. Navigate to the project directory:
   ```bash
   cd Project-Echo
   ```

### Option 2: Download the Release

1. Download the latest release from the [Releases page](https://github.com/yourusername/Project-Echo/releases)
2. Extract the ZIP file to your desired location

## Running the Application

### Using Docker (Recommended)

1. Navigate to the project directory
2. Run the application using Docker Compose:
   ```bash
   docker-compose up app
   ```
3. Access the application through your Tailscale IP:
   - HTTP: `http://<your-tailscale-ip>:8080`
   - HTTPS: `https://<your-tailscale-ip>:8443`

### Using the Command Line

1. Navigate to the project directory
2. Run the application:
   ```bash
   dotnet run
   ```
3. Access the application through your Tailscale IP:
   - HTTP: `http://<your-tailscale-ip>:8080`
   - HTTPS: `https://<your-tailscale-ip>:8443`

### Using Visual Studio

1. Open the `Project-Echo.sln` solution file in Visual Studio
2. Press F5 or click the "Run" button
3. Access the application through your Tailscale IP

### Using Visual Studio Code

1. Open the project folder in Visual Studio Code
2. Make sure you have the C# extension installed
3. Press F5 or select "Run > Start Debugging"
4. Access the application through your Tailscale IP

## Initial Configuration

After starting the application for the first time, you'll need to:

1. Configure your database connections (if using the database query feature)
2. Set up SSH credentials (if using the SSH terminal feature)
3. Configure remote desktop connections (if using the remote access feature)
4. Verify Tailscale connectivity between all nodes

See the [User Guide](/Documentation/user-guide) for detailed instructions on these configuration steps.

## Troubleshooting

If you encounter issues during installation or startup:
1. Verify Tailscale connectivity:
   ```bash
   tailscale status
   ```
2. Check if ports are accessible:
   ```bash
   tailscale ping <target-ip>
   ```
3. Refer to the [Troubleshooting](troubleshooting.md) guide
4. Open an issue on the GitHub repository 