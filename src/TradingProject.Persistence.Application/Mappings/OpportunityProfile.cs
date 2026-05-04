using AutoMapper;
using TradingProject.Persistence.Application.UseCases.Opportunities;
using TradingProject.Persistence.Application.UseCases.Opportunities.CreateOpportunity;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.Mappings;

public class OpportunityProfile : Profile
{
    public OpportunityProfile()
    {
        CreateMap<Opportunity, OpportunityResponse>();
        CreateMap<CreateOpportunityRequest, Opportunity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Acted, opt => opt.Ignore());
    }
}
