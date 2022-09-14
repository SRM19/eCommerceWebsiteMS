using Foody.Web.Models;
using Foody.Web.Services.IServices;

namespace Foody.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory clientFactory;
        public ProductService(IHttpClientFactory httpClient) : base(httpClient)
        {
            clientFactory = httpClient;
        }

        public async Task<T> CreateProductAsync<T>(ProductsDto product, string token)
        {
           return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.POST,
                Data = product,
                Url = Utils.Constants.ProductAPIBase + "/api/products",
                AccessToken = token
            });
        }

        public async Task<T> DeleteProductAsync<T>(int id, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.DELETE,
                Url = Utils.Constants.ProductAPIBase + "/api/products/" + id,
                AccessToken = token
            });
        }

        public async Task<T> GetallProductsAsync<T>(string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.GET,
                Url = Utils.Constants.ProductAPIBase + "/api/products",
                AccessToken = token
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.GET,
                Url = Utils.Constants.ProductAPIBase + "/api/products/" + id,
                AccessToken = token
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductsDto product, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.PUT,
                Data = product,
                Url = Utils.Constants.ProductAPIBase + "/api/products",
                AccessToken = token
            });
        }
    }
}
