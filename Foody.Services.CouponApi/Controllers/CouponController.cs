using Foody.Services.CouponApi.Models.DataTransferObjs;
using Foody.Services.CouponApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Foody.Services.CouponApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        protected ResponseDto _response;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
            _response = new();
        }

        [HttpGet("{code}")]
        public async Task<object> GetCoupon(string code)
        {
            try
            {
                CouponDto coupon = await _couponRepository.GetCouponbyCode(code);
                if(coupon == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = coupon;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessage = new List<string> { ex.Message };
            }
            return _response;
        }
    }
}
