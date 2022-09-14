using Foody.Web.Models;
using Foody.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Foody.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public CartController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartforUserbyId());
        }

        public async Task<CartDto> LoadCartforUserbyId()
        {
            //get user id
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            //get accesstoken
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            //get cart from cart service
            CartDto cart = new();
            var response = await _cartService.GetCartbyUserIdAsync<ResponseDto>(userId, accessToken);
            if (response != null && response.IsSuccess == true)
            {
                cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }
            if (cart.CartHeader != null)
            {
                foreach (var cartDetail in cart.CartDetails)
                {
                    cart.CartHeader.OrderTotal += (cartDetail.Count * cartDetail.Product.Price);
                }
            }
            return cart;
        }

        public async Task<IActionResult> RemoveItem(int cartDetailId)
        {
            //get accesstoken
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            //remove item using cart service
            var response = await _cartService.RemovefromCartAsync<ResponseDto>(cartDetailId, accessToken);
            if (response != null && response.IsSuccess == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return RedirectToAction(nameof(Error));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
