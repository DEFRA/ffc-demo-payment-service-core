using FFCDemoPaymentService.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using FFCDemoPaymentService.Payments;

namespace FFCDemoPaymentService.Messaging.Actions
{
    public class PaymentAction : IMessageAction<Payment>, IMessageDeserializer<Payment>
    {
        readonly IServiceScopeFactory serviceScopeFactory;

        public PaymentAction(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void ReceiveMessage(string message)
        {
            var payment = DeserializeMessage(message);
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                paymentService.CreatePayment(payment.ClaimId, payment.Value);
            }
        }

        public Payment DeserializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<Payment>(message);
        }
    }
}
