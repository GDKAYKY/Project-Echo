using Microsoft.AspNetCore.Mvc;
using Project_Echo.Services;

namespace Project_Echo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<DatabaseController> _logger;
    private readonly IDatabaseQueryService _databaseQueryService;
    private readonly IDatabaseTypeDetector _databaseTypeDetector;

    public DatabaseController(IDatabaseService databaseService, ILogger<DatabaseController> logger, IDatabaseQueryService databaseQueryService, IDatabaseTypeDetector databaseTypeDetector)
    {
        _databaseService = databaseService;
        _logger = logger;
        _databaseQueryService = databaseQueryService;
        _databaseTypeDetector = databaseTypeDetector;
    }

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
            var result = await _databaseQueryService.ExecuteRawQueryAsync(request.Query, new Models.DatabaseConnectionModel { ConnectionString = connection.Host, Type = connection.Type });
            var executionTime = (DateTime.UtcNow - startTime).TotalSeconds;

            return Ok(new
            {
                success = true,
                data = new
                {
                    columns = result.Columns.ToArray(),
                    rows = result.Rows.ToArray(),
                    rowCount = result.RowCount,
                    executionTime
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            return BadRequest(new { success = false, error = new { message = ex.Message } });
        }
    }

    [HttpGet("tables/{connectionId}")]
    public async Task<IActionResult> GetTables(string connectionId)
    {
        try
        {
            var connection = await _databaseService.GetConnectionAsync(connectionId);
            if (connection == null)
            {
                return NotFound(new { success = false, error = new { message = "Database connection not found" } });
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