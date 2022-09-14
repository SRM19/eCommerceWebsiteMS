using Foody.Services.ProductsApi.Models.DataTransferObjs;

namespace Foody.Services.ProductsApi.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductsDto>> GetProducts();
        Task<ProductsDto> GetProductById(int productid);

        Task<ProductsDto> CreateUpdateProduct(ProductsDto product);

        Task<bool> DeleteProduct(int productid);
    }
}
