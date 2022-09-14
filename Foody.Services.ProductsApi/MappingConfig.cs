using AutoMapper;
using Foody.Services.ProductsApi.Models;
using Foody.Services.ProductsApi.Models.DataTransferObjs;

namespace Foody.Services.ProductsApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductsDto, Product>();
                config.CreateMap<Product, ProductsDto>();
            });
            return mappingConfig;
        }
    }
}
