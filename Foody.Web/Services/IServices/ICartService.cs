using Foody.Web.Models;

namespace Foody.Web.Services.IServices
{
    public interface ICartService
    {
        Task<T> GetCartbyUserIdAsync<T>(string userId,string token);
        Task<T> AddtoCartAsync<T>(CartDto cart,string token);
        Task<T> UpdateCartAsync<T>(CartDto cart, string token);
        Task<T> RemovefromCartAsync<T>(int cartDetailId, string token);

        Task<T> ClearCartAsync<T>(string userId, string token);

        Task<T> ApplyCoupon<T>(CartDto cart, string token);

        Task<T> RemoveCoupon<T>(string userId, string token);

        Task<T> Checkout<T>(CartHeaderDto cart, string token);
    }
}
