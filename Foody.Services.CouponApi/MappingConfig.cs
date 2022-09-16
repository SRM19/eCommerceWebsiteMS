using AutoMapper;
using Foody.Services.CouponApi.Models;
using Foody.Services.CouponApi.Models.DataTransferObjs;

namespace Foody.Services.CouponApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Coupon, CouponDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
