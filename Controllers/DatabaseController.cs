using Microsoft.AspNetCore.Mvc;
using Project_Echo.Services;
using System.Text.RegularExpressions;
using Project_Echo.Models;

namespace Project_Echo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController(IDatabaseService databaseService, ILogger<DatabaseController> logger, IDatabaseQueryService databaseQueryService, IDatabaseTypeDetector databaseTypeDetector, IQueryAnalyzer queryAnalyzer) : ControllerBase
{
    private readonly IDatabaseService _databaseService = databaseService;
    private readonly ILogger<DatabaseController> _logger = logger;
    private readonly IDatabaseQueryService _databaseQueryService = databaseQueryService;
    private readonly IDatabaseTypeDetector _databaseTypeDetector = databaseTypeDetector;
    private readonly IQueryAnalyzer _queryAnalyzer = queryAnalyzer;

    [HttpPost("query")]
    public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequest request)
    {
        try
        {
            var connection = await _databaseService.GetConnectionAsync(request.ConnectionId);
            if (connection == null)
            {
                return BadRequest(new { success = false, error = new { message = "Database connection not found" } });
            }

            var startTime = DateTime.UtcNow;
            var connectionModel = new Models.DatabaseConnectionModel { ConnectionString = connection.Host, Type = connection.Type };

            // Detect CPF: formats like 000.000.000-00 or 11 digits
            var cpfPattern = new Regex(@"^(\d{3}\.\d{3}\.\d{3}-\d{2}|\d{11})$");
            QueryResultModel result;
            bool isCpfSearch = false;

            string? matchedTable = null;
            if (cpfPattern.IsMatch(request.Query.Trim()))
            {
                isCpfSearch = true;
                // Try to find rows in any table that has an exact CPF column (multiple common names)
                var tables = await _databaseTypeDetector.GetTablesAsync(connection.Host, connection.Type);
                QueryResultModel? firstMatch = null;
                var candidateColumns = new[] { "cpf", "CPF", "Cpf", "CPF/CNPJ", "CPF_CNPJ" };
                foreach (var table in tables)
                {
                    try
                    {
                        foreach (var col in candidateColumns)
                        {
                            var where = connection.Type == Models.DatabaseType.SQLite
                                ? $"{col} = '{request.Query.Trim()}' COLLATE NOCASE"
                                : $"{col} = '{request.Query.Trim()}'";
                            var qm = new Models.QueryModel
                            {
                                ConnectionId = request.ConnectionId,
                                TableName = table,
                                Page = 1,
                                PageSize = 1000,
                                WhereClause = where
                            };
                            var queryRes = await _databaseQueryService.ExecuteQueryAsync(qm, connectionModel);
                            if (queryRes.Rows.Count > 0)
                            {
                                firstMatch = queryRes;
                                matchedTable = table;
                                break;
                            }
                        }
                        if (firstMatch != null) break;

                        // If not found, try normalized comparison (strip dots/dashes/spaces)
                        var digitsOnly = Regex.Replace(request.Query, "[^0-9]", "");
                        if (!string.IsNullOrEmpty(digitsOnly))
                        {
                            foreach (var col in candidateColumns)
                            {
                                string escapedTable = EscapeIdentifierLocal(table, connection.Type);
                                string escapedCol = EscapeIdentifierLocal(col, connection.Type);
                                string normalizedExpr = $"REPLACE(REPLACE(REPLACE({escapedCol},'.',''),'-',''),' ','')";
                                string sql = $"SELECT * FROM {escapedTable} WHERE {normalizedExpr} = '{digitsOnly}'";
                                var normRes = await _databaseQueryService.ExecuteRawQueryAsync(sql, connectionModel);
                                if (normRes.Rows.Count > 0)
                                {
                                    firstMatch = normRes;
                                    matchedTable = table;
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignore tables without 'cpf' column or other errors
                    }
                }

                result = firstMatch ?? new QueryResultModel { Success = true, Columns = new List<string>(), Rows = new List<object[]>(), RowCount = 0 };
                result.ExecutionTime = (DateTime.UtcNow - startTime).TotalSeconds;
            }
            else
            {
                // Default: execute the provided raw query
                result = await _databaseQueryService.ExecuteRawQueryAsync(request.Query, connectionModel);
            }
            var executionTime = (DateTime.UtcNow - startTime).TotalSeconds;

            // Only run analyzer for non-CPF free text queries
            string[]? analysisColumns = null;
            object[][]? analysisRows = null;
            if (!isCpfSearch)
            {
                var analysisDict = _queryAnalyzer.AnalyzeQuery(request.Query);
                analysisColumns = new[] { "Key", "Match" };
                analysisRows = analysisDict
                    .Select(kvp => new object[] { kvp.Key, kvp.Value })
                    .ToArray();
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    columns = result.Columns.ToArray(),
                    rows = result.Rows.ToArray(),
                    rowCount = result.RowCount,
                    executionTime,
                    matchedTable,
                    analysis = analysisColumns == null ? null : new
                    {
                        columns = analysisColumns,
                        rows = analysisRows,
                        rowCount = analysisRows!.Length
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            return BadRequest(new { success = false, error = new { message = ex.Message } });
        }
    }

    private static string EscapeIdentifierLocal(string identifier, Models.DatabaseType type)
    {
        return type switch
        {
            Models.DatabaseType.SQLite => $"[{identifier}]",
            Models.DatabaseType.MySQL => $"`{identifier}`",
            Models.DatabaseType.SQLServer => $"[{identifier}]",
            Models.DatabaseType.PostgreSQL => $"\"{identifier}\"",
            _ => identifier
        };
    }

    [HttpGet("tables/{connectionId}")]
    public async Task<IActionResult> GetTables(string connectionId)
    {
        try
        {
            var connection = await _databaseService.GetConnectionAsync(connectionId);
            if (connection == null)
            {
                _logger.LogInformation("Database connection with ID {ConnectionId} not found", connectionId);
                return NotFound(new { success = false, error = new { message = "Database connection not found" }});
            }

            var tables = await _databaseTypeDetector.GetTablesAsync(connection.Host, connection.Type);
            return Ok(new { success = true, data = new { tables = tables } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tables for connection {ConnectionId}", connectionId);
            return BadRequest(new { success = false, error = new { message = ex.Message } });
        }
    }

    public class QueryRequest
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
    }


}