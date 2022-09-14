using AutoMapper;
using Foody.Services.ProductsApi.DbContexts;
using Foody.Services.ProductsApi.Models;
using Foody.Services.ProductsApi.Models.DataTransferObjs;
using Microsoft.EntityFrameworkCore;

namespace Foody.Services.ProductsApi.Repository
{
    public class ProductRepository : IProductRepository
    {
        private ApplicationDbContext _db;
        private IMapper _mapper;

        public ProductRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<ProductsDto> CreateUpdateProduct(ProductsDto productdto)
        {
            Product product = _mapper.Map<Product>(productdto);
            if(product.ProductId > 0)
            {
                _db.Products.Update(product);
            }
            else
            {
                _db.Products.Add(product);
            }
            await _db.SaveChangesAsync();
            return _mapper.Map<ProductsDto>(product);
        }

        public async Task<bool> DeleteProduct(int productid)
        {
            try
            {
                Product product = await _db.Products.FindAsync(productid);
                Product productOne = await _db.Products.FirstOrDefaultAsync(x => x.ProductId == productid);
                if (product == null)
                {
                    return false;
                }
                else
                {
                    _db.Products.Remove(product);
                    await _db.SaveChangesAsync();
                }
                return true;
               
            }catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ProductsDto> GetProductById(int productid)
        {
            Product product = await _db.Products.Where(x => x.ProductId == productid).FirstOrDefaultAsync();
            return _mapper.Map<ProductsDto>(product);
        }

        public async Task<IEnumerable<ProductsDto>> GetProducts()
        {
            List<Product> products = await _db.Products.ToListAsync();
            return _mapper.Map<List<ProductsDto>>(products);

        }
    }
}
