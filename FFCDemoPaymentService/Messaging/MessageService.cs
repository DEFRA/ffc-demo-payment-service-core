using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using FFCDemoPaymentService.Messaging.Mapping;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageService : BackgroundService, IMessageService
    {
        MessageConfig messageConfig;
        SqsReceiver scheduleReceiver;
        SqsReceiver paymentReceiver;
        IMessageAction<Schedule> scheduleAction;
        IMessageAction<Payment> paymentAction;


        public MessageService(
            IMessageAction<Schedule> scheduleAction,
            IMessageAction<Payment> paymentAction,
            MessageConfig messageConfig, 
            SqsReceiver scheduleReceiver = null, 
            SqsReceiver paymentReceiver = null)
        {
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
            this.messageConfig = messageConfig;
            this.scheduleReceiver = scheduleReceiver;
            this.paymentReceiver = scheduleReceiver;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartPolling();
            return Task.CompletedTask;
        }

        public void StartPolling()
        {
            SqsConfig scheduleQueueConfig = new ScheduleMap(messageConfig).MapToSqsConfig();
            scheduleReceiver = scheduleReceiver != null ? scheduleReceiver : 
                new SqsReceiver(scheduleQueueConfig, new Action<string>(scheduleAction.ReceiveMessage));
            scheduleReceiver.StartPolling();

            SqsConfig paymentQueueConfig = new PaymentMap(messageConfig).MapToSqsConfig();
            paymentReceiver = paymentReceiver != null ? paymentReceiver : 
                new SqsReceiver(paymentQueueConfig, new Action<string>(paymentAction.ReceiveMessage));
            paymentReceiver.StartPolling();
        }
    }
}
