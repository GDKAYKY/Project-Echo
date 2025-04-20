# Project ECHO Deployment Guide

This guide provides instructions for deploying Project ECHO in various environments.

## Deployment Options

Project ECHO can be deployed in several ways:

1. **Self-hosted**: Deploy on your own infrastructure
2. **Cloud-hosted**: Deploy on cloud platforms
3. **Docker**: Deploy using containerization
4. **Kubernetes**: Deploy in a Kubernetes cluster

## Prerequisites

Before deploying Project ECHO, ensure you have:

- .NET 7.0 SDK or Runtime installed on the target server
- Web server (IIS, Nginx, Apache) for production deployments
- SSL certificate for secure communication
- Database for storing application data (optional)
- Network access to systems you want to monitor or connect to

## Self-Hosted Deployment

### Windows Server with IIS

1. **Install Prerequisites**:
   - Install .NET 7.0 SDK or Runtime
   - Install IIS and ASP.NET Core Hosting Bundle

2. **Build the Application**:
   ```
   dotnet publish -c Release -o ./publish
   ```

3. **Configure IIS**:
   - Create a new website in IIS
   - Set the physical path to the `publish` directory
   - Configure application pool to use No Managed Code

4. **Set up URL Rewrite Rules**:
   - Install URL Rewrite module for IIS
   - Add the following rule to web.config:
     ```xml
     <rewrite>
       <rules>
         <rule name="Redirect to HTTPS" stopProcessing="true">
           <match url="(.*)" />
           <conditions>
             <add input="{HTTPS}" pattern="^OFF$" />
           </conditions>
           <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent" />
         </rule>
       </rules>
     </rewrite>
     ```

5. **Configure HTTPS**:
   - Install SSL certificate in IIS
   - Bind the certificate to your website

### Linux with Nginx

1. **Install Prerequisites**:
   - Install .NET 7.0 SDK or Runtime
   - Install Nginx

2. **Build the Application**:
   ```
   dotnet publish -c Release -o ./publish
   ```

3. **Configure Nginx**:
   - Create a new Nginx configuration file:
     ```
     server {
         listen 80;
         server_name your-server.com;
         return 301 https://$host$request_uri;
     }

     server {
         listen 443 ssl;
         server_name your-server.com;

         ssl_certificate /path/to/certificate.crt;
         ssl_certificate_key /path/to/private.key;

         location / {
             proxy_pass http://localhost:5002;
             proxy_http_version 1.1;
             proxy_set_header Upgrade $http_upgrade;
             proxy_set_header Connection keep-alive;
             proxy_set_header Host $host;
             proxy_cache_bypass $http_upgrade;
             proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
             proxy_set_header X-Forwarded-Proto $scheme;
         }
     }
     ```

4. **Set up service**:
   - Create a systemd service file:
     ```
     [Unit]
     Description=Project ECHO

     [Service]
     WorkingDirectory=/path/to/publish
     ExecStart=/usr/bin/dotnet /path/to/publish/Project-Echo.dll
     Restart=always
     RestartSec=10
     KillSignal=SIGINT
     SyslogIdentifier=project-echo
     User=www-data
     Environment=ASPNETCORE_ENVIRONMENT=Production
     Environment=ASPNETCORE_URLS=http://localhost:5002

     [Install]
     WantedBy=multi-user.target
     ```

5. **Start the service**:
   ```
   sudo systemctl enable project-echo
   sudo systemctl start project-echo
   ```

## Cloud-Hosted Deployment

### Microsoft Azure

1. **Create Azure App Service**:
   - Log in to the Azure Portal
   - Create a new App Service with .NET 7.0 runtime

2. **Deploy the Application**:
   - Use Visual Studio's Publish feature, or
   - Set up Azure DevOps pipeline, or
   - Use GitHub Actions

3. **Configure Settings**:
   - Add application settings in the Configuration section
   - Enable HTTPS Only
   - Configure custom domain and SSL

### AWS

1. **Create Elastic Beanstalk Environment**:
   - Use the .NET on Windows Server platform
   - Configure environment type (load balanced or single instance)

