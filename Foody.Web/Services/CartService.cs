using Foody.Web.Models;
using Foody.Web.Services.IServices;

namespace Foody.Web.Services
{
    public class CartService : BaseService, ICartService
    {
        private readonly IHttpClientFactory clientFactory;

        public CartService(IHttpClientFactory httpClient) : base(httpClient)
        {
            this.clientFactory = httpClient;
        }

        public async Task<T> AddtoCartAsync<T>(CartDto cart, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.POST,
                Data = cart,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/AddCart",
                AccessToken = token
            });
        }

        public async Task<T> GetCartbyUserIdAsync<T>(string userId, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.GET,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId,
                AccessToken = token
            });
        }

        public async Task<T> RemovefromCartAsync<T>(int cartDetailId, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.POST,
                Data = cartDetailId,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/RemoveCart",
                AccessToken = token
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDto cart, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.POST,
                Data = cart,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/UpdateCart",
                AccessToken = token
            });
        }

        public async Task<T> ClearCartAsync<T>(string userId, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.DELETE,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/ClearCart/" + userId,
                AccessToken = token
            });
        }

        public async Task<T> ApplyCoupon<T>(CartDto cart, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.POST,
                Data = cart,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
                AccessToken = token
            });
        }

        public async Task<T> RemoveCoupon<T>(string userId, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest()
            {
                Method = Utils.Constants.RequestType.POST,
                Data = userId,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/RemoveCoupon",
                AccessToken = token
            });
        }

        public async Task<T> Checkout<T>(CartHeaderDto cartHeaderDto, string token)
        {
            return await this.SendRequestAsync<T>(new APIRequest
            {
                Method = Utils.Constants.RequestType.POST,
                Data = cartHeaderDto,
                Url = Utils.Constants.ShoppingCartAPIBase + "/api/cart/Checkout",
                AccessToken = token

            });
        }
    }
}
