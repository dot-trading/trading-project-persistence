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
        CancellationToken ct,
        [FromQuery] int limit = 50,
        [FromQuery] string? symbol = null,
        [FromQuery] bool? isApproved = null)
        => Ok(await mediator.SendQueryAsync(new GetOpportunitiesQuery(limit, symbol, isApproved), ct));

    [HttpPost]
    public async Task<IActionResult> CreateOpportunity(
        [FromBody] CreateOpportunityRequest request, CancellationToken ct)
    {
        var opportunity = await mediator.SendCommandAsync<CreateOpportunityCommand, OpportunityResponse>(
            new CreateOpportunityCommand(request), ct);
        return CreatedAtAction(nameof(GetOpportunities), opportunity);
    }
}
