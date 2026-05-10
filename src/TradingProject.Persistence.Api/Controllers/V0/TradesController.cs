using Asp.Versioning;
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TradingProject.Persistence.Application.UseCases.Trades;
using TradingProject.Persistence.Application.UseCases.Trades.CreateTrade;
using TradingProject.Persistence.Application.UseCases.Trades.GetTrades;
using TradingProject.Persistence.Application.UseCases.Trades.UpdateTrade;

namespace TradingProject.Persistence.Api.Controllers.V0;

[ApiController]
[ApiVersion("0.0")]
[Route("api/v{version:apiVersion}/trades")]
[Route("api/trades")]
public class TradesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrades(
        [FromQuery] int limit = 50,
        [FromQuery] int page = 1,
        [FromQuery] string? status = null,
        [FromQuery] string? symbol = null,
        CancellationToken cancellationToken = default)
        => Ok(await mediator.SendQueryAsync(
            new GetTradesQuery(limit, page, status, symbol),
            cancellationToken));

    [HttpPost]
    public async Task<IActionResult> CreateTrade(
        [FromBody] CreateTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        var trade = await mediator.SendCommandAsync<CreateTradeCommand, TradeResponse>(
            new CreateTradeCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetTrades), trade);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTrade(
        int id,
        [FromBody] UpdateTradeRequest request,
        CancellationToken cancellationToken = default)
    {
        var trade = await mediator.SendCommandAsync<UpdateTradeCommand, TradeResponse?>(
            new UpdateTradeCommand(id, request), cancellationToken);
        return trade is null ? NotFound() : Ok(trade);
    }
}
