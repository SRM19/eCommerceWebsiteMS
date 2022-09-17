using Foody.Services.ShoppingCartApi.Models.DataTransferObjs;

namespace Foody.Services.ShoppingCartApi.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string code);
    }
}
