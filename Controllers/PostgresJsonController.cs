using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Project_Echo.Helpers;
using Project_Echo.Models;
using Project_Echo.Services;

namespace Project_Echo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostgresJsonController(
    IDatabaseQueryService queryService,
    IDatabaseService databaseService,
    ILogger<PostgresJsonController> logger)
    : ControllerBase
{
    [HttpPost("query")]
    public async Task<IActionResult> QueryJson([FromBody] JsonQueryRequest request)
    {
        if (string.IsNullOrEmpty(request.ConnectionId))
        {
            return BadRequest("ID da conexão é obrigatório");
        }

        var connection = await databaseService.GetConnectionAsync(request.ConnectionId);
        if (connection == null)
        {
            return NotFound("Conexão não encontrada");
        }

        if (connection.Type != DatabaseType.PostgreSQL)
        {
            return BadRequest("Esta operação é suportada apenas para PostgreSQL");
        }

        var connectionModel = new DatabaseConnectionModel
        {
            Type = connection.Type,
            ConnectionString = connection.Host
        };

        try
        {
            var result = await queryService.QueryJsonFieldAsync(
                request.TableName,
                request.JsonColumn,
                request.JsonPath,
                request.IsNestedPath,
                request.JsonOperator,
                request.JsonValue,
                connectionModel);

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar consulta JSON");
            return StatusCode(500, $"Erro ao executar consulta: {ex.Message}");
        }
    }

    [HttpPost("array")]
    public async Task<IActionResult> QueryJsonArray([FromBody] JsonArrayQueryRequest request)
    {
        if (string.IsNullOrEmpty(request.ConnectionId))
        {
            return BadRequest("ID da conexão é obrigatório");
        }

        var connection = await databaseService.GetConnectionAsync(request.ConnectionId);
        if (connection == null)
        {
            return NotFound("Conexão não encontrada");
        }

        if (connection.Type != DatabaseType.PostgreSQL)
        {
            return BadRequest("Esta operação é suportada apenas para PostgreSQL");
        }

        var connectionModel = new DatabaseConnectionModel
        {
            Type = connection.Type,
            ConnectionString = connection.Host
        };

        try
        {
            var result = await queryService.QueryJsonArrayContainsAsync(
                request.TableName,
                request.JsonColumn,
                request.ArrayPath,
                request.Value,
                connectionModel);

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar consulta em array JSON");
            return StatusCode(500, $"Erro ao executar consulta: {ex.Message}");
        }
    }

    [HttpPost("complex")]
    public async Task<IActionResult> ExecuteComplexJsonQuery([FromBody] ComplexJsonQueryRequest request)
    {
        if (string.IsNullOrEmpty(request.ConnectionId))
        {
            return BadRequest("ID da conexão é obrigatório");
        }

        if (string.IsNullOrEmpty(request.Query))
        {
            return BadRequest("A consulta SQL é obrigatória");
        }

        var connection = await databaseService.GetConnectionAsync(request.ConnectionId);
        if (connection == null)
        {
            return NotFound("Conexão não encontrada");
        }

        if (connection.Type != DatabaseType.PostgreSQL)
        {
            return BadRequest("Esta operação é suportada apenas para PostgreSQL");
        }

        var connectionModel = new DatabaseConnectionModel
        {
            Type = connection.Type,
            ConnectionString = connection.Host
        };

        try
        {
            var result = await queryService.ExecuteComplexJsonQueryAsync(request.Query, connectionModel);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar consulta JSON complexa");
            return StatusCode(500, $"Erro ao executar consulta: {ex.Message}");
        }
    }

    [HttpPost("insert")]
    public async Task<IActionResult> InsertJsonObject([FromBody] InsertJsonRequest request)
    {
        if (string.IsNullOrEmpty(request.ConnectionId))
        {
            return BadRequest("ID da conexão é obrigatório");
        }

        var connection = await databaseService.GetConnectionAsync(request.ConnectionId);
        if (connection == null)
        {
            return NotFound("Conexão não encontrada");
        }

        if (connection.Type != DatabaseType.PostgreSQL)
        {
            return BadRequest("Esta operação é suportada apenas para PostgreSQL");
        }

        try
        {
            // Deserializar o objeto JSON da string
            var jsonObject = JsonDocument.Parse(request.JsonData).RootElement;
                
            // Converter colunas adicionais se houver
            Dictionary<string, object>? additionalColumns = null;
            if (request.AdditionalColumns != null && request.AdditionalColumns.Count > 0)
            {
                additionalColumns = request.AdditionalColumns;
            }

            await PostgresJsonHelper.InsertJsonObjectAsync(
                connection.Host,
                request.TableName,
                request.JsonColumn,
                jsonObject,
                additionalColumns);

            return Ok(new { Success = true, Message = "Objeto JSON inserido com sucesso" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao inserir objeto JSON");
            return StatusCode(500, $"Erro ao inserir objeto: {ex.Message}");
        }
    }
}

public class JsonQueryRequest
{
    public string ConnectionId { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string JsonColumn { get; set; } = string.Empty;
    public string JsonPath { get; set; } = string.Empty;
    public bool IsNestedPath { get; set; } = false;
    public string JsonOperator { get; set; } = "=";
    public string JsonValue { get; set; } = string.Empty;
}

public class JsonArrayQueryRequest
{
    public string ConnectionId { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string JsonColumn { get; set; } = string.Empty;
    public string ArrayPath { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class ComplexJsonQueryRequest
{
    public string ConnectionId { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
}

public class InsertJsonRequest
{
    public string ConnectionId { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string JsonColumn { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
    public Dictionary<string, object>? AdditionalColumns { get; set; }
}