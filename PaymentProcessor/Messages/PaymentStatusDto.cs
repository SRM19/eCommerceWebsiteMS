using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentProcessor.Messages
{
    internal class PaymentStatusDto
    {
        public int OrderId { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
