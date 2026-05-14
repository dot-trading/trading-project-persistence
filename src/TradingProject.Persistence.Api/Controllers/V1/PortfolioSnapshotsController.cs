using Asp.Versioning;
using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.CreatePortfolioSnapshot;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.GetPortfolioSnapshots;

namespace TradingProject.Persistence.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/portfolio-snapshots")]
public class PortfolioSnapshotsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPortfolioSnapshots(
        [FromQuery] int limit = 50,
        [FromQuery] int page = 1,
        CancellationToken cancellationToken = default)
        => Ok(await mediator.SendQueryAsync(new GetPortfolioSnapshotsQuery(limit, page), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> CreatePortfolioSnapshot(
        [FromBody] CreatePortfolioSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await mediator.SendCommandAsync<CreatePortfolioSnapshotCommand, PortfolioSnapshotResponse>(
            new CreatePortfolioSnapshotCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetPortfolioSnapshots), snapshot);
    }
}
