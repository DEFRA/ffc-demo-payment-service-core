using System;
using System.Threading;
using System.Threading.Tasks;
using Amqp;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using Microsoft.Extensions.Hosting;

namespace FFCDemoPaymentService.Messaging
{
    public class AmqpService : BackgroundService
    {
        readonly IMessageAction<Schedule> scheduleAction;
        readonly IMessageAction<Payment> paymentAction;

        private AmqpReceiver<Payment> paymentReceiver;
        private AmqpReceiver<Schedule> scheduleReceiver;
        private Session session;
        private Connection connection;
        private string paymentQueue;
        private string scheduleQueue;
        private Address address;

        public AmqpService(
            MessageConfig messageConfig,
            IMessageAction<Schedule> scheduleAction,
            IMessageAction<Payment> paymentAction)
        {
            this.paymentQueue = messageConfig.PaymentQueueName;
            this.scheduleQueue = messageConfig.ScheduleQueueName;
            this.scheduleAction = scheduleAction;
            this.paymentAction = paymentAction;


            this.address = new Address(
                messageConfig.MessageQueueHost,
                Int32.Parse(messageConfig.MessageQueuePort),
                messageConfig.MessageQueueUser,
                messageConfig.MessageQueuePassword,
                "/",
                messageConfig.MessageQueuePort == "5672" ? "AMQP" : "AMQPS"
                );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("opening connection...");
            connection = new Connection(address);
            Console.WriteLine("creating session...");
            session = new Session(connection);
            paymentReceiver = new AmqpReceiver<Payment>(session, paymentQueue, paymentAction);
            scheduleReceiver = new AmqpReceiver<Schedule>(session, scheduleQueue, scheduleAction);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("closing paymentReceiver...");
            paymentReceiver.Close();
            Console.WriteLine("closing scheduleReceiver...");
            scheduleReceiver.Close();
            Console.WriteLine("closing session...");
            session.Close();
            Console.WriteLine("closing connection...");
            connection.Close();
            return Task.CompletedTask;
        }
    }
}