2. **Deploy the Application**:
   - Package the application as a deployment bundle
   - Upload through AWS Management Console, or
   - Use AWS CLI or AWS Toolkit

3. **Configure Settings**:
   - Set up environment variables
   - Configure HTTPS
   - Set up custom domain

## Docker Deployment

1. **Create Dockerfile**:
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
   WORKDIR /app
   EXPOSE 80
   EXPOSE 443

   FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
   WORKDIR /src
   COPY ["Project-Echo.csproj", "./"]
   RUN dotnet restore "Project-Echo.csproj"
   COPY . .
   RUN dotnet build "Project-Echo.csproj" -c Release -o /app/build

   FROM build AS publish
   RUN dotnet publish "Project-Echo.csproj" -c Release -o /app/publish

   FROM base AS final
   WORKDIR /app
   COPY --from=publish /app/publish .
   ENTRYPOINT ["dotnet", "Project-Echo.dll"]
   ```

2. **Build Docker Image**:
   ```
   docker build -t project-echo .
   ```

3. **Run Container**:
   ```
   docker run -d -p 80:80 -p 443:443 --name project-echo project-echo
   ```

4. **Docker Compose (Optional)**:
   - Create docker-compose.yml:
     ```yaml
     version: '3'
     services:
       web:
         image: project-echo
         ports:
           - "80:80"
           - "443:443"
         restart: always
         environment:
           - ASPNETCORE_ENVIRONMENT=Production
     ```
   - Run with:
     ```
     docker-compose up -d
     ```

## Kubernetes Deployment

1. **Create Kubernetes Manifests**:

   - Deployment:
     ```yaml
     apiVersion: apps/v1
     kind: Deployment
     metadata:
       name: project-echo
     spec:
       replicas: 3
       selector:
         matchLabels:
           app: project-echo
       template:
         metadata:
           labels:
             app: project-echo
         spec:
           containers:
           - name: project-echo
             image: project-echo:latest
             ports:
             - containerPort: 80
             env:
             - name: ASPNETCORE_ENVIRONMENT
               value: Production
     ```

   - Service:
     ```yaml
     apiVersion: v1
     kind: Service
     metadata:
       name: project-echo
     spec:
       selector:
         app: project-echo
       ports:
       - port: 80
         targetPort: 80
       type: ClusterIP
     ```

   - Ingress:
     ```yaml
     apiVersion: networking.k8s.io/v1
     kind: Ingress
     metadata:
       name: project-echo-ingress
       annotations:
         nginx.ingress.kubernetes.io/ssl-redirect: "true"
     spec:
       tls:
       - hosts:
         - project-echo.example.com
         secretName: project-echo-tls
       rules:
       - host: project-echo.example.com
         http:
           paths:
           - path: /
             pathType: Prefix
             backend:
               service:
                 name: project-echo
                 port:
                   number: 80
     ```

2. **Apply Manifests**:
   ```
   kubectl apply -f deployment.yaml
   kubectl apply -f service.yaml
   kubectl apply -f ingress.yaml
   ```

3. **Create TLS Secret**:
   ```
   kubectl create secret tls project-echo-tls --key privkey.pem --cert cert.pem
   ```

## Configuration

Regardless of deployment method, you should configure:

1. **Application Settings**:
   - Edit appsettings.json or use environment variables
   - Configure database connection strings (if needed)
   - Set logging levels

2. **Authentication**:
   - Configure user authentication method
   - Set up API keys for programmatic access

3. **Networking**:
   - Configure firewall rules to allow required traffic
   - Ensure proper access to systems you want to monitor

## Scaling Considerations

- Use load balancing for high availability
- Consider database scaling for large installations
- Monitor resource usage and scale accordingly
- Use CDN for static content delivery

## Backup and Recovery

- Regularly back up application data
- Create system restore points
- Document recovery procedures
- Test recovery processes periodically

## Monitoring

- Set up application monitoring
- Configure health checks
- Set up alerts for system issues
- Monitor performance metrics 