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
            List<Task> tasks = new List<Task>();

            var scheduleReceiver = new SqsReceiver();
            var t1 = Task.Run(() => scheduleReceiver.StartPolling(messageConfig.ScheduleQueueEndpoint, 
                messageConfig.ScheduleQueueUrl, 
                new Action<string>(ScheduleHandler), 
                messageConfig.ScheduleAccessKeyId, 
                messageConfig.ScheduleAccessKey,
                messageConfig.CreateScheduleQueue,
                messageConfig.ScheduleQueueName
                ));
            tasks.Add(t1);

            var paymentReceiver = new SqsReceiver();
            var t2 = Task.Run(() => paymentReceiver.StartPolling(messageConfig.PaymentQueueEndpoint, 
                messageConfig.PaymentQueueUrl, 
                new Action<string>(PaymentHandler), 
                messageConfig.PaymentAccessKeyId, 
                messageConfig.PaymentAccessKey,
                messageConfig.CreatePaymentQueue,
                messageConfig.PaymentQueueName                
                ));
            tasks.Add(t2);

            Task.WaitAll(tasks.ToArray());
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
