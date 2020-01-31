using System;
using FFCDemoPaymentService.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace FFCDemoPaymentService.Messaging.Actions
{
    public class PaymentAction : IMessageAction<Payment>, IMessageDeserializer<Payment>
    {
        IServiceScopeFactory serviceScopeFactory;

        public PaymentAction(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void ReceiveMessage(string message)
        {
            var payment = DeserializeMessage(message);

            // TODO persist payment in database, need to pull db context from service scope factory
        }

        public Payment DeserializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<Payment>(message);
        }
    }
}
