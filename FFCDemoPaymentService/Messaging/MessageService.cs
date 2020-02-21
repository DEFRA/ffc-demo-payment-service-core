using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using FFCDemoPaymentService.Messaging.Mapping;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;

namespace FFCDemoPaymentService.Messaging
{
    public class MessageService : BackgroundService, IMessageService
    {
        readonly MessageConfig messageConfig;
        IReceiver scheduleReceiver;
        IReceiver paymentReceiver;
        readonly IMessageAction<Schedule> scheduleAction;
        readonly IMessageAction<Payment> paymentAction;


        public MessageService(
            IMessageAction<Schedule> scheduleAction,
            IMessageAction<Payment> paymentAction,
            MessageConfig messageConfig, 
            IReceiver scheduleReceiver = null, 
            IReceiver paymentReceiver = null)
        {
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
            this.messageConfig = messageConfig;
            this.scheduleReceiver = scheduleReceiver;
            this.paymentReceiver = paymentReceiver;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartPolling();
            return Task.CompletedTask;
        }

        public void StartPolling()
        {
            SqsConfig scheduleQueueConfig = new ScheduleMap(messageConfig).MapToSqsConfig();
            scheduleReceiver ??= new SqsReceiver(scheduleQueueConfig, new Action<string>(scheduleAction.ReceiveMessage));
            // scheduleReceiver.StartPolling();

            SqsConfig paymentQueueConfig = new PaymentMap(messageConfig).MapToSqsConfig();
            paymentReceiver ??= new SqsReceiver(paymentQueueConfig, new Action<string>(paymentAction.ReceiveMessage));
            paymentReceiver.StartPolling();
        }
    }
}
