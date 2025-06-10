using System.ComponentModel.DataAnnotations;

namespace Project_Echo.Models
{
    public class DatabaseConnectionModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        public DatabaseType Type { get; set; }

        [StringLength(500, ErrorMessage = "Connection string cannot be longer than 500 characters")]
        public string? ConnectionString { get; set; }

        public IFormFile? DatabaseFile { get; set; }

        public List<string> Tables { get; set; } = new();

        public bool IsConnected { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUsed { get; set; }
    }

    public class QueryModel
    {
        [Required(ErrorMessage = "Connection ID is required")]
        public string ConnectionId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Table name is required")]
        public string TableName { get; set; } = string.Empty;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 100;

        public string? WhereClause { get; set; }

        public string? OrderBy { get; set; }
    }

    public class QueryResultModel
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Columns { get; set; } = new();
        public List<object[]> Rows { get; set; } = new();
        public int TotalRows { get; set; }
        public int RowCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public double ExecutionTime { get; set; }
    }
} 