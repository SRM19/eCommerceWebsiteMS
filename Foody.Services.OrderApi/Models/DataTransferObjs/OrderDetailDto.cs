
namespace Foody.Services.OrderApi.Models.DataTransferObjs
{
    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int OrderHeaderId { get; set; }
        public virtual Order OrderHeader { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
}
