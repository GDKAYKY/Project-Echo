using System.Data.Common;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Project_Echo.Models;

namespace Project_Echo.Services
{
    public interface IDatabaseTypeDetector
    {
        Task<DatabaseType> DetectTypeAsync(string connectionString);
        Task<DatabaseType> DetectTypeFromFileAsync(IFormFile file);
        Task<bool> TestConnectionAsync(string connectionString, DatabaseType type);
        Task<List<string>> GetTablesAsync(string connectionString, DatabaseType type);
    }

    public class DatabaseTypeDetector : IDatabaseTypeDetector
    {
        public async Task<DatabaseType> DetectTypeAsync(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string cannot be empty");

            // Try each database type
            if (await TestConnectionAsync(connectionString, DatabaseType.SQLite))
                return DatabaseType.SQLite;
            if (await TestConnectionAsync(connectionString, DatabaseType.MySQL))
                return DatabaseType.MySQL;
            if (await TestConnectionAsync(connectionString, DatabaseType.SQLServer))
                return DatabaseType.SQLServer;
            if (await TestConnectionAsync(connectionString, DatabaseType.PostgreSQL))
                return DatabaseType.PostgreSQL;

            throw new InvalidOperationException("Could not determine database type from connection string");
        }

        public async Task<DatabaseType> DetectTypeFromFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null", nameof(file));
            }

            // Read the first few bytes of the file
            using var stream = file.OpenReadStream();
            using var reader = new BinaryReader(stream);
            var header = await Task.Run(() => reader.ReadBytes(16)); // Read first 16 bytes

            // Check for SQLite signature
            if (IsSQLiteFile(header))
            {
                return DatabaseType.SQLite;
            }

            // Check for MySQL signature
            if (IsMySQLFile(header))
            {
                return DatabaseType.MySQL;
            }

            // Check for SQL Server signature
            if (IsSQLServerFile(header))
            {
                return DatabaseType.SQLServer;
            }

            // Check for PostgreSQL signature
            if (IsPostgreSQLFile(header))
            {
                return DatabaseType.PostgreSQL;
            }

            throw new InvalidOperationException("Unsupported or invalid database file format");
        }

        public async Task<bool> TestConnectionAsync(string connectionString, DatabaseType type)
        {
            try
            {
                using var connection = CreateConnection(connectionString, type);
                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetTablesAsync(string connectionString, DatabaseType type)
        {
            var tables = new List<string>();
            using var connection = CreateConnection(connectionString, type);
            await connection.OpenAsync();

            switch (type)
            {
                case DatabaseType.SQLite:
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                    break;

                case DatabaseType.MySQL:
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SHOW TABLES";
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                    break;

                case DatabaseType.SQLServer:
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                    break;

                case DatabaseType.PostgreSQL:
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";
                        using var reader = await command.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                    break;
            }

            return tables;
        }

        private static DbConnection CreateConnection(string connectionString, DatabaseType type)
        {
            return type switch
            {
                DatabaseType.SQLite => new SqliteConnection(connectionString),
                DatabaseType.MySQL => new MySqlConnection(connectionString),
                DatabaseType.SQLServer => new Microsoft.Data.SqlClient.SqlConnection(connectionString),
                DatabaseType.PostgreSQL => new NpgsqlConnection(connectionString),
                _ => throw new ArgumentException($"Unsupported database type: {type}")
            };
        }

        private static bool IsSQLiteFile(byte[] header)
        {
            // SQLite files start with "SQLite format 3\0"
            var sqliteSignature = new byte[] { 0x53, 0x51, 0x4C, 0x69, 0x74, 0x65, 0x20, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x20, 0x33, 0x00 };
            return header.Length >= sqliteSignature.Length && 
                   header.Take(sqliteSignature.Length).SequenceEqual(sqliteSignature);
        }

        private static bool IsMySQLFile(byte[] header)
        {
            // MySQL files start with a specific header
            // This is a simplified check - you might want to add more validation
            return header.Length >= 4 && header[0] == 0xFE && header[1] == 0xFE && header[2] == 0xFE && header[3] == 0xFE;
        }

        private static bool IsSQLServerFile(byte[] header)
        {
            // SQL Server files start with a specific header
            // This is a simplified check - you might want to add more validation
            return header.Length >= 8 && 
                   header[0] == 0x01 && header[1] == 0x0F && 
                   header[4] == 0x00 && header[5] == 0x01;
        }

        private static bool IsPostgreSQLFile(byte[] header)
        {
            // PostgreSQL files start with a specific header
            // This is a simplified check - you might want to add more validation
            return header.Length >= 4 && 
                   header[0] == 0x50 && header[1] == 0x47 && 
                   header[2] == 0x44 && header[3] == 0x42;
        }
    }
} 