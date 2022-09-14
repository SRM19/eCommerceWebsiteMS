namespace Foody.Services.ShoppingCartApi.Models.DataTransferObjs
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; }

        public IEnumerable<CartDetailsDto> CartDetails { get; set; }
    }
}
