using AutoMapper;
using Foody.Services.OrderApi.Messages;
using Foody.Services.OrderApi.Models;
using Foody.Services.OrderApi.Models.DataTransferObjs;

namespace Foody.Services.OrderApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Order, OrderDto>().ReverseMap();
                config.CreateMap<OrderHeaderDto, OrderDto>();
                config.CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
