using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using FFCDemoPaymentService.Messaging.Mapping;

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
            SqsConfig scheduleQueueConfig = new ScheduleMap(messageConfig).MapToSqsConfig();
            var scheduleReceiver = new SqsReceiver(scheduleQueueConfig, new Action<string>(ScheduleHandler));
            scheduleReceiver.StartPolling();

            SqsConfig paymentQueueConfig = new PaymentMap(messageConfig).MapToSqsConfig();
            var paymentReceiver = new SqsReceiver(paymentQueueConfig, new Action<string>(PaymentHandler));
            paymentReceiver.StartPolling();
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
