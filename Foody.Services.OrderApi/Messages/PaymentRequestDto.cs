using Foody.MessageBus;

namespace Foody.Services.OrderApi.Messages
{
    //This will be pushed to message bus, hence inherit BaseMessage class
    public class PaymentRequestDto : BaseMessage
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public string CVV { get; set; }
        public string ExpiryMonthYear { get; set; }
        public double OrderTotal { get; set; }
        public string UserEmail { get; set; }

    }
}
