using Foody.Web.Models;
using Foody.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Foody.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductsDto> products = new();
            var acc_token = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetallProductsAsync<ResponseDto>(acc_token);
            if (response != null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductsDto>>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        [Authorize]
        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateProduct(ProductsDto product)
        {
            if (ModelState.IsValid)
            {
                var acc_token = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.CreateProductAsync<ResponseDto>(product, acc_token);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("ProductIndex");
                }
            }
            return View(product);
        }

        [Authorize]
        public async Task<IActionResult> EditProduct(int productId)
        {
            ProductsDto product = new();
            var acc_token = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, acc_token);
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductsDto>(Convert.ToString(response.Result));
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditProduct(ProductsDto product)
        {
            if (ModelState.IsValid)
            {
                var acc_token = await HttpContext.GetTokenAsync("access_token");
                var response = await _productService.UpdateProductAsync<ResponseDto>(product, acc_token);
                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction("ProductIndex");
                }
            }
            return View(product);
        }

        [Authorize]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            ProductsDto product = new();
            var acc_token = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, acc_token);
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductsDto>(Convert.ToString(response.Result));
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(ProductsDto product)
        {
            var acc_token = await HttpContext.GetTokenAsync("access_token");
            var response = await _productService.DeleteProductAsync<ResponseDto>(product.ProductId, acc_token);
            if (response.IsSuccess)
            {
                return RedirectToAction("ProductIndex");
            }
            return NotFound();
        }
    }
}
