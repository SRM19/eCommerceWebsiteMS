using Foody.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foody.Services.PaymentApi
{
    public interface IMessageBus
    {
        void PublishMessage(BaseMessage message);
    }
}
