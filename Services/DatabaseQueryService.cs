using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Npgsql;
using Project_Echo.Models;

namespace Project_Echo.Services
{
    public interface IDatabaseQueryService
    {
        Task<QueryResultModel> ExecuteQueryAsync(QueryModel query, DatabaseConnectionModel connection);
        Task<int> GetTotalRowsAsync(string tableName, string? whereClause, DatabaseConnectionModel connection);
        Task<QueryResultModel> ExecuteRawQueryAsync(string rawQuery, DatabaseConnectionModel connection);
        
        // Novos métodos para suporte a JSON
        Task<QueryResultModel> QueryJsonFieldAsync(string tableName, string jsonColumn, string jsonPath, 
            bool isNestedPath, string jsonOperator, string jsonValue, DatabaseConnectionModel connection);
        Task<QueryResultModel> QueryJsonArrayContainsAsync(string tableName, string jsonColumn, 
            string arrayPath, string value, DatabaseConnectionModel connection);
        Task<QueryResultModel> ExecuteComplexJsonQueryAsync(string rawJsonQuery, DatabaseConnectionModel connection);
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

                await using var dbConnection = CreateConnection(connection);
                await dbConnection.OpenAsync();

                await using var command = dbConnection.CreateCommand();

                // Build the base query (without WHERE clause initially)
                var sql = BuildQuery(query, connection.Type);

                // Add WHERE clause and parameters
                if (!string.IsNullOrEmpty(query.WhereClause))
                {
                    sql += AddWhereClauseAndParameters(command, query.WhereClause, connection.Type, logger);
                }

                command.CommandText = sql;

                await using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = [.. schema.AsEnumerable().Select(row => row["ColumnName"].ToString() ?? string.Empty)];
                }

                result.Rows = await ReadDataFromReader(reader);
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
            await using var dbConnection = CreateConnection(connection);
            await dbConnection.OpenAsync();

            await using var command = dbConnection.CreateCommand();
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

                await using var dbConnection = CreateConnection(connection);
                await dbConnection.OpenAsync();

                await using var command = dbConnection.CreateCommand();
                command.CommandText = rawQuery;

                logger.LogInformation("Executing raw query: {RawQuery}", rawQuery);

