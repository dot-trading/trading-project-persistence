using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;
using TradingProject.Persistence.Application.Abstractions;
using TradingProject.Persistence.Application.Common.Models;

namespace TradingProject.Persistence.Application.Queries;

public class GetOpenPositionsQuery : IQuery<List<OpenPosition>>;

public class GetOpenPositionsQueryHandler(ITradingDbContext context, IMapper mapper)
    : IQueryHandler<GetOpenPositionsQuery, List<OpenPosition>>
{
    public async Task<List<OpenPosition>> Handle(GetOpenPositionsQuery query, CancellationToken cancellationToken)
    {
        return await context.Trades
            .Where(t => t.Status == "open")
            .OrderByDescending(t => t.CreatedAt)
            .ProjectTo<OpenPosition>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}
