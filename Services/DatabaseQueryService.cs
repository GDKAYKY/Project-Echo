using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Project_Echo.Models;
using System.Data.SqlClient;

namespace Project_Echo.Services
{
    public interface IDatabaseQueryService
    {
        Task<QueryResultModel> ExecuteQueryAsync(QueryModel query, DatabaseConnectionModel connection);
        Task<int> GetTotalRowsAsync(string tableName, string? whereClause, DatabaseConnectionModel connection);
        Task<QueryResultModel> ExecuteRawQueryAsync(string rawQuery, DatabaseConnectionModel connection);
    }

    public class DatabaseQueryService(ILogger<DatabaseQueryService> logger) : IDatabaseQueryService
    {
        public async Task<QueryResultModel> ExecuteQueryAsync(QueryModel query, DatabaseConnectionModel connection)
        {
            var result = new QueryResultModel
            {
                Success = true,
                Page = query.Page,
                PageSize = query.PageSize
            };

            try
            {
                var startTime = DateTime.UtcNow;

                using var dbConnection = CreateConnection(connection);
                await dbConnection.OpenAsync();

                // Build the query
                var sql = BuildQuery(query, connection.Type);
                using var command = dbConnection.CreateCommand();
                command.CommandText = sql;

                // Add parameters if needed
                if (!string.IsNullOrEmpty(query.WhereClause))
                {
                    // TODO: Add parameter handling for WHERE clause
                }

                using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = schema.AsEnumerable()
                        .Select(row => row["ColumnName"].ToString() ?? string.Empty)
                        .ToList();
                }

                var rows = new List<object[]>();
                while (await reader.ReadAsync())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    rows.Add(values);
                }

                result.Rows = rows;
                result.TotalRows = await GetTotalRowsAsync(query.TableName, query.WhereClause, connection);
                result.TotalPages = (int)Math.Ceiling(result.TotalRows / (double)query.PageSize);
                result.ExecutionTime = (DateTime.UtcNow - startTime).TotalSeconds;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing query");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<int> GetTotalRowsAsync(string tableName, string? whereClause, DatabaseConnectionModel connection)
        {
            using var dbConnection = CreateConnection(connection);
            await dbConnection.OpenAsync();

            var sql = $"SELECT COUNT(*) FROM {EscapeIdentifier(tableName, connection.Type)}";
            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += $" WHERE {whereClause}";
            }

            using var command = dbConnection.CreateCommand();
            command.CommandText = sql;

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<QueryResultModel> ExecuteRawQueryAsync(string rawQuery, DatabaseConnectionModel connection)
        {
            var result = new QueryResultModel
            {
                Success = true,
                Page = 1, // Raw queries typically don't have pagination unless explicitly added in the query
                PageSize = int.MaxValue // Set to max value as pagination is not handled by this method
            };

            try
            {
                var startTime = DateTime.UtcNow;

                using var dbConnection = CreateConnection(connection);
                await dbConnection.OpenAsync();

                using var command = dbConnection.CreateCommand();
                command.CommandText = rawQuery;

                logger.LogInformation("Executing raw query: {RawQuery}", rawQuery);

                using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = schema.AsEnumerable()
                        .Select(row => row["ColumnName"].ToString() ?? string.Empty)
                        .ToList();
                }

                var rows = new List<object[]>();
                while (await reader.ReadAsync())
                {
                    var values = new object[reader.FieldCount];
                    reader.GetValues(values);
                    rows.Add(values);
                }

                result.Rows = rows;
                result.RowCount = rows.Count; // Set rowCount for raw queries
                result.ExecutionTime = (DateTime.UtcNow - startTime).TotalSeconds;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing raw query");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private static string BuildQuery(QueryModel query, DatabaseType type)
        {
            var tableName = EscapeIdentifier(query.TableName, type);
            var sql = $"SELECT * FROM {tableName}";

            if (!string.IsNullOrEmpty(query.WhereClause))
            {
                sql += $" WHERE {query.WhereClause}";
            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                sql += $" ORDER BY {query.OrderBy}";
            }

            // Add pagination
            switch (type)
            {
                case DatabaseType.SQLite:
                case DatabaseType.MySQL:
                    sql += $" LIMIT {query.PageSize} OFFSET {(query.Page - 1) * query.PageSize}";
                    break;

                case DatabaseType.SQLServer:
                    sql = $@"
                        WITH PagedData AS (
                            SELECT *, ROW_NUMBER() OVER (ORDER BY {(string.IsNullOrEmpty(query.OrderBy) ? "(SELECT NULL)" : query.OrderBy)}) AS RowNum
                            FROM {tableName}
                            {(string.IsNullOrEmpty(query.WhereClause) ? "" : $"WHERE {query.WhereClause}")}
                        )
                        SELECT * FROM PagedData
                        WHERE RowNum BETWEEN {(query.Page - 1) * query.PageSize + 1} AND {query.Page * query.PageSize}";
                    break;

                case DatabaseType.PostgreSQL:
                    sql += $" LIMIT {query.PageSize} OFFSET {(query.Page - 1) * query.PageSize}";
                    break;
            }

            return sql;
        }

        private static DbConnection CreateConnection(DatabaseConnectionModel connection)
        {
            var connectionString = connection.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is required");
            }

            return connection.Type switch
            {
                DatabaseType.SQLite => new SqliteConnection(connectionString),
                DatabaseType.MySQL => new MySqlConnection(connectionString),
                DatabaseType.SQLServer => new SqlConnection(connectionString),
                DatabaseType.PostgreSQL => new NpgsqlConnection(connectionString),
                _ => throw new ArgumentException($"Unsupported database type: {connection.Type}")
            };
        }

        private static string EscapeIdentifier(string identifier, DatabaseType type)
        {
            return type switch
            {
                DatabaseType.SQLite => $"[{identifier}]",
                DatabaseType.MySQL => $"`{identifier}`",
                DatabaseType.SQLServer => $"[{identifier}]",
                DatabaseType.PostgreSQL => $"\"{identifier}\"",
                _ => identifier
            };
        }
    }
} 