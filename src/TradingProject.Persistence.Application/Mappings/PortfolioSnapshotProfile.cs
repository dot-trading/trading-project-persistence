using AutoMapper;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots;
using TradingProject.Persistence.Application.UseCases.PortfolioSnapshots.CreatePortfolioSnapshot;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.Mappings;

public class PortfolioSnapshotProfile : Profile
{
    public PortfolioSnapshotProfile()
    {
        CreateMap<PortfolioSnapshot, PortfolioSnapshotResponse>();
        CreateMap<CreatePortfolioSnapshotRequest, PortfolioSnapshot>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
