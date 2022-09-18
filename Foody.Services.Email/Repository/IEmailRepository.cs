using Foody.Services.Email.Messages;

namespace Foody.Services.Email.Repository
{
    public interface IEmailRepository
    {
        Task SendandLogEmail(PaymentStatusDto paymentStatusDto);
    }
}
