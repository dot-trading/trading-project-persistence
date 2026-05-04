using AutoMapper;
using TradingProject.Persistence.Application.UseCases.Opportunities;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.Mappings;

public class OpportunityProfile : Profile
{
    public OpportunityProfile()
    {
        CreateMap<Opportunity, OpportunityResponse>();
    }
}
