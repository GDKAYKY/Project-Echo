# Deploying Project Echo to Vercel

This guide walks you through deploying the Project Echo .NET Core API on Vercel.

## Prerequisites

- A [Vercel account](https://vercel.com/signup)
- Your Project Echo codebase in a Git repository (GitHub, GitLab, or Bitbucket)

## Deployment Steps

### 1. Log in to Vercel

- Go to the [Vercel login page](https://vercel.com/login)
- Sign in with your preferred method

### 2. Create a New Project

- Click the "New Project" button on your Vercel dashboard
- Connect your Git repository that contains the Project Echo codebase
- Select the repository containing your code

### 3. Configure Project Settings

- Project Name: Enter a name for your deployment (e.g., "project-echo-api")
- Framework Preset: Select "Other"
- Root Directory: Keep as default (/)
- Build and Output Settings: These are already configured in the `vercel.json` file
- Environment Variables: Add any required environment variables for your API

### 4. Deploy

- Click "Deploy" to start the deployment process
- Vercel will build and deploy your API according to the configuration in `vercel.json`

### 5. Access Your API

Once deployed, your API endpoints will be available at:

- Main API: `https://your-project-name.vercel.app/api`
- Echo API: `https://your-project-name.vercel.app/api/echo`

You can test the main API by visiting:
`https://your-project-name.vercel.app/api?name=YourName`

## How It Works

The Project Echo API is deployed as serverless functions on Vercel:

1. The `api` folder contains C# functions that handle API requests
2. The `vercel.json` file configures how Vercel builds and routes requests to these functions
3. Each `.cs` file in the `api` directory becomes an endpoint

## Troubleshooting

If you encounter any issues:

1. Check the Vercel deployment logs for errors
2. Ensure your `vercel.json` configuration is correct
3. Verify that your API endpoints are properly structured for serverless functions
4. Check that all required dependencies are included in your project 