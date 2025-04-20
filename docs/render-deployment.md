# Deploying Project Echo to Render using Docker

This guide walks you through deploying Project Echo on Render using Docker.

## Prerequisites

- A [Render account](https://render.com)
- Your Project Echo codebase in a Git repository (GitHub, GitLab, or Bitbucket)

## Deployment Steps

### 1. Log in to Render

- Go to the [Render dashboard](https://dashboard.render.com)
- Sign in with your preferred method

### 2. Create a New Web Service

- Click the "New +" button in the top right
- Select "Web Service" from the dropdown menu

### 3. Connect Your Repository

- Choose the repository where your Project Echo code is stored
- Authorize Render to access your repository if prompted

### 4. Configure Your Web Service

- Name: Enter a name for your service (e.g., "project-echo")
- Environment: Select "Docker"
- Branch: Choose the branch you want to deploy (usually "main" or "master")
- Root Directory: Only change if your Dockerfile is not in the root of your repository
- Region: Choose the server region closest to your users
- Instance Type: Select an appropriate plan (Free is good for testing)

### 5. Advanced Settings (Optional)

- Environment Variables: Add any environment variables your application needs
- Health Check Path: Set to `/` or another endpoint that returns a 200 status
- Auto-Deploy: Enable if you want Render to automatically deploy when you push changes to your repo

### 6. Create Web Service

- Click "Create Web Service" to start the deployment process
- Render will build your Docker image and deploy your application

## How It Works

The deployment process follows these steps:

1. Render pulls your code from the Git repository
2. It uses the Dockerfile to build a Docker image of your application
3. The image is deployed as a container on Render's infrastructure
4. Your application is accessible via a Render subdomain (e.g., `project-echo.onrender.com`)

## Troubleshooting

If you encounter issues:

1. Check the build logs on the Render dashboard
2. Verify your Dockerfile is correctly configured for a .NET 7.0 application
3. Make sure all necessary files are committed to your repository
4. Check that your application is configured to listen on the correct port

## Port Configuration

Render automatically assigns a port to your container via the `PORT` environment variable. 
In your Program.cs, ensure you're binding to the correct port with code like:

```csharp
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
```

Make sure to add this before building the application. 