using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.ViewModels.BarModels;
using AnnualReports.Web.ViewModels.FundModels;
using AutoMapper;

namespace GRis.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                #region Fund

                cfg.CreateMap<Fund, FundDetailsViewModel>()
                .ForMember(dest => dest.MapTo, opt => opt.MapFrom(src => src.MapToFundId == null ? src.FundNumber : src.MapToFund.FundNumber))
                ;

                #endregion Fund
                cfg.CreateMap<Bar, BarDetailsViewModel>()
              .ForMember(dest => dest.MapToBarId, opt => opt.MapFrom(src => src.MapToBarId == null ? src.BarNumber : src.MapToBar.BarNumber));
                cfg.CreateMap<BarDetailsViewModel, Bar>()
             .ForMember(dest => dest.MapToBarId, opt => opt.MapFrom(src =>  src.MapToBarId));
                #region Bar
                #endregion
            });
        }
    }
}