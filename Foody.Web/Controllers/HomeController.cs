using Foody.Web.Models;
using Foody.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Foody.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductsDto> products = new List<ProductsDto>();
            var response = await _productService.GetallProductsAsync<ResponseDto>("");
            if (response != null && response.IsSuccess)
            {
                products = JsonConvert.DeserializeObject<List<ProductsDto>>(Convert.ToString(response.Result));
            }
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var acc_token = await HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductsDto product = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");
            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductsDto>(Convert.ToString(response.Result));
                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductsDto product)
        {
            CartDto cart = new CartDto()
            {
                CartHeader = new()
                {
                    UserId = User.Claims.Where(u => u.Type == "sub").FirstOrDefault()?.Value
                }
            };
            CartDetailsDto cartProduct = new()
            {
                ProductId = product.ProductId,
                Count = product.Count
            };
            //also populate the product in cart details in 2 ways
            //one way is to use hidden attribute and build the object
            //call product api service and get the product

            //response from product api service need to deserialized and added to cart details product field
            var response = await _productService.GetProductByIdAsync<ResponseDto>(product.ProductId, "");
            if (response != null && response.IsSuccess == true)
            {
                cartProduct.Product = JsonConvert.DeserializeObject<ProductsDto>(Convert.ToString(response.Result));
            }
            List<CartDetailsDto> cartDetails = new() { cartProduct };
            cart.CartDetails = cartDetails;

            var acc_token = await HttpContext.GetTokenAsync("access_token");
            var addtoCart = await _cartService.AddtoCartAsync<ResponseDto>(cart, acc_token);
            if (addtoCart != null && addtoCart.IsSuccess == true)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}