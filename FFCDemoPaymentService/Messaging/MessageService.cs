using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageService : BackgroundService, IMessageService
    {
        private MessageConfig messageConfig;

        public MessageService(MessageConfig messageConfig)
        {
            this.messageConfig = messageConfig;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartPolling();
            return Task.CompletedTask;
        }

        public void StartPolling()
        {
            var scheduleReceiver = new SqsReceiver();
            Task.Run(()=>scheduleReceiver.StartPolling(messageConfig.ScheduleQueueEndpoint, messageConfig.ScheduleQueueUrl, new Action<string>(ScheduleHandler)));

            var paymentReceiver = new SqsReceiver();
            Task.Run(()=>paymentReceiver.StartPolling(messageConfig.PaymentQueueEndpoint, messageConfig.PaymentQueueUrl, new Action<string>(PaymentHandler)));
        }

        public void ScheduleHandler(string message)
        {
            Console.WriteLine(message);
        }

        public void PaymentHandler(string message)
        {
            Console.WriteLine(message);
        }
    }
}
