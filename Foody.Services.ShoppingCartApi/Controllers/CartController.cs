using Foody.Services.ShoppingCartApi.Models;
using Foody.Services.ShoppingCartApi.Models.DataTransferObjs;
using Foody.Services.ShoppingCartApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Foody.Services.ShoppingCartApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICartRepository _cartRepository;
        protected ResponseDto _response;
        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            _response = new();
        }

        [HttpGet("{userId}")]
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

        [HttpPost]
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

        [HttpPost]
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

        [HttpPost]
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

        [HttpDelete("{userId}")]
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
    }
}
