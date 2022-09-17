using Foody.Services.ShoppingCartApi.DbContexts;
using Foody.Services.ShoppingCartApi.Models.DataTransferObjs;
using Newtonsoft.Json;

namespace Foody.Services.ShoppingCartApi.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private HttpClient _httpClient;
        private ResponseDto response;

        public CouponRepository(HttpClient httpClient)
        {
            this._httpClient = httpClient;
            response = new ResponseDto();
        }

        public async Task<CouponDto> GetCoupon(string code)
        {
            var apiResponse = await _httpClient.GetAsync($"/api/coupon/{code}");
            var content = await apiResponse.Content.ReadAsStringAsync();
            response = JsonConvert.DeserializeObject<ResponseDto>(content);
            if(response !=null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
            }
            return new CouponDto();
           
        }
    }
}
