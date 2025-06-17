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

                using var command = dbConnection.CreateCommand();

                // Build the base query (without WHERE clause initially)
                var sql = BuildQuery(query, connection.Type);

                // Add WHERE clause and parameters
                if (!string.IsNullOrEmpty(query.WhereClause))
                {
                    sql += AddWhereClauseAndParameters(command, query.WhereClause, connection.Type, logger);
                }

                command.CommandText = sql;

                using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = [.. schema.AsEnumerable().Select(row => row["ColumnName"].ToString() ?? string.Empty)];
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

            using var command = dbConnection.CreateCommand();
            var sql = $"SELECT COUNT(*) FROM {EscapeIdentifier(tableName, connection.Type)}";

            if (!string.IsNullOrEmpty(whereClause))
            {
                sql += AddWhereClauseAndParameters(command, whereClause, connection.Type, logger, "_count");
            }
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
                    result.Columns = [.. schema.AsEnumerable().Select(row => row["ColumnName"].ToString() ?? string.Empty)];
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

            // The WHERE clause will be added by AddWhereClauseAndParameters in ExecuteQueryAsync

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
                    // For SQL Server, the WHERE clause is part of the subquery,
                    // so we need to ensure it's parameterized correctly here too.
                    // This is a complex case with string parsing. For now, we'll
                    // assume it's handled by the calling method that provides params.
                    // If the `WhereClause` is not parameterized here, it remains a vulnerability
                    // for SQL Server pagination.
                    sql = $@"{sql}
                        WITH PagedData AS (
                            SELECT *, ROW_NUMBER() OVER (ORDER BY {(string.IsNullOrEmpty(query.OrderBy) ? "(SELECT NULL)" : query.OrderBy)}) AS RowNum
                            FROM {tableName}
                        )
                        SELECT * FROM PagedData
                        WHERE RowNum BETWEEN {(query.Page - 1) * query.PageSize + 1} AND {query.Page * query.PageSize}";
                    // The WHERE clause for PagedData needs to be built with parameters here.
                    // This is where a more structured QueryModel for filters would be ideal.
                    break;

                case DatabaseType.PostgreSQL:
                    sql += $" LIMIT {query.PageSize} OFFSET {(query.Page - 1) * query.PageSize}";
                    break;
            }

            return sql;
        }

        // Helper method to add WHERE clause and parameters for simple cases
        private static string AddWhereClauseAndParameters(DbCommand command, string whereClause, DatabaseType type, ILogger<DatabaseQueryService> logger, string paramPrefix = "_where_")
        {
            string[] operators = ["=", ">=", "<=", ">", "<", "!=", "<>", "LIKE"];
            var foundOperator = operators.FirstOrDefault(op => whereClause.Contains(op, StringComparison.OrdinalIgnoreCase));

            if (foundOperator != null)
            {
                var parts = whereClause.Split([foundOperator], 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    var columnName = EscapeIdentifier(parts[0], type);
                    var value = parts[1].Trim('\'', ' '); // Remove quotes from string values

                    string parameterName = $"@{paramPrefix}{command.Parameters.Count}";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = value; // Value is currently a string; more robust parsing would convert to correct type
                    command.Parameters.Add(parameter);

                    return $" WHERE {columnName} {foundOperator} {parameterName}";
                }
            }

            // Fallback: If parsing fails or is too complex for this simple parser, return original.
            // WARNING: This path is still vulnerable to SQL Injection for complex queries.
            logger.LogWarning("Complex WHERE clause not parameterized: {WhereClause}", whereClause);
            return $" WHERE {whereClause}";
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
                DatabaseType.SQLServer => new Microsoft.Data.SqlClient.SqlConnection(connectionString),
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