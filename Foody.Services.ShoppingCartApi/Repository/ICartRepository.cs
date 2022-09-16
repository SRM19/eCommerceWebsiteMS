using Foody.Services.ShoppingCartApi.Models.DataTransferObjs;

namespace Foody.Services.ShoppingCartApi.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string userId);

        Task<CartDto> CreateUpdateCart(CartDto cartDto);

        Task<bool> RemoveFromCart(int productId);

        Task<bool> ClearCart(string userId);

        Task<bool> ApplyCoupon(string userId, string code);

        Task<bool> RemoveCoupon(string userId);
    }
}
