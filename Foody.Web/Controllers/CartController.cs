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
        private readonly ICouponService _couponService;

        public CartController(IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    var getCoupon = await _couponService.GetCoupon<ResponseDto>(cart.CartHeader.CouponCode,accessToken);
                    if(getCoupon!=null && getCoupon.IsSuccess == true)
                    {
                        var couponDetail = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(getCoupon.Result));
                        cart.CartHeader.DiscountTotal = couponDetail.DiscountAmount;
                    }
                }
                foreach (var cartDetail in cart.CartDetails)
                {
                    cart.CartHeader.OrderTotal += (cartDetail.Count * cartDetail.Product.Price);
                }
                if (cart.CartHeader.OrderTotal > cart.CartHeader.DiscountTotal)
                {
                    cart.CartHeader.OrderTotal -= cart.CartHeader.DiscountTotal;
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

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            //get accesstoken
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            //update UI to show coupon applied
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken);

            if (response != null && response.IsSuccess == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();


        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            //get accesstoken
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            //remove coupon to show in UI
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId,accessToken);

            if (response != null && response.IsSuccess == true)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();

        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartforUserbyId());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                //get accesstoken
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cartDto.CartHeader,accessToken);
                return RedirectToAction(nameof(Confirmation));
            }
            catch(Exception ex)
            {
                return View(cartDto);
            }
        }

        public async Task<IActionResult> Confirmation()
        {
            return View();
        }
    }
}