                await using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = [.. schema.AsEnumerable().Select(row => row["ColumnName"].ToString() ?? string.Empty)];
                }

                result.Rows = await ReadDataFromReader(reader);
                result.RowCount = result.Rows.Count; // Set rowCount for raw queries
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

        private static string AddWhereClauseAndParameters(DbCommand command, string whereClause, DatabaseType type, ILogger<DatabaseQueryService> logger, string paramPrefix = "_where_")
        {
            string[] operators = ["=", ">=", "<=", ">", "<", "!=", "<>", "LIKE"];
            var foundOperator = operators.FirstOrDefault(op => whereClause.Contains(op, StringComparison.OrdinalIgnoreCase));

            if (foundOperator != null)
            {
                // Verificar se esta é uma consulta JSON no PostgreSQL
                var isJsonQuery = type == DatabaseType.PostgreSQL && (
                    whereClause.Contains("->") || 
                    whereClause.Contains("->>") || 
                    whereClause.Contains("#>") || 
                    whereClause.Contains("#>>")
                );

                if (isJsonQuery)
                {
                    // Tratamento especial para consultas JSON
                    return HandlePostgresJsonWhereClause(command, whereClause, foundOperator, paramPrefix, logger);
                }

                var parts = whereClause.Split([foundOperator], 2, StringSplitOptions.TrimEntries);
                if (parts.Length == 2)
                {
                    var columnName = EscapeIdentifier(parts[0], type);
                    var value = parts[1].Trim('\'', ' '); // Remove quotes from string values

                    string parameterName = $"@{paramPrefix}{command.Parameters.Count}";

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = value;
                    command.Parameters.Add(parameter);

                    return $" WHERE {columnName} {foundOperator} {parameterName}";
                }
            }

            // Fallback para consultas complexas
            logger.LogWarning("Cláusula WHERE complexa não parametrizada: {WhereClause}", whereClause);
            return $" WHERE {whereClause}";
        }

        private static string HandlePostgresJsonWhereClause(DbCommand command, string whereClause, string foundOperator, string paramPrefix, ILogger<DatabaseQueryService> logger)
        {
            try
            {
                // Dividir a expressão pelo operador encontrado
                var parts = whereClause.Split([foundOperator], 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2)
                {
                    throw new ArgumentException("Formato inválido para consulta JSON");
                }

                // A parte à esquerda contém a expressão JSON (ex: data->'campo')
                var jsonExpression = parts[0];
                
                // A parte à direita contém o valor a ser comparado
                var value = parts[1].Trim('\'', ' ');

                // Criar parâmetro para o valor
                string parameterName = $"@{paramPrefix}{command.Parameters.Count}";
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value;
                command.Parameters.Add(parameter);

                // Retornar a cláusula WHERE com a expressão JSON
                return $" WHERE {jsonExpression} {foundOperator} {parameterName}";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar consulta JSON: {WhereClause}", whereClause);
                // Fallback para consulta não parametrizada
                logger.LogWarning("Consulta JSON não parametrizada: {WhereClause}", whereClause);
                return $" WHERE {whereClause}";
            }
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

        private static async Task<List<object[]>> ReadDataFromReader(DbDataReader reader)
        {
            var rows = new List<object[]>();
            while (await reader.ReadAsync())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                rows.Add(values);
            }
            return rows;
        }
        
        // Novo método para consulta em campos JSON
        public async Task<QueryResultModel> QueryJsonFieldAsync(
            string tableName,
            string jsonColumn,
            string jsonPath,
            bool isNestedPath,
            string jsonOperator,
            string jsonValue,
            DatabaseConnectionModel connection)
        {
            if (connection.Type != DatabaseType.PostgreSQL)
            {
                throw new NotSupportedException("Consulta em campos JSON é suportada apenas para PostgreSQL");
            }
            
            var result = new QueryResultModel { Success = true };
            
            try
            {
                using var dbConnection = new NpgsqlConnection(connection.ConnectionString);
                await dbConnection.OpenAsync();
                
                using var command = dbConnection.CreateCommand();
                
                // Determinar o operador JSON correto
                string jsonPathOperator = isNestedPath
                    ? (jsonOperator == "=" ? """#>>""" : "#>")
                    : (jsonOperator == "=" ? "->>" : "->");
                
                // Formatar o caminho JSON corretamente
                string formattedPath;
                if (isNestedPath)
                {
                    // Para operadores #> e #>>, o caminho deve estar no formato '{campo,subcampo}'
                    var pathParts = jsonPath.Split(',').Select(p => p.Trim());
                    formattedPath = $"'{{{string.Join(",", pathParts)}}}'";
                }
                else
                {
                    // Para operadores -> e ->>, o caminho é apenas 'campo'
                    formattedPath = $"'{jsonPath.Trim()}'";
                }
                
                // Construir a consulta
                var sql = $"SELECT * FROM {EscapeIdentifier(tableName, DatabaseType.PostgreSQL)} " +
                          $"WHERE {EscapeIdentifier(jsonColumn, DatabaseType.PostgreSQL)}{jsonPathOperator}{formattedPath} {jsonOperator} @jsonValue";
                
                command.CommandText = sql;
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@jsonValue";
                parameter.Value = jsonValue;
                command.Parameters.Add(parameter);
                
                using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = [.. schema.AsEnumerable().Select(row => row["ColumnName"].ToString() ?? string.Empty)];
                }
                
                result.Rows = await ReadDataFromReader(reader);
                result.RowCount = result.Rows.Count;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                logger.LogError(ex, "Erro ao executar consulta em campo JSON");
            }
            
            return result;
        }
        
        // Novo método para consulta em arrays JSON
        public async Task<QueryResultModel> QueryJsonArrayContainsAsync(
            string tableName, 
            string jsonColumn, 
            string arrayPath,
            string value,
            DatabaseConnectionModel connection)
        {
            if (connection.Type != DatabaseType.PostgreSQL)
            {
                throw new NotSupportedException("Consulta em arrays JSON é suportada apenas para PostgreSQL");
            }
            
            var result = new QueryResultModel { Success = true };
            
            try
            {
                using var dbConnection = new NpgsqlConnection(connection.ConnectionString);
                await dbConnection.OpenAsync();
                
                using var command = dbConnection.CreateCommand();
                
                // O operador ? verifica se um valor está presente em um array JSON
                string sql;
                if (arrayPath.Contains(','))
                {
                    // Array aninhado: data#>'{info,tags}' ? 'valor'
                    sql = $"SELECT * FROM {EscapeIdentifier(tableName, DatabaseType.PostgreSQL)} " +
                          $"WHERE {EscapeIdentifier(jsonColumn, DatabaseType.PostgreSQL)}#>'{{{arrayPath}}}' ? @value";
                }
                else
                {
                    // Array direto: data->'tags' ? 'valor'
                    sql = $"SELECT * FROM {EscapeIdentifier(tableName, DatabaseType.PostgreSQL)} " +
                          $"WHERE {EscapeIdentifier(jsonColumn, DatabaseType.PostgreSQL)}->''{arrayPath}'' ? @value";
                }
                
                command.CommandText = sql;
                
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@value";
                parameter.Value = value;
                command.Parameters.Add(parameter);
                
                using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = [.. schema.AsEnumerable().Select(row => row["ColumnName"].ToString() ?? string.Empty)];
                }
                
                result.Rows = await ReadDataFromReader(reader);
                result.RowCount = result.Rows.Count;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                logger.LogError(ex, "Erro ao executar consulta em array JSON");
            }
            
            return result;
        }
        
        public async Task<QueryResultModel> ExecuteComplexJsonQueryAsync(string rawJsonQuery, DatabaseConnectionModel connection)
        {
            if (connection.Type != DatabaseType.PostgreSQL)
            {
                throw new NotSupportedException($"Consultas JSON complexas são suportadas apenas para PostgreSQL, não para {connection.Type}");
            }
            
            var result = new QueryResultModel
            {
                Success = true
            };
            
            try
            {
                using var dbConnection = new NpgsqlConnection(connection.ConnectionString);
                await dbConnection.OpenAsync();
                
                using var command = dbConnection.CreateCommand();
                command.CommandText = rawJsonQuery;
                
                logger.LogInformation("Executando consulta JSON complexa: {JsonQuery}", rawJsonQuery);
                
                using var reader = await command.ExecuteReaderAsync();
                var schema = await reader.GetSchemaTableAsync();
                if (schema != null)
                {
                    result.Columns = [.. schema.AsEnumerable().Select(row => row["ColumnName"].ToString() ?? string.Empty)];
                }
                
                result.Rows = await ReadDataFromReader(reader);
                result.RowCount = result.Rows.Count;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao executar consulta JSON complexa");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }
    }
}