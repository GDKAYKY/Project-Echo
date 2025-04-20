# Project ECHO Troubleshooting Guide

This guide provides solutions to common issues you might encounter when using Project ECHO.

## Installation Issues

### .NET SDK Version Mismatch

**Problem**: Error message indicating incompatible .NET SDK version.

**Solution**:
1. Verify your installed .NET SDK version: `dotnet --version`
2. Install the correct version (.NET 7.0) from [Microsoft's .NET download page](https://dotnet.microsoft.com/download/dotnet/7.0)
3. If multiple SDK versions are installed, use global.json to specify the version:
   ```json
   {
     "sdk": {
       "version": "7.0.0"
     }
   }
   ```

### Database Connection Failure

**Problem**: Unable to connect to a database configured in Project ECHO.

**Solution**:
1. Verify database credentials are correct
2. Check that the database server is running and accessible from your network
3. Ensure firewall rules allow connections to the database port
4. Verify connection string format in appsettings.json

## Application Startup Issues

### Port Already in Use

**Problem**: Error message indicating the port is already in use.

**Solution**:
1. Identify which process is using the port:
   - Windows: `netstat -ano | findstr :5002`
   - Linux: `lsof -i :5002`
2. Terminate the process or change Project ECHO's port in `appsettings.json`:
   ```json
   {
     "Kestrel": {
       "Endpoints": {
         "Http": {
           "Url": "http://localhost:5010"
         },
         "Https": {
           "Url": "https://localhost:5011"
         }
       }
     }
   }
   ```

### Application Crashes on Startup

**Problem**: The application terminates immediately after starting.

**Solution**:
1. Check logs in the `logs` directory
2. Verify all dependencies are installed
3. Ensure appsettings.json is valid JSON
4. Run with development environment to see detailed errors:
   ```
   set ASPNETCORE_ENVIRONMENT=Development  # Windows
   export ASPNETCORE_ENVIRONMENT=Development  # Linux/macOS
   dotnet run
   ```

## User Interface Issues

### UI Not Loading Correctly

**Problem**: The interface appears broken or stylesheets are not loading.

**Solution**:
1. Clear your browser cache
2. Check browser console for errors
3. Verify that the wwwroot directory and its contents are properly deployed
4. Ensure you're using a supported browser (Chrome, Firefox, Edge, Safari)

### Sidebar Icons Not Displaying

**Problem**: The sidebar icons in the application are missing.

**Solution**:
1. Check that all SVG files are present in the `wwwroot/images` directory
2. Verify proper file permissions on the image files
3. Check network requests in browser developer tools to identify 404 errors
4. Ensure proper MIME types are configured for SVG files in web server

## Feature-Specific Issues

### SSH Terminal Connection Failure

**Problem**: Unable to establish SSH connections from the terminal page.

**Solution**:
1. Verify the server is reachable using an external SSH client
2. Check that the SSH port (usually 22) is open on the target server
3. Verify credentials are correct
4. Check for specific error messages in the browser console
5. Ensure WebSocket connections are allowed by your proxy/firewall

### Database Query Errors

**Problem**: SQL queries fail to execute properly.

**Solution**:
1. Test the query directly on the database to verify syntax
2. Check database user permissions
3. Verify connection string is correct
4. For timeout errors, increase the command timeout in settings
5. Check for proper parameter binding in parameterized queries

### Remote Desktop Connection Issues

**Problem**: Unable to connect to remote desktops.

**Solution**:
1. Verify the target machine is powered on and accessible
2. Check that remote desktop is enabled on the target machine
3. Ensure proper credentials are being used
4. Verify port forwarding if connecting through firewalls
5. Check that required protocols (RDP/VNC) are not blocked

## Network Analysis Issues

### Network Tools Not Working

**Problem**: Network analysis tools (ping, traceroute, etc.) fail to run.

**Solution**:
1. Verify that the target is reachable from the server
2. Check that ICMP is not blocked by firewalls for ping/traceroute
3. Ensure the server has proper permissions to run network tools
4. For Linux deployments, check that the application has CAP_NET_RAW capability

## Performance Issues

### Slow Application Response

**Problem**: The application becomes slow or unresponsive.

**Solution**:
1. Check server resource usage (CPU, memory, disk I/O)
2. Verify database query performance
3. Check network latency between server components
4. Consider scaling resources (vertical or horizontal)
5. Review application logs for errors or warnings
6. Implement caching for frequently accessed data

### Memory Leaks

**Problem**: Server memory usage continuously increases over time.

**Solution**:
1. Restart the application as a temporary fix
2. Check for long-running operations that may be causing memory leaks
3. Consider implementing a scheduled application restart
4. Enable memory dump on OOM for analysis
5. Update to the latest version as it might include fixes

## Security Issues

### Certificate Errors

**Problem**: Browser shows SSL/TLS certificate warnings.

**Solution**:
1. Ensure the certificate is valid and not expired
2. Verify the certificate is issued for the correct domain name
3. Check that the certificate chain is complete
4. Verify certificate is properly installed on the web server

### Authentication Failures

**Problem**: Unable to log in despite correct credentials.

**Solution**:
1. Reset user password
2. Check for account lockout due to failed attempts
3. Verify authentication provider configuration
4. Clear browser cookies and cache
5. Check server time synchronization (important for token validation)

## Debugging Techniques

### Enabling Detailed Logs

To enable detailed logging, modify `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

### Checking Application Logs

1. Log files are located in the `logs` directory
2. For Windows IIS deployments, check Event Viewer
3. For Linux systemd deployments: `journalctl -u project-echo`
4. For Docker deployments: `docker logs project-echo`

### Using Developer Tools

1. Open browser developer tools (F12)
2. Check Console tab for JavaScript errors
3. Check Network tab for failed requests
4. Use Application tab to inspect cookies and storage

## Getting Support

If you're unable to resolve your issue using this guide:

1. Search for similar issues in our [GitHub Issues](https://github.com/yourusername/Project-Echo/issues)
2. Check the [FAQ](faq.md) for additional guidance
3. Open a new issue with details about:
   - Your deployment environment
   - Steps to reproduce the issue
   - Relevant logs or error messages
   - Screenshots if applicable 