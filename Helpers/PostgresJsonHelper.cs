using System.Text.Json;
using Npgsql;
using NpgsqlTypes;

namespace Project_Echo.Helpers
{

    public static class PostgresJsonHelper
    {
        public static async Task InsertJsonObjectAsync(
            string connectionString,
            string tableName,
            string jsonColumn,
            object jsonObject,
            Dictionary<string, object>? additionalColumns = null)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Converter to JSON
            var json = JsonSerializer.Serialize(jsonObject);

            // Construct Query
            string sql;
            if (additionalColumns is { Count: > 0 })
            {
                var columnNames = new List<string> { jsonColumn };
                var paramNames = new List<string> { $"@{jsonColumn}" };

                foreach (var col in additionalColumns.Keys)
                {
                    columnNames.Add(col);
                    paramNames.Add($"@{col}");
                }

                sql = $"INSERT INTO {tableName} ({string.Join(", ", columnNames)}) " +
                      $"VALUES ({string.Join(", ", paramNames)})";
            }
            else
            {
                sql = $"INSERT INTO {tableName} ({jsonColumn}) VALUES (@{jsonColumn})";
            }

            await using var command = new NpgsqlCommand(sql, connection);
            
            var jsonParam = command.CreateParameter();
            jsonParam.ParameterName = $"@{jsonColumn}";
            jsonParam.Value = json;
            jsonParam.NpgsqlDbType = NpgsqlDbType.Jsonb; // Use JSONB para melhor desempenho
            command.Parameters.Add(jsonParam);
            
            if (additionalColumns != null)
            {
                foreach (var col in additionalColumns)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = $"@{col.Key}";
                    param.Value = col.Value ?? DBNull.Value;
                    command.Parameters.Add(param);
                }
            }

            await command.ExecuteNonQueryAsync();
        }

        public static async Task UpdateJsonFieldAsync(
            string connectionString,
            string tableName,
            string jsonColumn,
            string jsonPath,
            object newValue,
            string whereClause)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Converter o novo valor para JSON
            string jsonValue = JsonSerializer.Serialize(newValue);

            // Construir a consulta jsonb_set
            string sql =
                $"UPDATE {tableName} SET {jsonColumn} = jsonb_set({jsonColumn}, '{{{jsonPath}}}', @newValue::jsonb) " +
                $"WHERE {whereClause}";

            await using var command = new NpgsqlCommand(sql, connection);

            var param = command.CreateParameter();
            param.ParameterName = "@newValue";
            param.Value = jsonValue;
            command.Parameters.Add(param);

            await command.ExecuteNonQueryAsync();
        }
        
        public static async Task<T?> GetJsonObjectAsync<T>(
            string connectionString,
            string tableName,
            string jsonColumn,
            string whereClause)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            var sql = $"SELECT {jsonColumn} FROM {tableName} WHERE {whereClause} LIMIT 1";
            
            await using var command = new NpgsqlCommand(sql, connection);
            
            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return default;
            }
            
            string json = result.ToString() ?? "{}";
            return JsonSerializer.Deserialize<T>(json);
        }
        
        /// <summary>
        /// Adiciona um elemento a um array JSON existente
        /// </summary>
        public static async Task AddToJsonArrayAsync(
            string connectionString,
            string tableName,
            string jsonColumn,
            string arrayPath,
            object newElement,
            string whereClause)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            
            // Converter o novo elemento para JSON
            string jsonValue = JsonSerializer.Serialize(newElement);
            
            // Construir a consulta para adicionar ao array
            string sql;
            if (string.IsNullOrEmpty(arrayPath))
            {
                // Adicionar diretamente ao array raiz
                sql = $"UPDATE {tableName} SET {jsonColumn} = {jsonColumn} || @newElement::jsonb " +
                      $"WHERE {whereClause}";
            }
            else
            {
                // Criar ou atualizar um array em um caminho específico
                sql = $"UPDATE {tableName} SET {jsonColumn} = jsonb_set({jsonColumn}, '{{{arrayPath}}}', " +
                      $"COALESCE({jsonColumn}#>'{{{arrayPath}}}', '[]'::jsonb) || @newElement::jsonb) " +
                      $"WHERE {whereClause}";
            }
            
            await using var command = new NpgsqlCommand(sql, connection);
            
            var param = command.CreateParameter();
            param.ParameterName = "@newElement";
            param.Value = jsonValue;
            command.Parameters.Add(param);
            
            await command.ExecuteNonQueryAsync();
        }
    }
}



        

