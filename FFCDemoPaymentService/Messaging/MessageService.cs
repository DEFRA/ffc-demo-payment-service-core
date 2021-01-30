using System;
using System.Threading;
using System.Threading.Tasks;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Telemetry;
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
        private readonly ITelemetryProvider telemetryProvider;

        public MessageService(
            MessageConfig messageConfig,
            IMessageAction<Schedule> scheduleAction,
            IMessageAction<Payment> paymentAction,
            ITelemetryProvider telemetryProvider)
        {
            this.messageConfig = messageConfig;
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;
            this.telemetryProvider = telemetryProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            paymentReceiver = new Receiver<Payment>(messageConfig, paymentAction, telemetryProvider);
            scheduleReceiver = new Receiver<Schedule>(messageConfig, scheduleAction, telemetryProvider);
            Task.Run(() => paymentReceiver.ReceiveMessagesAsync(messageConfig.PaymentTopicName, messageConfig.PaymentSubscriptionName)).Wait();
            Task.Run(() => scheduleReceiver.ReceiveMessagesAsync(messageConfig.ScheduleTopicName, messageConfig.ScheduleSubscriptionName)).Wait();
            Console.WriteLine("Ready to schedule payments");
            return Task.CompletedTask;
        }
    }
}
