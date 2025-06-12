namespace Project_Echo.Models
{
    public enum DatabaseType
    {
        MySQL,
        PostgreSQL,
        SQLServer,
        Oracle,
        SQLite
    }

    public class DatabaseConnection
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public DatabaseType Type { get; set; }
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Database { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUsed { get; set; }
    }

    public class DatabaseUploadModel
    {
        public string Name { get; set; } = string.Empty;
        public DatabaseType Type { get; set; }
        public IFormFile? DatabaseFile { get; set; }
        public string? ConnectionString { get; set; }
    }

    public class DatabaseViewModel
    {
        public List<DatabaseConnection> Connections { get; set; } = new List<DatabaseConnection>();
        public DatabaseUploadModel UploadModel { get; set; } = new DatabaseUploadModel();
    }
} 