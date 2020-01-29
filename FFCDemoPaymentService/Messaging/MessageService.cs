using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

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
            var t1 = Task.Run(() => scheduleReceiver.StartPolling(messageConfig.ScheduleQueueEndpoint,
                messageConfig.ScheduleQueueUrl,
                new Action<string>(ScheduleHandler),
                messageConfig.ScheduleAccessKeyId,
                messageConfig.ScheduleAccessKey,
                messageConfig.CreateScheduleQueue,
                messageConfig.ScheduleQueueName
                ));

            var paymentReceiver = new SqsReceiver();
            var t2 = Task.Run(() => paymentReceiver.StartPolling(messageConfig.PaymentQueueEndpoint,
                messageConfig.PaymentQueueUrl,
                new Action<string>(PaymentHandler),
                messageConfig.PaymentAccessKeyId,
                messageConfig.PaymentAccessKey,
                messageConfig.CreatePaymentQueue,
                messageConfig.PaymentQueueName
                ));
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
