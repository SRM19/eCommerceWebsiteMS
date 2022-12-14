using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foody.Services.PaymentApi.Messages
{
    internal class PaymentRequestDto
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
