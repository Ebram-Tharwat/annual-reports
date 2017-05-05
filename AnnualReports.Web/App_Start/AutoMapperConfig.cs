using AnnualReports.Domain.Core.AnnualReportsDbModels;
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
            });
        }
    }
}