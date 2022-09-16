using Foody.Services.CouponApi.Models.DataTransferObjs;

namespace Foody.Services.CouponApi.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponbyCode(string code);
    }
}
