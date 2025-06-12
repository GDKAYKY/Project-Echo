using Microsoft.EntityFrameworkCore;
using Project_Echo.Models;
using System.Text.Json;

namespace Project_Echo.Services
{
    public interface IDatabaseService
    {
        Task<DatabaseConnection> UploadDatabaseAsync(DatabaseUploadModel model);
        Task<List<DatabaseConnection>> GetConnectionsAsync();
        Task<DatabaseConnection?> GetConnectionAsync(string id);
        Task<bool> TestConnectionAsync(DatabaseConnection connection);
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly string _uploadPath;
        private readonly List<DatabaseConnection> _connections = new();
        private readonly string _connectionsFilePath;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IWebHostEnvironment environment, IConfiguration configuration, ILogger<DatabaseService> logger)
        {
            _uploadPath = configuration["DatabaseStoragePath"]!;
            _connectionsFilePath = Path.Combine(_uploadPath, "connections.json");
            
            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
            LoadConnections();
            _logger = logger;
        }

        public async Task<DatabaseConnection> UploadDatabaseAsync(DatabaseUploadModel model)
        {
            var connection = new DatabaseConnection
            {
                Name = model.Name,
                Type = model.Type,
                CreatedAt = DateTime.UtcNow
            };

            if (model.DatabaseFile != null)
            {
                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.DatabaseFile.FileName)}";
                var filePath = Path.Combine(_uploadPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.DatabaseFile.CopyToAsync(stream);
                }

                // Store file path in connection
                if (connection.Type == DatabaseType.SQLite)
                {
                    connection.Host = $"Data Source={filePath}";
                }
                else
                {
                    connection.Host = filePath;
                }
            }
            else if (!string.IsNullOrEmpty(model.ConnectionString))
            {
                // Parse connection string to extract connection details
                var builder = new DbContextOptionsBuilder();
                switch (model.Type)
                {
                    case DatabaseType.MySQL:
                        builder.UseMySql(model.ConnectionString, await ServerVersion.AutoDetectAsync(model.ConnectionString));
                        break;
                    case DatabaseType.PostgreSQL:
                        builder.UseNpgsql(model.ConnectionString);
                        break;
                    case DatabaseType.SQLServer:
                        builder.UseSqlServer(model.ConnectionString);
                        break;
                    case DatabaseType.Oracle:
                        builder.UseOracle(model.ConnectionString);
                        break;
                    case DatabaseType.SQLite:
                        builder.UseSqlite(model.ConnectionString);
                        break;
                }

                // Store connection string
                connection.Host = model.ConnectionString;
            }

            // Add to in-memory collection
            _connections.Add(connection);
            SaveConnections();
            return connection;
        }

        public Task<List<DatabaseConnection>> GetConnectionsAsync()
        {
            // Return a copy of the connections list
            return Task.FromResult(_connections.ToList());
        }

        public Task<DatabaseConnection?> GetConnectionAsync(string id)
        {
            // Find connection by ID
            var connection = _connections.FirstOrDefault(c => c.Id == id);
            return Task.FromResult(connection);
        }

        public async Task<bool> TestConnectionAsync(DatabaseConnection connection)
        {
            try
            {
                var builder = new DbContextOptionsBuilder();
                switch (connection.Type)
                {
                    case DatabaseType.MySQL:
                        builder.UseMySql(connection.Host, await ServerVersion.AutoDetectAsync(connection.Host));
                        break;
                    case DatabaseType.PostgreSQL:
                        builder.UseNpgsql(connection.Host);
                        break;
                    case DatabaseType.SQLServer:
                        builder.UseSqlServer(connection.Host);
                        break;
                    case DatabaseType.Oracle:
                        builder.UseOracle(connection.Host);
                        break;
                    case DatabaseType.SQLite:
                        builder.UseSqlite(connection.Host);
                        break;
                }

                // Create a test context and try to connect
                using var context = new DbContext(builder.Options);
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        private void LoadConnections()
        {
            if (System.IO.File.Exists(_connectionsFilePath))
            {
                try
                {
                    var jsonString = System.IO.File.ReadAllText(_connectionsFilePath);
                    var loadedConnections = JsonSerializer.Deserialize<List<DatabaseConnection>>(jsonString);
                    if (loadedConnections != null)
                    {
                        _connections.AddRange(loadedConnections);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading connections from file.");
                }
            }
        }

        private void SaveConnections()
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(_connections);
                System.IO.File.WriteAllText(_connectionsFilePath, jsonString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving connections to file.");
            }
        }
    }
} 