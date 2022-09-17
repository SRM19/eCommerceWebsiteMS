using Foody.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foody.Services.PaymentApi.Messages
{
    internal class PaymentStatusDto : BaseMessage
    {
        public int OrderId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
