using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.CreatePortfolioSnapshot;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.GetPortfolioSnapshots;

namespace TradingProject.Persistence.Api.Controllers;

[ApiController]
[Route("api/portfolio-snapshots")]
public class PortfolioSnapshotsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPortfolioSnapshots(
        CancellationToken ct,
        [FromQuery] int limit = 50)
        => Ok(await mediator.SendQueryAsync(new GetPortfolioSnapshotsQuery(limit), ct));

    [HttpPost]
    public async Task<IActionResult> CreatePortfolioSnapshot(
        [FromBody] CreatePortfolioSnapshotRequest request, CancellationToken ct)
    {
        var snapshot = await mediator.SendCommandAsync<CreatePortfolioSnapshotCommand, PortfolioSnapshotResponse>(
            new CreatePortfolioSnapshotCommand(request), ct);
        return CreatedAtAction(nameof(GetPortfolioSnapshots), snapshot);
    }
}
