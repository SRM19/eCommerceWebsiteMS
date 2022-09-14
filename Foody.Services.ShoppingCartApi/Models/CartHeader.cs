using System.ComponentModel.DataAnnotations;

namespace Foody.Services.ShoppingCartApi.Models
{
    public class CartHeader
    {
        //Maintain relation between User and Cart with CartID and UserID
        [Key]
        public int CartHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
    }
}
