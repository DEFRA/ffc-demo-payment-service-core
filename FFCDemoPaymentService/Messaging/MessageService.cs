using System;
using System.Threading;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using Microsoft.Extensions.Hosting;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageService : BackgroundService
    {
        private readonly string connectionString;
        private readonly string endPoint;
        private readonly string scheduleQueue;
        private readonly string paymentQueue;
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
            connectionString = messageConfig.ConnectionString;
            paymentQueue = messageConfig.PaymentQueueName;
            scheduleQueue = messageConfig.ScheduleQueueName;
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
            credits = int.Parse(messageConfig.MessageQueuePreFetch);
            endPoint = $"Endpoint=sb://{messageConfig.MessageQueueHost}/";
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            paymentReceiver = new Receiver<Payment>(connectionString, paymentQueue, paymentAction, credits, endPoint);
            scheduleReceiver = new Receiver<Schedule>(connectionString, scheduleQueue, scheduleAction, credits, endPoint);

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
