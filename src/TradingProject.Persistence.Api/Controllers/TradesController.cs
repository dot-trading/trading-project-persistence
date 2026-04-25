using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TradingProject.Persistence.Application.UseCases.Trades;
using TradingProject.Persistence.Application.UseCases.Trades.CreateTrade;
using TradingProject.Persistence.Application.UseCases.Trades.GetTrades;
using TradingProject.Persistence.Application.UseCases.Trades.UpdateTrade;

namespace TradingProject.Persistence.Api.Controllers;

[ApiController]
[Route("api/trades")]
public class TradesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrades(
        CancellationToken ct,
        [FromQuery] int limit = 50,
        [FromQuery] string? status = null,
        [FromQuery] string? symbol = null)
        => Ok(await mediator.SendQueryAsync(new GetTradesQuery(limit, status, symbol), ct));

    [HttpPost]
    public async Task<IActionResult> CreateTrade([FromBody] CreateTradeRequest request, CancellationToken ct)
    {
        var trade = await mediator.SendCommandAsync<CreateTradeCommand, TradeResponse>(
            new CreateTradeCommand(request), ct);
        return CreatedAtAction(nameof(GetTrades), trade);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTrade(int id, [FromBody] UpdateTradeRequest request, CancellationToken ct)
    {
        var trade = await mediator.SendCommandAsync<UpdateTradeCommand, TradeResponse?>(
            new UpdateTradeCommand(id, request), ct);
        return trade is null ? NotFound() : Ok(trade);
    }
}
