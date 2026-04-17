using Microsoft.AspNetCore.Mvc;
using TradingProject.Persistence.Application.Abstractions;

namespace TradingProject.Persistence.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradingDataController(IDatabaseService databaseService) : ControllerBase
{
    [HttpGet("positions/open/count")]
    public IActionResult GetOpenPositionsCount()
    {
        return Ok(databaseService.GetOpenPositionsCount());
    }

    [HttpGet("pnl/daily")]
    public IActionResult GetDailyPnl()
    {
        return Ok(databaseService.GetDailyPnl());
    }

    [HttpGet("pnl/total")]
    public IActionResult GetTotalPnl()
    {
        return Ok(databaseService.GetTotalPnl());
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(databaseService.GetStats());
    }

    [HttpGet("pnl/summary")]
    public IActionResult GetPnlSummary()
    {
        return Ok(databaseService.GetPnlSummary());
    }

    [HttpGet("positions/open")]
    public IActionResult GetOpenPositions()
    {
        return Ok(databaseService.GetOpenPositions());
    }

    [HttpGet("trades/last")]
    public IActionResult GetLastTrades([FromQuery] int limit = 5)
    {
        return Ok(databaseService.GetLastTrades(limit));
    }
}
