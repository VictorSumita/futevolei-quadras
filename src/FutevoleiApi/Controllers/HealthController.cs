using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FutevoleiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IMongoDatabase _database;

    public HealthController(IMongoDatabase database)
    {
        _database = database;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            await _database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1));

            return Ok(new
            {
                mongo = "ok",
                database = _database.DatabaseNamespace.DatabaseName,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                mongo = "unreachable",
                error = ex.Message
            });
        }
    }
}