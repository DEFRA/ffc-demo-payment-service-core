using System;
using System.Threading;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.ServiceBus.Primitives;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageService : BackgroundService
    {
        private readonly string queueEndPoint;
        private readonly string scheduleQueue;
        private readonly string paymentQueue;
        private readonly TokenProvider tokenProvider;
        private readonly IMessageAction<Schedule> scheduleAction;
        private readonly IMessageAction<Payment> paymentAction;

        private Receiver<Payment> paymentReceiver;
        private Receiver<Schedule> scheduleReceiver;
        private readonly int credits;

        public MessageService(
            MessageConfig messageConfig,
            IMessageAction<Schedule> scheduleAction,
            IMessageAction<Payment> paymentAction)
        {
            paymentQueue = messageConfig.PaymentQueueName;
            scheduleQueue = messageConfig.ScheduleQueueName;
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
            credits = int.Parse(messageConfig.MessageQueuePreFetch);
            queueEndPoint = messageConfig.MessageQueueEndPoint;
            tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            paymentReceiver = new Receiver<Payment>(tokenProvider, queueEndPoint, paymentQueue, paymentAction, credits);
            scheduleReceiver = new Receiver<Schedule>(tokenProvider, queueEndPoint, scheduleQueue, scheduleAction, credits);

            return Task.CompletedTask;
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("closing paymentReceiver...");
            await paymentReceiver.CloseAsync();
            Console.WriteLine("closing scheduleReceiver...");
            await scheduleReceiver.CloseAsync();
        }
    }
}
