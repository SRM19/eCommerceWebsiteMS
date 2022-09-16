using Foody.Web.Models;
using Foody.Web.Services.IServices;

namespace Foody.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        private readonly IHttpClientFactory clientFactory;

        public CouponService(IHttpClientFactory httpClient) : base(httpClient)
        {
            this.clientFactory = httpClient;
        }

        public async Task<T> GetCoupon<T>(string couponCode, string token)
        {
            return await SendRequestAsync<T>(new APIRequest
            {
                Method = Utils.Constants.RequestType.GET,
                Url = Utils.Constants.CouponAPIBase + "/api/coupon/" + couponCode,
                AccessToken = token
            });
        }
    }
}
