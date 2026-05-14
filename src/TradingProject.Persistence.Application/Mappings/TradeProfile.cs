using AutoMapper;
using TradingProject.Persistence.Application.Common;
using TradingProject.Persistence.Application.Common.Models;
using TradingProject.Persistence.Application.UseCases.Trades;
using TradingProject.Persistence.Application.UseCases.Trades.CreateTrade;
using TradingProject.Persistence.Application.UseCases.Trades.UpdateTrade;
using TradingProject.Persistence.Domain.Entities;

namespace TradingProject.Persistence.Application.Mappings;

public class TradeProfile : Profile
{
    public TradeProfile()
    {
        CreateMap<Trade, TradeResponse>();

        CreateMap<Trade, OpenPosition>()
            .ForCtorParam("Entry", opt => opt.MapFrom(src => src.Price))
            .ForCtorParam("Value", opt => opt.MapFrom(src => src.Value));
        
        CreateMap<CreateTradeRequest, Trade>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.QuoteAsset, opt => opt.MapFrom(src => SymbolHelper.ExtractQuoteAsset(src.Symbol)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "open"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CloseAt, opt => opt.Ignore())
            .ForMember(dest => dest.ClosePrice, opt => opt.Ignore())
            .ForMember(dest => dest.Pnl, opt => opt.Ignore())
            .ForMember(dest => dest.PnlPct, opt => opt.Ignore());

        CreateMap<UpdateTradeRequest, Trade>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
