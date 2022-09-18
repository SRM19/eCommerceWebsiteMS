using Foody.MessageBus;

namespace Foody.Services.Email.Messages
{
    public class PaymentStatusDto 
    {
        public int OrderId { get; set; }
        public bool IsConfirmed { get; set; }
        public string UserEmail { get; set; }
    }
}
