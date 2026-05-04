using Cortex.Mediator;
using Microsoft.AspNetCore.Mvc;
using TradingProject.Persistence.Application.UseCases.Opportunities;
using TradingProject.Persistence.Application.UseCases.Opportunities.CreateOpportunity;
using TradingProject.Persistence.Application.UseCases.Opportunities.GetOpportunities;

namespace TradingProject.Persistence.Api.Controllers;

[ApiController]
[Route("api/opportunities")]
public class OpportunitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOpportunities(
        [FromQuery] int limit = 50,
        [FromQuery] int page = 1,
        [FromQuery] string? symbol = null,
        [FromQuery] bool? isApproved = null,
        CancellationToken cancellationToken = default)
        => Ok(await mediator.SendQueryAsync(
            new GetOpportunitiesQuery(limit, page, symbol, isApproved),
            cancellationToken));

    [HttpPost]
    public async Task<IActionResult> CreateOpportunity(
        [FromBody] CreateOpportunityRequest request,
        CancellationToken cancellationToken = default)
    {
        var opportunity = await mediator.SendCommandAsync<CreateOpportunityCommand, OpportunityResponse>(
            new CreateOpportunityCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetOpportunities), opportunity);
    }
}
