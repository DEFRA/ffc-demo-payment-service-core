using System;
using System.Configuration;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Transport { get; set; }
        public string PaymentQueue { get; set; }
        public string PaymentUserName { get; set; }
        public string PaymentPassword { get; set; }
    }
}