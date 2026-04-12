using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class GiftCardEmailRequest
    {
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string SenderName { get; set; }
        public string Greeting { get; set; }
        public decimal Amount { get; set; }
    }
}
