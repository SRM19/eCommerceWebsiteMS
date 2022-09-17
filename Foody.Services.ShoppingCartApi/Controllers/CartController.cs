using Foody.MessageBus;
using Foody.Services.ShoppingCartApi.Messages;
using Foody.Services.ShoppingCartApi.Models;
using Foody.Services.ShoppingCartApi.Models.DataTransferObjs;
using Foody.Services.ShoppingCartApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Foody.Services.ShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICartRepository _cartRepository;
        private ICouponRepository _couponRepository;
        private IMessageBus _messageBus;
        protected ResponseDto _response;
        public CartController(ICartRepository cartRepository, ICouponRepository couponRepository, IMessageBus messageBus)
        {
            _cartRepository = cartRepository;
            _couponRepository = couponRepository;
            _messageBus = messageBus;
            _response = new();
        }

        [Authorize]
        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cart = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cart;
            }catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                CartDto cart = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                CartDto cart = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody]int cartId)
        {
            try
            {
                bool res = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = res;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpDelete("ClearCart/{userId}")]
        public async Task<object> ClearCart(string userId)
        {
            try
            {
                bool res = await _cartRepository.ClearCart(userId);
                _response.Result = res;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon(CartDto cartDto)
        {
            try
            {
                bool res = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
                _response.Result = res;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody]string userId)
        {
            try
            {
                bool res = await _cartRepository.RemoveCoupon(userId);
                _response.Result = res;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }

        [Authorize]
        [HttpPost("Checkout")]
        public async Task<object> Checkout(OrderHeaderDto orderHeaderDto)
        {
            try
            {
                //getusercart
                CartDto cartDto = await _cartRepository.GetCartByUserId(orderHeaderDto.UserId);
                if (cartDto == null)
                {
                    return BadRequest();
                }
                orderHeaderDto.CartDetails = cartDto.CartDetails;

                if (!string.IsNullOrEmpty(orderHeaderDto.CouponCode))
                {
                    //verify coupon is valid before place the order
                    var coupon = await _couponRepository.GetCoupon(orderHeaderDto.CouponCode);
                    if(orderHeaderDto.DiscountTotal != coupon.DiscountAmount)
                    {
                        _response.IsSuccess = false;
                        _response.ErrorMessage = new List<string>() { "Coupon amount has changed. Please confirm" };
                        _response.DisplayMessage = "Coupon amount has changed. Please confirm";
                        return _response;
                    }
                }
               
                //logic to add message to process order
                _messageBus.SendMessage(orderHeaderDto, "checkoutqueue");

                await _cartRepository.ClearCart(orderHeaderDto.UserId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _response;
        }
    }
}
