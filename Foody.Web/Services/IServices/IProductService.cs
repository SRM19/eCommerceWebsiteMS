using Foody.Web.Models;

namespace Foody.Web.Services.IServices
{
    public interface IProductService 
    {
        Task<T> GetallProductsAsync<T>(string token);

        Task<T> GetProductByIdAsync<T>(int id, string token);

        Task<T> CreateProductAsync<T>(ProductsDto product, string token);

        Task<T> UpdateProductAsync<T>(ProductsDto product, string token);

        Task<T> DeleteProductAsync<T>(int id, string token);
    }
}
