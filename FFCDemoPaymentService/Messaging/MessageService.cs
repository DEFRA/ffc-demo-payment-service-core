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
        private readonly MessageConfig messageConfig;
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
            this.messageConfig = messageConfig;
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
            credits = int.Parse(messageConfig.MessageQueuePreFetch);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            paymentReceiver = new Receiver<Payment>(messageConfig, messageConfig.PaymentQueueName, paymentAction, credits);
            scheduleReceiver = new Receiver<Schedule>(messageConfig, messageConfig.ScheduleQueueName, scheduleAction, credits);

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
