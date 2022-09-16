using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Foody.Services.OrderApi.Messages;

namespace Foody.Services.OrderApi.Models.DataTransferObjs
{
    [AutoMap(typeof(OrderHeaderDto))]
    public class OrderDto
    {
        [Ignore]
        public int OrderHeaderId { get; set; }
        public string UserId { get; set; }
        public string CouponCode { get; set; }
        public double OrderTotal { get; set; }
        public double DiscountTotal { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime PickupDateTime { get; set; }

        [Ignore]
        public DateTime OrderTime { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public int TotalItems { get; set; }

        [Ignore]
        public List<OrderDetailDto> OrderDetails { get; set; }
        public bool PaymentStatus { get; set; }
    }
}
