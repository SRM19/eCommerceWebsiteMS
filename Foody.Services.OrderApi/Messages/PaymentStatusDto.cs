using Foody.MessageBus;

namespace Foody.Services.OrderApi.Messages
{
    public class PaymentStatusDto 
    {
        public int OrderId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
